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

			var proc = Process.GetProcessesByName("wow")[0];
			Hook hook = new Hook(proc.Id);

			int a = 10;

			while (true)
			{
				//ObjectManager.Update();

				//Console.Clear();
				//bot.Update();
				a--;
				if (a == 0)
				{
					hook.DoString();
				}
				
				if (a == -10) 
				{ 
					hook.GetLocalizedText();
				}
				
				Thread.Sleep(100);
			}
		}
	}
}
