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
		Unit target;

		public void Update()
		{
			DoBuffsIfNeeded();

			if (null == target)
				DecideTarget();

			if (target.Health == 0)
				Loot();
			else
				RunToTargetAndAutoAttack();

			//Console.WriteLine(target.Health);

			DoRotation();
		}

		private void Loot()
		{
			CTM.SetGuid(target.Guid);
			CTM.SetPosition(target.Position);
			CTM.SetAction(CTMAction.Loot);
		}

		private void DecideTarget()
		{
			List<Unit> enemies = ObjectManager.GetEnemies().
				//Where(e => e.Health > 0).
				OrderBy(e => Vector3.Distance(e.Position, Bot.runHandler.Position)).ToList();
			
			target = enemies[0];
		}

		private void RunToTargetAndAutoAttack()
		{
			CTM.SetGuid(target.Guid);
			CTM.SetPosition(target.Position);
			CTM.SetAction(CTMAction.Attack);
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
