using Binarysharp.MemoryManagement;
using Binarysharp.MemoryManagement.Helpers;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using WowBot.BotStuff;

namespace WowBot
{
	class Program
	{
		static RunHandler runHandler;

		static void Main(string[] args)
		{
			Init();
			Bot bot = new Bot();
			//Console.WriteLine(LuaHelper.GetContainerNumFreeSlots());

			while (true)
			{
				//Console.Clear();
				//ObjectManager.GetAllObjects();

				//runHandler.MoveTo(runHandler.Position + new Vector3(1,0,0));

				Thread.Sleep(50);
			}
		}

		private static void Init()
		{
			runHandler = new RunHandler();
		}

	}
}
