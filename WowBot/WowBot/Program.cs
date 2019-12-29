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

			while (true)
			{
				ObjectManager.Update();

				Console.Clear();
				bot.Update();

				Thread.Sleep(50);
			}
		}

		private static void Init()
		{
			runHandler = new RunHandler();
		}

	}
}
