using Binarysharp.MemoryManagement;
using Binarysharp.MemoryManagement.Helpers;
using Binarysharp.MemoryManagement.Native;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;
using WowBot.BotStuff;
using WowBot.Utils;
using WowBot.Visuals;

namespace WowBot
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			SetupForm();
		}


		private static void SetupForm()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}

		private static void SetupBot()
		{
		
		}

	}
}
