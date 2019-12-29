using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WowBot.BotStuff;

namespace WowBot.Utils.WowObject
{
	class Unit : GameObject
	{
		public readonly string Name;
		public readonly ReactionType Reaction;

		//TODO: this may go to gameopbject later, not sure yet
		public Vector3 Position {
			get
			{
				return new Vector3
				(
					Memory.Read<float>(baseAddress + (int)ObjectOffsets.Pos_X),
					Memory.Read<float>(baseAddress + (int)ObjectOffsets.Pos_Y),
					Memory.Read<float>(baseAddress + (int)ObjectOffsets.Pos_Z)
				);
			}
		}

		public Unit(uint baseAddress, ulong Guid, GOType Type, string Name, ReactionType Reaction) : base(baseAddress, Guid, Type)
		{
			this.Name = Name;
			this.Reaction = Reaction;
		}

		public bool IsAttackable()
		{
			return Reaction <= ReactionType.Neutral;
		}

		public enum ReactionType
		{
			Unknown = 0,
			Exceptionally_Hostile = 1,
			Very_Hostile = 2,
			Hostile = 3,
			Neutral = 4,
			Friendly = 5,
			Very_Friendly = 6,
			Exceptionally_friendly = 7,
			Exalted = 8,
		}
	}
}
