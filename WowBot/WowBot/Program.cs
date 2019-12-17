using Binarysharp.MemoryManagement;
using Binarysharp.MemoryManagement.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WowBot
{
	class Program
	{
		const int PROCESS_WM_READ = 0x0010;

		[DllImport("kernel32.dll")]
		public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

		[DllImport("kernel32.dll")]
		public static extern bool ReadProcessMemory(int hProcess,  int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

		static RunHandler runHandler;

		static void Main(string[] args)
		{
			Init();

			int a = ObjectManager.GetPlayerAdress();



			while (true)
			{
				Update();
				Console.Clear();
				Console.WriteLine(Convert.ToString(Memory.ReadByte(0x00CA11D8 + 0x1C), 2));
				//Console.WriteLine(Convert.ToString(Memory.ReadByte(0x00CA11D8 + 0x1C), 2));
				runHandler.MoveTo(runHandler.Position + new Vector3(1, 0, 0));

				//Console.WriteLine(a);

				Thread.Sleep(50);
			}
		}

		private static void Init()
		{
			runHandler = new RunHandler();
		}

		private static void Update()
		{
			runHandler.Update();
		}
	}
}
