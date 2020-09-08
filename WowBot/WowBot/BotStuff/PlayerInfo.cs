using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WowBot.Utils;

namespace WowBot
{
	class PlayerInfo
	{
		int Level;
		int Health;
		int Resource;
		int Durability;
		bool IsDead;

		uint playerBase
		{
			get => ObjectManager.GetPlayerBaseAddress();
		}

		public Vector3 Position {
			get
			{
				float x = MemoryHandler.Instance.ReadFloat(playerBase + (int)ObjectOffsets.Pos_X);
				float y = MemoryHandler.Instance.ReadFloat(playerBase + (int)ObjectOffsets.Pos_Y);
				float z = MemoryHandler.Instance.ReadFloat(playerBase + (int)ObjectOffsets.Pos_Z);

				return new Vector3(x, y, z);
			}
		}

		public static PlayerInfo Instance { get; } = new PlayerInfo();
	}
}
