using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WowBot.Utils.WowObject
{
	class GameObject
	{
		public readonly ulong Guid;
		public readonly GOType Type;

		public GameObject(ulong Guid, GOType Type)
		{
			this.Guid = Guid;
			this.Type = Type;
		}
	}

	enum GOType
	{
		None,
		Item,
		Container,
		Unit,
		Player,
		GameObject,
		DynamicObject,
		Corpse
	}
}
