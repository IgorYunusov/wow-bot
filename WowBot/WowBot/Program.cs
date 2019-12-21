using Binarysharp.MemoryManagement;
using Binarysharp.MemoryManagement.Helpers;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace WowBot
{
	class Program
	{
		static RunHandler runHandler;

		static void Main(string[] args)
		{

			Init();

			Thread.Sleep(2000);
			Console.WriteLine(LuaHelper.GetContainerNumFreeSlots());

			while (true)
			{
				Update();
				//Console.Clear();

				//runHandler.MoveTo(runHandler.Position + new Vector3(1,0,0));

				Thread.Sleep(50);
			}
		}

		private static void Init()
		{
			runHandler = new RunHandler();
		}

		private static void Update()
		{
		}

	}
}
