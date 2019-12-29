using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WowBot.Utils.WowObject;
using static WowBot.Utils.WowObject.Unit;

namespace WowBot
{
	static class ObjectManager
	{
		static uint objectManager;
		static List<GameObject> gameObjects = new List<GameObject>();
		static DateTime lastScanTime;
		const int UpdateAfterMillis = 500;

		static Dictionary<ulong, ReactionType> reactionCache = new Dictionary<ulong, ReactionType>();

		static ObjectManager()
		{
			uint CurMgr = Memory.Read<uint>((uint)ObjectManagerEnum.CurMgrPointer);
			objectManager = Memory.Read<uint>(CurMgr + (uint)ObjectManagerEnum.CurMgrOffset);
			lastScanTime = DateTime.Now;
		}

		public static void Update()
		{
			if ((DateTime.Now - lastScanTime).TotalMilliseconds > UpdateAfterMillis)
			{
				CollectAllObjects();
				lastScanTime = DateTime.Now;
			}
		}

		public static List<Unit> GetEnemies()
		{
			return gameObjects.OfType<Unit>().Where(u => u.IsAttackable()).ToList();
		}

		public static List<T> GetObjects<T>()
		{
			return gameObjects.OfType<T>().ToList();
		}

		public static void CollectAllObjects()
		{
			gameObjects.Clear();

			uint currObjPtr = objectManager + (uint)ObjectManagerEnum.FirstObject;
			uint currentObjBaseAddress = Memory.Read<uint>(currObjPtr);

			// don't know why but currentObjBase % 2 operation makes it work
			while (currentObjBaseAddress != uint.MinValue && currentObjBaseAddress%2 == uint.MinValue) 
			{
				float x = Memory.Read<float>(currentObjBaseAddress + (int)ObjectOffsets.Pos_X);
				ulong guid = Memory.Read<ulong>(currentObjBaseAddress + (int)ObjectOffsets.Guid);
				GOType type = (GOType) Memory.Read<short>(currentObjBaseAddress + (uint)ObjectOffsets.Type);

				switch (type)
				{
					case GOType.None:
						break;
					case GOType.Item:
						break;
					case GOType.Container:
						break;
					case GOType.Unit:
						string name = GetName(currentObjBaseAddress);

						if (false == reactionCache.ContainsKey(guid))
							reactionCache.Add(guid, LuaHelper.GetReactionType(guid));

						ReactionType reaction = reactionCache[guid];
						gameObjects.Add(new Unit(currentObjBaseAddress, guid, type, name, reaction));
						break;
					case GOType.Player:
						break;
					case GOType.GameObject:
						gameObjects.Add(new GameObject(currentObjBaseAddress ,guid, type));
						break;
					case GOType.DynamicObject:
						break;
					case GOType.Corpse:
						break;
					default:
						break;
				}

				currObjPtr = currentObjBaseAddress + (uint)ObjectManagerEnum.NextObject;
				currentObjBaseAddress = Memory.Read<uint>(currObjPtr);

			} ;
		}

		private static string GetName(uint currentObjBase)
		{
			uint ptrToNameAddress = Memory.Read<uint>(currentObjBase + (uint)Globals.UnitName1);
			if (ptrToNameAddress != 0)
			{
				uint nameAddress = Memory.Read<uint>(ptrToNameAddress + (uint)Globals.UnitName2);
				return Memory.ReadString((uint)nameAddress);
			}

			return "";
		}
	}
}
