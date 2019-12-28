using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WowBot.BotStuff.State;

namespace WowBot.BotStuff
{
	class Bot
	{
		IBotState state;

		public Bot()
		{

		}

		public void Update()
		{
			DecideState();
			state.Update();

			UpdateAntiAFK();
		}

		private void DecideState()
		{
			state = new CombatState();
		}

		void UpdateAntiAFK()
		{
			Memory.Write<uint>((uint)Globals.LastHardwareAction, Memory.Read<uint>((uint)Globals.Timestamp));
		}
	}
}
