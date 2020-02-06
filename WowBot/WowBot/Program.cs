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
		static void Main(string[] args)
		{
			Bot bot = new Bot();

			while (true)
			{
				ObjectManager.Update();

				//Console.Clear();
				bot.Update();

				
				Thread.Sleep(100);
			}
		}
	}
}
