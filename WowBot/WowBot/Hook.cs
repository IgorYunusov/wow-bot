using System;
using System.Collections.Generic;
using System.Linq;
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
					}
				}
				threadHooked = true;
			}
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
