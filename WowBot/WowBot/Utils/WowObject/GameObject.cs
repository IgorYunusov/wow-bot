using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WowBot.Utils.WowObject
{
	class GameObject
	{
		protected readonly uint baseAddress;
		public readonly ulong Guid;
		public readonly GOType Type;

		public GameObject(uint baseAddress, ulong Guid, GOType Type)
		{
			this.baseAddress = baseAddress;
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
