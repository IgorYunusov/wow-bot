using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WowBot.Utils.WowObject
{
	class Unit : GameObject
	{
		public readonly string Name;
		public readonly ReactionType Reaction;

		public Unit(ulong Guid, GOType Type, string Name, ReactionType Reaction) : base(Guid, Type)
		{
			this.Name = Name;
			this.Reaction = Reaction;
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
