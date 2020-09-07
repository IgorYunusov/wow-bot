using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Magic;

namespace WowBot
{
	public class Hook
	{ // Addresse Inection code:
		uint injected_code = 0;
		uint addresseInjection = 0;
		public bool threadHooked = false;
		uint retnInjectionAsm = 0;
		bool InjectionUsed = false;
		public BlackMagic Memory = new BlackMagic();
		public int _processId = 0;

		public uint IsThereCodeToExecute { get; private set; }
		uint codeCaveForInjection;
		uint returnAddress;


		public Hook(int processId)
		{
			_processId = processId;
			Hooking();
		}
		public void Hooking()
		{ // Offset: 
			uint DX_DEVICE = 0xC5DF88;
			uint DX_DEVICE_IDX = 0x397C;
			uint ENDSCENE_IDX = 0xA8; // Process Connect:
			if (!Memory.IsProcessOpen)
			{
				Memory.OpenProcessAndThread(_processId);
			}
			if (Memory.IsProcessOpen)
			{ // Get address of EndScene
				uint pDevice = Memory.ReadUInt(DX_DEVICE);
				uint pEnd = Memory.ReadUInt(pDevice + DX_DEVICE_IDX);
				uint pScene = Memory.ReadUInt(pEnd);
				uint pEndScene = Memory.ReadUInt(pScene + ENDSCENE_IDX);
				if (Memory.ReadByte(pEndScene) == 0xE9 && (injected_code == 0 || addresseInjection == 0)) // check if wow is already hooked and dispose Hook
				{
					DisposeHooking();
				}
				if (Memory.ReadByte(pEndScene) != 0xE9) // check if wow is already hooked
				{
					try
					{
						pEndScene += 0x2; //köll ez?

						//byte[] originalEndscene = Memory.ReadBytes(pEndScene, 5);
						byte[] originalEndscene = new byte[] { 0xB8, 0x51, 0xD7, 0xCA, 0x64 }; 

						uint endsceneReturnAddress = pEndScene + 0x5;
						
						IsThereCodeToExecute = Memory.AllocateMemory(4);
						Memory.WriteInt(IsThereCodeToExecute, 0);

						returnAddress = Memory.AllocateMemory(4);
						Memory.WriteInt(returnAddress, 0);

						uint codeCave = Memory.AllocateMemory(64);
						codeCaveForInjection = Memory.AllocateMemory(256);

						Memory.Asm.Clear();
						Memory.Asm.AddLine("PUSHFD");
						Memory.Asm.AddLine("PUSHAD");

						Memory.Asm.AddLine($"MOV EBX, [{IsThereCodeToExecute}]"); //check if ther is code to execute 
						Memory.Asm.AddLine("CMP EBX, 1");		//TODO: CHANGE THIS BACK LÉÁTER!%%!!
						Memory.Asm.AddLine($"JNE @out"); //if there is no, jump to @out

						//execute our code
						Memory.Asm.AddLine($"MOV EDX, {codeCaveForInjection}");		//teygük be a kódunkat EDX be
						Memory.Asm.AddLine($"CALL EDX");							//Hívjuk meg
						Memory.Asm.AddLine($"MOV [{returnAddress}], EAX");          // [] == dereferálás, a returnAdress ben lévő címre tegyük az eredményt, ne közvetlen a returnAddressbe

						Memory.Asm.AddLine($"@out:");
						Memory.Asm.AddLine("MOV EDX, 0"); //we have finished the execution, set <isThereCodeToExecute> back to 0
						Memory.Asm.AddLine($"MOV [{(IsThereCodeToExecute)}], EDX");

						Memory.Asm.AddLine("POPAD");
						Memory.Asm.AddLine("POPFD");

						int asmLength = Memory.Asm.Assemble().Length;

						Memory.Asm.Inject(codeCave);

						Memory.Asm.Clear();

						Memory.WriteBytes(codeCave + (uint)asmLength, originalEndscene);

						Memory.Asm.AddLine($"JMP {endsceneReturnAddress}");
						Memory.Asm.Inject(codeCave + (uint)asmLength + 5);
						Memory.Asm.Clear();

						Memory.Asm.AddLine($"JMP {codeCave}");
						Memory.Asm.Inject(pEndScene);

					}
					catch (Exception e)
					{
						
					}

					/*try
					{
						threadHooked = false; // allocate memory to store injected code:
						injected_code = Memory.AllocateMemory(2048); // allocate memory the new injection code pointer:
						addresseInjection = Memory.AllocateMemory(0x4);
						Memory.WriteInt(addresseInjection, 0); // allocate memory the pointer return value:
						retnInjectionAsm = Memory.AllocateMemory(0x4);
						Memory.WriteInt(retnInjectionAsm, 0); // Generate the STUB to be injected
						Memory.Asm.Clear(); // $Asm // save regs
						Memory.Asm.AddLine("pushad");
						Memory.Asm.AddLine("pushfd"); // Test if you need launch injected code:
						Memory.Asm.AddLine("mov eax, [" + addresseInjection + "]");
						Memory.Asm.AddLine("test eax, ebx");
						Memory.Asm.AddLine("je @out"); // Launch Fonction:
						Memory.Asm.AddLine("mov eax, [" + addresseInjection + "]");
						Memory.Asm.AddLine("call eax"); // Copie pointer return value:
						Memory.Asm.AddLine("mov [" + retnInjectionAsm + "], eax"); // Enter value 0 of addresse func inject
						Memory.Asm.AddLine("mov edx, " + addresseInjection);
						Memory.Asm.AddLine("mov ecx, 0");
						Memory.Asm.AddLine("mov [edx], ecx"); // Close func
						Memory.Asm.AddLine("@out:"); // load reg
						Memory.Asm.AddLine("popfd");
						Memory.Asm.AddLine("popad"); // injected code
						uint sizeAsm = (uint)(Memory.Asm.Assemble().Length);
						Memory.Asm.Inject(injected_code); // Size asm jumpback
						int sizeJumpBack = 5; // copy and save original instructions
						Memory.Asm.Clear();
						Memory.Asm.AddLine("mov edi, edi");
						Memory.Asm.AddLine("push ebp");
						Memory.Asm.AddLine("mov ebp, esp");
						Memory.Asm.Inject(injected_code + sizeAsm); // create jump back stub
						Memory.Asm.Clear();
						Memory.Asm.AddLine("jmp " + (pEndScene + sizeJumpBack));
						Memory.Asm.Inject(injected_code + sizeAsm + (uint)sizeJumpBack); // create hook jump
						Memory.Asm.Clear(); // $jmpto
						Memory.Asm.AddLine("jmp " + (injected_code));
						Memory.Asm.Inject(pEndScene);
					}
					catch
					{
						threadHooked = false;
						return;
					}*/
				}
				threadHooked = true;
			}
		}
		
