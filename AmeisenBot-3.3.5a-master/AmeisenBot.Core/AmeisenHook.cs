using AmeisenBotLogger;
using AmeisenBotUtilities;
using Magic;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace AmeisenBotCore
{
    /// <summary>
    /// Class that manages the hooking of WoW's EndScene
    /// </summary>
    public class AmeisenHook
    {
        public bool isHooked = false;
        public bool isInjectionUsed = false;

        public AmeisenHook(BlackMagic blackmagic)
        {
            BlackMagic = blackmagic;
            Hook();
            hookJobs = new ConcurrentQueue<HookJob>();
            hookWorker = new Thread(new ThreadStart(DoWork));

            // wait for the world to be loaded
            while (!AmeisenCore.IsWorldLoaded())
            {
                Thread.Sleep(200);
            }

            IsNotInWorld = false;

            if (isHooked)
            {
                hookWorker.Start();
            }
        }

        public bool IsNotInWorld { get; set; }
        public int JobCount => hookJobs.Count;

        /// <summary>
        /// Execute asm on the hook, if the hook finished executing the job
        /// it will set the IsFinished property to true, that's why you supply
        /// the job as a reference.
        /// </summary>
        /// <param name="hookJob"></param>
        public void AddHookJob(ref HookJob hookJob) 
            => hookJobs.Enqueue(hookJob);

        /// <summary>
        /// Execute asm on the hook, if the hook finished executing the job
        /// it will set the IsFinished property to true, that's why you supply
        /// the job as a reference.
        /// 
        /// You can supply a ReturnHookJob to read a variable directly after
        /// you executed something else.
        /// </summary>
        /// <param name="hookJob"></param>
        public void AddHookJob(ref ReturnHookJob hookJob) 
            => hookJobs.Enqueue(hookJob);

        public void DisposeHooking()
        {
            // get D3D9 Endscene Pointer
            uint endscene = GetEndScene();
            endscene += ENDSCENE_HOOK_OFFSET;

            // check if WoW is hooked
            if (BlackMagic.ReadByte(endscene) == 0xE9)
            {
                BlackMagic.WriteBytes(endscene, originalEndscene);

                BlackMagic.FreeMemory(codeCave);
                BlackMagic.FreeMemory(codeToExecute);
                BlackMagic.FreeMemory(codeCaveForInjection);
            }

            isHooked = false;
            hookWorker.Join();
        }

        private const uint ENDSCENE_HOOK_OFFSET = 0x2;
        private uint codeCave;
        private uint codeCaveForInjection;
        private uint codeToExecute;
        private uint endsceneReturnAddress;
        private ConcurrentQueue<HookJob> hookJobs;
        private Thread hookWorker;
        private byte[] originalEndscene = new byte[] { 0xB8, 0x51, 0xD7, 0xCA, 0x64 };
        private uint returnAdress;
        private BlackMagic BlackMagic { get; set; }

        private void DoWork()
        {
            while (isHooked)
            {
                // do not do anything while we are in a loadingscreen
                // WoW doesn't like that and will crash
                if (AmeisenCore.IsInLoadingScreen())
                {
                    Thread.Sleep(50);
                    continue;
                }

                if (!hookJobs.IsEmpty)
                {
                    if (hookJobs.TryDequeue(out HookJob currentJob))
                    {
                        // process a hook job
                        InjectAndExecute(currentJob.Asm, currentJob.ReadReturnBytes, out bool wasJobSuccessful);

                        // if its a chained hook job, execute it too
                        if (currentJob.GetType() == typeof(ReturnHookJob) && wasJobSuccessful)
                        {
                            currentJob.ReturnValue = InjectAndExecute(
                                ((ReturnHookJob)currentJob).ChainedJob.Asm,
                                ((ReturnHookJob)currentJob).ChainedJob.ReadReturnBytes,
                                out bool wasChainedJobSuccessful);
                        }

                        currentJob.IsFinished = true;
                    }
                }
                Thread.Sleep(1);
            }
        }

        private uint GetEndScene()
        {
            uint pDevice = BlackMagic.ReadUInt(Offsets.devicePtr1);
            uint pEnd = BlackMagic.ReadUInt(pDevice + Offsets.devicePtr2);
            uint pScene = BlackMagic.ReadUInt(pEnd);
            return BlackMagic.ReadUInt(pScene + Offsets.endScene);
        }

        private void Hook()
        {
            if (BlackMagic.IsProcessOpen)
            {
                // get D3D9 Endscene Pointer
                uint endscene = GetEndScene();

                // if WoW is already hooked, unhook it
                if (BlackMagic.ReadByte(endscene) == 0xE9)
                {
                    originalEndscene = new byte[] { 0xB8, 0x51, 0xD7, 0xCA, 0x64 };
                    DisposeHooking();
                }

                try
                {
                    // if WoW is now/was unhooked, hook it
                    if (BlackMagic.ReadByte(endscene) != 0xE9)
                    {
                        // first thing thats 5 bytes big is here
                        // we are going to replace this 5 bytes with
                        // our JMP instruction (JMP (1 byte) + Address (4 byte))
                        endscene += ENDSCENE_HOOK_OFFSET;

                        // the address that we will return to after 
                        // the jump wer'e going to inject
                        endsceneReturnAddress = endscene + 0x5;

                        // read our original EndScene
                        //originalEndscene = BlackMagic.ReadBytes(endscene, 5);

                        // integer to check if there is code waiting to be executed
                        codeToExecute = BlackMagic.AllocateMemory(4);
                        BlackMagic.WriteInt(codeToExecute, 0);

                        // integer to save the address of the return value
                        returnAdress = BlackMagic.AllocateMemory(4);
                        BlackMagic.WriteInt(returnAdress, 0);

                        // codecave to check if we need to execute something
                        codeCave = BlackMagic.AllocateMemory(64);
                        // codecave for the code we wa't to execute
                        codeCaveForInjection = BlackMagic.AllocateMemory(256);

                        AmeisenLogger.Instance.Log(LogLevel.DEBUG, $"EndScene at: 0x{endscene.ToString("X")}", this);
                        AmeisenLogger.Instance.Log(LogLevel.DEBUG, $"EndScene returning at: 0x{(endsceneReturnAddress).ToString("X")}", this);
                        AmeisenLogger.Instance.Log(LogLevel.DEBUG, $"CodeCave at: 0x{codeCave.ToString("X")}", this);
                        AmeisenLogger.Instance.Log(LogLevel.DEBUG, $"CodeCaveForInjection at: 0x{codeCaveForInjection.ToString("X")}", this);
                        AmeisenLogger.Instance.Log(LogLevel.DEBUG, $"CodeToExecute at: 0x{codeToExecute.ToString("X")}", this);
                        AmeisenLogger.Instance.Log(LogLevel.DEBUG, $"Original Endscene bytes: {Utils.ByteArrayToString(originalEndscene)}", this);

                        BlackMagic.Asm.Clear();
                        // save registers
                        BlackMagic.Asm.AddLine("PUSHFD");
                        BlackMagic.Asm.AddLine("PUSHAD");

                        // check for code to be executed
                        BlackMagic.Asm.AddLine($"MOV EBX, [{(codeToExecute)}]");
                        BlackMagic.Asm.AddLine("TEST EBX, 1");
                        BlackMagic.Asm.AddLine("JE @out");

                        // execute our stuff and get return address
                        BlackMagic.Asm.AddLine($"MOV EDX, {(codeCaveForInjection)}");
                        BlackMagic.Asm.AddLine("CALL EDX");
                        BlackMagic.Asm.AddLine($"MOV [{(returnAdress)}], EAX");

                        // finish up our execution
                        BlackMagic.Asm.AddLine("@out:");
                        BlackMagic.Asm.AddLine("MOV EDX, 0");
                        BlackMagic.Asm.AddLine($"MOV [{(codeToExecute)}], EDX");

                        // restore registers
                        BlackMagic.Asm.AddLine("POPAD");
                        BlackMagic.Asm.AddLine("POPFD");

                        // needed to determine the position where the original
                        // asm is going to be placed
                        int asmLenght = BlackMagic.Asm.Assemble().Length;

                        // inject the instructions into our codecave
                        BlackMagic.Asm.Inject(codeCave);
                        // ---------------------------------------------------
                        // End of the code that checks if there is asm to be
                        // executed on our hook
                        // ---------------------------------------------------

                        // Prepare to replace the instructions inside WoW
                        BlackMagic.Asm.Clear();

                        // do the original EndScene stuff after we restored the registers
                        // and insert it after our code
                        BlackMagic.WriteBytes(codeCave + (uint)asmLenght, originalEndscene);

                        // return to original function after we're done with our stuff
                        BlackMagic.Asm.AddLine($"JMP {(endsceneReturnAddress)}");
                        BlackMagic.Asm.Inject((codeCave + (uint)asmLenght) + 5);
                        BlackMagic.Asm.Clear();
                        // ---------------------------------------------------
                        // End of doing the original stuff and returning to
                        // the original instruction
                        // ---------------------------------------------------

                        // modify original EndScene instructions to start the hook
                        BlackMagic.Asm.AddLine($"JMP {(codeCave)}");
                        BlackMagic.Asm.Inject(endscene);
                        // we should've hooked WoW now
                    }
                    isHooked = true;
                }
                catch { isHooked = false; }
            }
        }

        /// <summary>
        /// Inject assembly code on our hook
        /// </summary>
        /// <param name="asm">assembly to execute</param>
        /// <param name="readReturnBytes">should the return bytes get read</param>
        /// <param name="successful">if the reading of return bytes was successful</param>
        /// <returns></returns>
        private byte[] InjectAndExecute(string[] asm, bool readReturnBytes, out bool successful)
        {
            List<byte> returnBytes = new List<byte>();

            try
            {
                // wait for the code to be executed
                while (isInjectionUsed || AmeisenCore.IsInLoadingScreen() || BlackMagic.ReadInt(codeToExecute) > 0)
                {
                    Thread.Sleep(5);
                }

                isInjectionUsed = true;

                // preparing to inject the given ASM
                BlackMagic.Asm.Clear();
                // add all lines
                foreach (string s in asm)
                {
                    BlackMagic.Asm.AddLine(s);
                }

                // now there is code to be executed
                BlackMagic.WriteInt(codeToExecute, 1);
                // inject it
                BlackMagic.Asm.Inject(codeCaveForInjection);

                // we don't need this atm
                //AmeisenManager.Instance().GetBlackMagic().Asm.AddLine("JMP " + (endsceneReturnAddress));
                //int asmLenght = BlackMagic.Asm.Assemble().Length;

                // wait for the code to be executed
                while (BlackMagic.ReadInt(codeToExecute) > 0)
                {
                    Thread.Sleep(1);
                }

                // if we want to read the return value do it otherwise we're done
                if (readReturnBytes)
                {
                    byte buffer = new byte();
                    try
                    {
                        // get our return parameter address
                        uint dwAddress = BlackMagic.ReadUInt(returnAdress);

                        // read all parameter-bytes until we the buffer is 0
                        buffer = BlackMagic.ReadByte(dwAddress);
                        while (buffer != 0)
                        {
                            returnBytes.Add(buffer);
                            dwAddress = dwAddress + 1;
                            buffer = BlackMagic.ReadByte(dwAddress);
                        }
                    }
                    catch (Exception e)
                    {
                        AmeisenLogger.Instance.Log(
                            LogLevel.ERROR,
                            $"Crash at reading returnAddress: {e.ToString()}",
                            this);
                    }
                }
            }
            catch (Exception e)
            {
                // now there is no more code to be executed
                BlackMagic.WriteInt(codeToExecute, 0);
                successful = false;

                AmeisenLogger.Instance.Log(
                    LogLevel.ERROR,
                    $"Crash at InjectAndExecute: {e.ToString()}",
                    this);

                foreach (string s in asm)
                {
                    AmeisenLogger.Instance.Log(
                        LogLevel.ERROR,
                        $"ASM Content: {s}",
                        this);
                }

                AmeisenLogger.Instance.Log(
                    LogLevel.ERROR,
                    $"ReadReturnBytes: {readReturnBytes}",
                    this);
            }

            // now we can use the hook again
            isInjectionUsed = false;
            successful = true;

            return returnBytes.ToArray();
        }
    }

    /// <summary>
    /// Job to execute ASM code on the endscene hook
    /// </summary>
    public class HookJob
    {
        /// <summary>
        /// Build a job to execute on the endscene hook
        /// </summary>
        /// <param name="asm">ASM to execute</param>
        /// <param name="readReturnBytes">read the return bytes</param>
        public HookJob(string[] asm, bool readReturnBytes)
        {
            IsFinished = false;
            Asm = asm;
            ReadReturnBytes = readReturnBytes;
            ReturnValue = null;
        }

        public string[] Asm { get; set; }
        public bool IsFinished { get; set; }
        public bool ReadReturnBytes { get; set; }
        public object ReturnValue { get; set; }
    }

    /// <summary>
    /// At the moment used for GetLocalizedText, to chain-execute Jobs
    /// </summary>
    public class ReturnHookJob : HookJob
    {
        /// <summary>
        /// Build a job to execute on the endscene hook
        /// </summary>
        /// <param name="asm">ASM to execute</param>
        /// <param name="readReturnBytes">read the return bytes</param>
        /// <param name="chainedJob">
        /// Job to execute after running the main Job, for example GetLocalizedText stuff
        /// </param>
        public ReturnHookJob(string[] asm, bool readReturnBytes, HookJob chainedJob)
            : base(asm, readReturnBytes) { ChainedJob = chainedJob; }

        public HookJob ChainedJob { get; private set; }
    }
}