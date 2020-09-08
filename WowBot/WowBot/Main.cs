using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WowBot.BotStuff;

namespace WowBot
{
	class Main
	{
		public EventHandler BackgroundThreadUpdated;

		public void UpdateStuff()
		{
			Bot bot = new Bot();

			var proc = Process.GetProcessesByName("wow")[0];
			Hook hook = new Hook(proc.Id);

			int a = 10;

			ObjectManager.Update();

			Console.WriteLine(PlayerInfo.Instance.Position);
			//bot.Update();
			a--;
			if (a % 10 == 0)
			{
				//hook.DoString();
			}

			if (a == -10)
			{
				//hook.GetLocalizedText();
			}

			BackgroundThreadUpdated?.Invoke(this, null);
		}
	}


}