		public void DoString()
		{
			while (Memory.ReadInt(IsThereCodeToExecute) > 0 || InjectionUsed)
			{
				Thread.Sleep(1);
			}
			InjectionUsed = true;

			Console.WriteLine("dostring");

			string variable = "freeslots";
			string command = $"DEFAULT_CHAT_FRAME: AddMessage(\"SZOPKIAGECIM\")";
			string command2 = $"{variable} = GetContainerNumFreeSlots(0) + GetContainerNumFreeSlots(1) + GetContainerNumFreeSlots(2) + GetContainerNumFreeSlots(3) + GetContainerNumFreeSlots(4);" +
							  $"DEFAULT_CHAT_FRAME: AddMessage({variable})";
			string command3 = $"{variable} = \"YES\"";

			uint argCCCommand = Memory.AllocateMemory(Encoding.UTF8.GetBytes(command2).Length + 1);
			Memory.WriteBytes(argCCCommand, Encoding.UTF8.GetBytes(command2));

			Memory.Asm.Clear();

			Memory.Asm.AddLine($"MOV EAX, {argCCCommand}");
			Memory.Asm.AddLine("PUSH 0");
			Memory.Asm.AddLine("PUSH EAX");
			Memory.Asm.AddLine("PUSH EAX");
			Memory.Asm.AddLine($"MOV EAX, {(uint)Lua.Lua_DoString}");
			Memory.Asm.AddLine($"CALL EAX");

			Memory.Asm.AddLine("ADD ESP, 0xC");
			Memory.Asm.AddLine("RETN");

			// now there is code to be executed
			Memory.WriteInt(IsThereCodeToExecute, 1);
			// inject it
			Memory.Asm.Inject(codeCaveForInjection);

			while (Memory.ReadInt(IsThereCodeToExecute) > 0)
			{
				Thread.Sleep(1);
			}

			Memory.FreeMemory(argCCCommand);
			InjectionUsed = false;
		}

