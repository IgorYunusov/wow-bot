using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WowBot.Utils;
using WowBot.Utils.WowObject;

namespace WowBot.BotStuff.State
{
	class CombatState : IBotState
	{
		string[] buffs = { Spell.RetributionAura, Spell.BlessingOfWisdom, Spell.SealOfWisdom};

		public void Update()
		{
			DoBuffsIfNeeded();

			DecideTarget();
			RunToTarget();

			DoRotation();
		}

		private void DecideTarget()
		{
			var a = ObjectManager.GetEnemies();
			foreach(var b in a)
			{
				Console.WriteLine(b.Name);
			}
		}

		private void RunToTarget()
		{
			
		}

		private void DoRotation()
		{
			
		}

		void DoBuffsIfNeeded()
		{
			foreach (string b in buffs)
			{
				if(LuaHelper.IsSpellCastable(b) && !LuaHelper.IsAuraActive(b))
				{
					LuaHelper.CastSpellByName(b);
					Thread.Sleep(CoolDown.Global);
				}
			}
		}
	}
}
