using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Magic;
using WowBot.Utils;

namespace WowBot
{
	public class Hook
	{ // Addresse Inection code:
		uint injected_code = 0;
		uint addresseInjection = 0;
		public bool threadHooked = false;
		uint retnInjectionAsm = 0;
		bool InjectionUsed = false;
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
			if (!MemoryHandler.Instance.IsProcessOpen)
			{
				MemoryHandler.Instance.OpenProcessAndThread(_processId);
			}
			if (MemoryHandler.Instance.IsProcessOpen)
			{ // Get address of EndScene
				uint pDevice = MemoryHandler.Instance.ReadUInt(DX_DEVICE);
				uint pEnd = MemoryHandler.Instance.ReadUInt(pDevice + DX_DEVICE_IDX);
				uint pScene = MemoryHandler.Instance.ReadUInt(pEnd);
				uint pEndScene = MemoryHandler.Instance.ReadUInt(pScene + ENDSCENE_IDX);
				if (MemoryHandler.Instance.ReadByte(pEndScene) == 0xE9 && (injected_code == 0 || addresseInjection == 0)) // check if wow is already hooked and dispose Hook
				{
					DisposeHooking();
				}
				if (MemoryHandler.Instance.ReadByte(pEndScene) != 0xE9) // check if wow is already hooked
				{
					try
					{
						pEndScene += 0x2; //köll ez?

						//byte[] originalEndscene = MemoryHandler.Instance.ReadBytes(pEndScene, 5);
						byte[] originalEndscene = new byte[] { 0xB8, 0x51, 0xD7, 0xCA, 0x64 }; 

						uint endsceneReturnAddress = pEndScene + 0x5;
						
						IsThereCodeToExecute = MemoryHandler.Instance.AllocateMemory(4);
						MemoryHandler.Instance.WriteInt(IsThereCodeToExecute, 0);

						returnAddress = MemoryHandler.Instance.AllocateMemory(4);
						MemoryHandler.Instance.WriteInt(returnAddress, 0);

						uint codeCave = MemoryHandler.Instance.AllocateMemory(64);
						codeCaveForInjection = MemoryHandler.Instance.AllocateMemory(256);

						MemoryHandler.Instance.Asm.Clear();
						MemoryHandler.Instance.Asm.AddLine("PUSHFD");
						MemoryHandler.Instance.Asm.AddLine("PUSHAD");

						MemoryHandler.Instance.Asm.AddLine($"MOV EBX, [{IsThereCodeToExecute}]"); //check if ther is code to execute 
						MemoryHandler.Instance.Asm.AddLine("CMP EBX, 1");		//TODO: CHANGE THIS BACK LÉÁTER!%%!!
						MemoryHandler.Instance.Asm.AddLine($"JNE @out"); //if there is no, jump to @out

						//execute our code
						MemoryHandler.Instance.Asm.AddLine($"MOV EDX, {codeCaveForInjection}");		//teygük be a kódunkat EDX be
						MemoryHandler.Instance.Asm.AddLine($"CALL EDX");							//Hívjuk meg
						MemoryHandler.Instance.Asm.AddLine($"MOV [{returnAddress}], EAX");          // [] == dereferálás, a returnAdress ben lévő címre tegyük az eredményt, ne közvetlen a returnAddressbe

						MemoryHandler.Instance.Asm.AddLine($"@out:");
						MemoryHandler.Instance.Asm.AddLine("MOV EDX, 0"); //we have finished the execution, set <isThereCodeToExecute> back to 0
						MemoryHandler.Instance.Asm.AddLine($"MOV [{(IsThereCodeToExecute)}], EDX");

						MemoryHandler.Instance.Asm.AddLine("POPAD");
						MemoryHandler.Instance.Asm.AddLine("POPFD");

						int asmLength = MemoryHandler.Instance.Asm.Assemble().Length;

						MemoryHandler.Instance.Asm.Inject(codeCave);

						MemoryHandler.Instance.Asm.Clear();

						MemoryHandler.Instance.WriteBytes(codeCave + (uint)asmLength, originalEndscene);

						MemoryHandler.Instance.Asm.AddLine($"JMP {endsceneReturnAddress}");
						MemoryHandler.Instance.Asm.Inject(codeCave + (uint)asmLength + 5);
						MemoryHandler.Instance.Asm.Clear();

						MemoryHandler.Instance.Asm.AddLine($"JMP {codeCave}");
						MemoryHandler.Instance.Asm.Inject(pEndScene);

					}
					catch (Exception e)
					{
						
					}

					/*try
					{
						threadHooked = false; // allocate memory to store injected code:
						injected_code = MemoryHandler.Instance.AllocateMemory(2048); // allocate memory the new injection code pointer:
						addresseInjection = MemoryHandler.Instance.AllocateMemory(0x4);
						MemoryHandler.Instance.WriteInt(addresseInjection, 0); // allocate memory the pointer return value:
						retnInjectionAsm = MemoryHandler.Instance.AllocateMemory(0x4);
						MemoryHandler.Instance.WriteInt(retnInjectionAsm, 0); // Generate the STUB to be injected
						MemoryHandler.Instance.Asm.Clear(); // $Asm // save regs
						MemoryHandler.Instance.Asm.AddLine("pushad");
						MemoryHandler.Instance.Asm.AddLine("pushfd"); // Test if you need launch injected code:
						MemoryHandler.Instance.Asm.AddLine("mov eax, [" + addresseInjection + "]");
						MemoryHandler.Instance.Asm.AddLine("test eax, ebx");
						MemoryHandler.Instance.Asm.AddLine("je @out"); // Launch Fonction:
						MemoryHandler.Instance.Asm.AddLine("mov eax, [" + addresseInjection + "]");
						MemoryHandler.Instance.Asm.AddLine("call eax"); // Copie pointer return value:
						MemoryHandler.Instance.Asm.AddLine("mov [" + retnInjectionAsm + "], eax"); // Enter value 0 of addresse func inject
						MemoryHandler.Instance.Asm.AddLine("mov edx, " + addresseInjection);
						MemoryHandler.Instance.Asm.AddLine("mov ecx, 0");
						MemoryHandler.Instance.Asm.AddLine("mov [edx], ecx"); // Close func
						MemoryHandler.Instance.Asm.AddLine("@out:"); // load reg
						MemoryHandler.Instance.Asm.AddLine("popfd");
						MemoryHandler.Instance.Asm.AddLine("popad"); // injected code
						uint sizeAsm = (uint)(MemoryHandler.Instance.Asm.Assemble().Length);
						MemoryHandler.Instance.Asm.Inject(injected_code); // Size asm jumpback
						int sizeJumpBack = 5; // copy and save original instructions
						MemoryHandler.Instance.Asm.Clear();
						MemoryHandler.Instance.Asm.AddLine("mov edi, edi");
						MemoryHandler.Instance.Asm.AddLine("push ebp");
						MemoryHandler.Instance.Asm.AddLine("mov ebp, esp");
						MemoryHandler.Instance.Asm.Inject(injected_code + sizeAsm); // create jump back stub
						MemoryHandler.Instance.Asm.Clear();
						MemoryHandler.Instance.Asm.AddLine("jmp " + (pEndScene + sizeJumpBack));
						MemoryHandler.Instance.Asm.Inject(injected_code + sizeAsm + (uint)sizeJumpBack); // create hook jump
						MemoryHandler.Instance.Asm.Clear(); // $jmpto
						MemoryHandler.Instance.Asm.AddLine("jmp " + (injected_code));
						MemoryHandler.Instance.Asm.Inject(pEndScene);
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
			while (MemoryHandler.Instance.ReadInt(IsThereCodeToExecute) > 0 || InjectionUsed)
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
			string command4 = "print(GetAccountExpansionLevel())";

			uint argCCCommand = MemoryHandler.Instance.AllocateMemory(Encoding.UTF8.GetBytes(command2).Length + 1);
			MemoryHandler.Instance.WriteBytes(argCCCommand, Encoding.UTF8.GetBytes(command2));

			MemoryHandler.Instance.Asm.Clear();

			MemoryHandler.Instance.Asm.AddLine($"MOV EAX, {argCCCommand}");
			MemoryHandler.Instance.Asm.AddLine("PUSH 0");
			MemoryHandler.Instance.Asm.AddLine("PUSH EAX");
			MemoryHandler.Instance.Asm.AddLine("PUSH EAX");
			MemoryHandler.Instance.Asm.AddLine($"MOV EAX, {(uint)Lua.Lua_DoString}");
			MemoryHandler.Instance.Asm.AddLine($"CALL EAX");

			MemoryHandler.Instance.Asm.AddLine("ADD ESP, 0xC");
			MemoryHandler.Instance.Asm.AddLine("RETN");

			// now there is code to be executed
			MemoryHandler.Instance.WriteInt(IsThereCodeToExecute, 1);
			// inject it
			MemoryHandler.Instance.Asm.Inject(codeCaveForInjection);

			while (MemoryHandler.Instance.ReadInt(IsThereCodeToExecute) > 0)
			{
				Thread.Sleep(1);
			}

			MemoryHandler.Instance.FreeMemory(argCCCommand);
			InjectionUsed = false;
		}

		public string GetLocalizedText(string variable)
		{ 
			uint variableAddress = MemoryHandler.Instance.AllocateMemory(Encoding.UTF8.GetBytes(variable).Length + 1); // offset:
			MemoryHandler.Instance.WriteBytes(variableAddress, Encoding.UTF8.GetBytes(variable));

			MemoryHandler.Instance.Asm.Clear();

			MemoryHandler.Instance.Asm.AddLine($"CALL {(uint)Globals.ClntObjMgrGetActivePlayerObj}");
			MemoryHandler.Instance.Asm.AddLine("MOV ECX, EAX");
			MemoryHandler.Instance.Asm.AddLine("PUSH -1");
			MemoryHandler.Instance.Asm.AddLine($"PUSH {variableAddress}");
			MemoryHandler.Instance.Asm.AddLine($"CALL {(uint)Lua.Lua_GetLocalizedText}");
			MemoryHandler.Instance.Asm.AddLine($"RETN");


			// Inject the shit 
			//string sResult = Encoding.ASCII.GetString(MyHook.InjectAndExecute(asm)); // Free memory allocated for command 
			MemoryHandler.Instance.FreeMemory(variableAddress); // Uninstall the hook 
			return null;//return sResult;
		}
		uint dwAddress;
		public void GetLocalizedText()
		{
			while (MemoryHandler.Instance.ReadInt(IsThereCodeToExecute) > 0 || InjectionUsed)
			{
				Thread.Sleep(1);
			}

			Console.WriteLine("gettext");

			string variable = "freeslots";

			uint argCC = MemoryHandler.Instance.AllocateMemory(Encoding.UTF8.GetBytes(variable).Length + 1);
			MemoryHandler.Instance.WriteBytes(argCC, Encoding.UTF8.GetBytes(variable));

			MemoryHandler.Instance.Asm.Clear();

			MemoryHandler.Instance.Asm.AddLine($"CALL {(uint)Globals.ClntObjMgrGetActivePlayerObj}");
			MemoryHandler.Instance.Asm.AddLine("MOV ECX, EAX");
			MemoryHandler.Instance.Asm.AddLine("PUSH -1");
			MemoryHandler.Instance.Asm.AddLine($"PUSH {(argCC)}");
			MemoryHandler.Instance.Asm.AddLine($"CALL {(uint)Lua.Lua_GetLocalizedText}");
			MemoryHandler.Instance.Asm.AddLine("RETN");


			// now there is code to be executed
			MemoryHandler.Instance.WriteInt(IsThereCodeToExecute, 1);
			// inject it
			MemoryHandler.Instance.Asm.Inject(codeCaveForInjection);

			while (MemoryHandler.Instance.ReadInt(IsThereCodeToExecute) > 0)
			{
				Thread.Sleep(1);
			}

			try
			{
				List<byte> returnBytes = new List<byte>();
				dwAddress = MemoryHandler.Instance.ReadUInt(returnAddress);
				byte buffer = MemoryHandler.Instance.ReadByte(dwAddress);
				
				while (buffer != 0)
				{
					returnBytes.Add(buffer);
					dwAddress = dwAddress + 1;
					buffer = MemoryHandler.Instance.ReadByte(dwAddress);
				}

				string result = Encoding.UTF8.GetString(returnBytes.ToArray());

				Console.WriteLine(result);
			}
			catch(Exception e)
			{

			}

			MemoryHandler.Instance.FreeMemory(argCC);
			InjectionUsed = false;
		}

		public void DisposeHooking()
		{
			try
			{ // Offset:
				uint DX_DEVICE = 0xC5DF88;
				uint DX_DEVICE_IDX = 0x397C;
				uint ENDSCENE_IDX = 0xA8; // Get address of EndScene:
				uint pDevice = MemoryHandler.Instance.ReadUInt(DX_DEVICE);
				uint pEnd = MemoryHandler.Instance.ReadUInt(pDevice + DX_DEVICE_IDX);
				uint pScene = MemoryHandler.Instance.ReadUInt(pEnd);
				uint pEndScene = MemoryHandler.Instance.ReadUInt(pScene + ENDSCENE_IDX);
				if (MemoryHandler.Instance.ReadByte(pEndScene) == 0xE9) // check if wow is already hooked and dispose Hook
				{ // Restore origine endscene: 
					MemoryHandler.Instance.Asm.Clear();
					MemoryHandler.Instance.Asm.AddLine("mov edi, edi");
					MemoryHandler.Instance.Asm.AddLine("push ebp");
					MemoryHandler.Instance.Asm.AddLine("mov ebp, esp");
					MemoryHandler.Instance.Asm.Inject(pEndScene);
				} // free memory:
				MemoryHandler.Instance.FreeMemory(injected_code);
				MemoryHandler.Instance.FreeMemory(addresseInjection);
				MemoryHandler.Instance.FreeMemory(retnInjectionAsm);
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
			MemoryHandler.Instance.WriteInt(retnInjectionAsm, 0);
			if (MemoryHandler.Instance.IsProcessOpen && threadHooked)
			{ // Write the asm stuff
				MemoryHandler.Instance.Asm.Clear();
				foreach (string tempLineAsm in asm)
				{
					MemoryHandler.Instance.Asm.AddLine(tempLineAsm);
				} // Allocation Memory
				uint injectionAsm_Codecave = MemoryHandler.Instance.AllocateMemory(MemoryHandler.Instance.Asm.Assemble().Length);
				try
				{ // Inject
					MemoryHandler.Instance.Asm.Inject(injectionAsm_Codecave);
					MemoryHandler.Instance.WriteInt(addresseInjection, (int)injectionAsm_Codecave);
					while (MemoryHandler.Instance.ReadInt(addresseInjection) > 0)
					{
						Thread.Sleep(5);
					} // Wait to launch code
					if (returnLength > 0)
					{
						tempsByte = MemoryHandler.Instance.ReadBytes(MemoryHandler.Instance.ReadUInt(retnInjectionAsm), returnLength);
					}
					else
					{
						byte Buf = new Byte();
						List<byte> retnByte = new List<byte>();
						uint dwAddress = MemoryHandler.Instance.ReadUInt(retnInjectionAsm);
						Buf = MemoryHandler.Instance.ReadByte(dwAddress);
						while (Buf != 0)
						{
							retnByte.Add(Buf);
							dwAddress = dwAddress + 1;
							Buf = MemoryHandler.Instance.ReadByte(dwAddress);
						}
						tempsByte = retnByte.ToArray();
					}
				}
				catch { } // Free memory allocated
				MemoryHandler.Instance.FreeMemory(injectionAsm_Codecave);
			}
			InjectionUsed = false; // return
			return tempsByte;
		}
	}
}