		public string GetLocalizedText(string variable)
		{ 
			uint variableAddress = Memory.AllocateMemory(Encoding.UTF8.GetBytes(variable).Length + 1); // offset:
			Memory.WriteBytes(variableAddress, Encoding.UTF8.GetBytes(variable));

			Memory.Asm.Clear();

			Memory.Asm.AddLine($"CALL {(uint)Globals.ClntObjMgrGetActivePlayerObj}");
			Memory.Asm.AddLine("MOV ECX, EAX");
			Memory.Asm.AddLine("PUSH -1");
			Memory.Asm.AddLine($"PUSH {variableAddress}");
			Memory.Asm.AddLine($"CALL {(uint)Lua.Lua_GetLocalizedText}");
			Memory.Asm.AddLine($"RETN");


			// Inject the shit 
			//string sResult = Encoding.ASCII.GetString(MyHook.InjectAndExecute(asm)); // Free memory allocated for command 
			Memory.FreeMemory(variableAddress); // Uninstall the hook 
			return null;//return sResult;
		}
		uint dwAddress;
		public void GetLocalizedText()
		{
			while (Memory.ReadInt(IsThereCodeToExecute) > 0 || InjectionUsed)
			{
				Thread.Sleep(1);
			}

			Console.WriteLine("gettext");

			string variable = "freeslots";

			uint argCC = Memory.AllocateMemory(Encoding.UTF8.GetBytes(variable).Length + 1);
			Memory.WriteBytes(argCC, Encoding.UTF8.GetBytes(variable));

			Memory.Asm.Clear();

			Memory.Asm.AddLine($"CALL {(uint)Globals.ClntObjMgrGetActivePlayerObj}");
			Memory.Asm.AddLine("MOV ECX, EAX");
			Memory.Asm.AddLine("PUSH -1");
			Memory.Asm.AddLine($"PUSH {(argCC)}");
			Memory.Asm.AddLine($"CALL {(uint)Lua.Lua_GetLocalizedText}");
			Memory.Asm.AddLine("RETN");


			// now there is code to be executed
			Memory.WriteInt(IsThereCodeToExecute, 1);
			// inject it
			Memory.Asm.Inject(codeCaveForInjection);

			while (Memory.ReadInt(IsThereCodeToExecute) > 0)
			{
				Thread.Sleep(1);
			}

			try
			{
				List<byte> returnBytes = new List<byte>();
				dwAddress = Memory.ReadUInt(returnAddress);
				byte buffer = Memory.ReadByte(dwAddress);
				
				while (buffer != 0)
				{
					returnBytes.Add(buffer);
					dwAddress = dwAddress + 1;
					buffer = Memory.ReadByte(dwAddress);
				}

				string result = Encoding.UTF8.GetString(returnBytes.ToArray());

				Console.WriteLine(result);
			}
			catch(Exception e)
			{

			}

			Memory.FreeMemory(argCC);
			InjectionUsed = false;
		}

		public void DisposeHooking()
		{
			try
			{ // Offset:
				uint DX_DEVICE = 0xC5DF88;
				uint DX_DEVICE_IDX = 0x397C;
				uint ENDSCENE_IDX = 0xA8; // Get address of EndScene:
				uint pDevice = Memory.ReadUInt(DX_DEVICE);
				uint pEnd = Memory.ReadUInt(pDevice + DX_DEVICE_IDX);
				uint pScene = Memory.ReadUInt(pEnd);
				uint pEndScene = Memory.ReadUInt(pScene + ENDSCENE_IDX);
				if (Memory.ReadByte(pEndScene) == 0xE9) // check if wow is already hooked and dispose Hook
				{ // Restore origine endscene: 
					Memory.Asm.Clear();
					Memory.Asm.AddLine("mov edi, edi");
					Memory.Asm.AddLine("push ebp");
					Memory.Asm.AddLine("mov ebp, esp");
					Memory.Asm.Inject(pEndScene);
				} // free memory:
				Memory.FreeMemory(injected_code);
				Memory.FreeMemory(addresseInjection);
				Memory.FreeMemory(retnInjectionAsm);
			}
			catch { }
		}
		public byte[] InjectAndExecute(string[] asm, int returnLength = 0)
		{
			while (InjectionUsed)
			{
				Thread.Sleep(5);
			}
			InjectionUsed = true; // Hook Wow:
			Hooking();
			byte[] tempsByte = new byte[0]; // reset return value pointer
			Memory.WriteInt(retnInjectionAsm, 0);
			if (Memory.IsProcessOpen && threadHooked)
			{ // Write the asm stuff
				Memory.Asm.Clear();
				foreach (string tempLineAsm in asm)
				{
					Memory.Asm.AddLine(tempLineAsm);
				} // Allocation Memory
				uint injectionAsm_Codecave = Memory.AllocateMemory(Memory.Asm.Assemble().Length);
				try
				{ // Inject
					Memory.Asm.Inject(injectionAsm_Codecave);
					Memory.WriteInt(addresseInjection, (int)injectionAsm_Codecave);
					while (Memory.ReadInt(addresseInjection) > 0)
					{
						Thread.Sleep(5);
					} // Wait to launch code
					if (returnLength > 0)
					{
						tempsByte = Memory.ReadBytes(Memory.ReadUInt(retnInjectionAsm), returnLength);
					}
					else
					{
						byte Buf = new Byte();
						List<byte> retnByte = new List<byte>();
						uint dwAddress = Memory.ReadUInt(retnInjectionAsm);
						Buf = Memory.ReadByte(dwAddress);
						while (Buf != 0)
						{
							retnByte.Add(Buf);
							dwAddress = dwAddress + 1;
							Buf = Memory.ReadByte(dwAddress);
						}
						tempsByte = retnByte.ToArray();
					}
				}
				catch { } // Free memory allocated
				Memory.FreeMemory(injectionAsm_Codecave);
			}
			InjectionUsed = false; // return
			return tempsByte;
		}
	}
}
