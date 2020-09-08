using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WowBot.Utils;
using WowBot.Utils.WowObject;
using static WowBot.Utils.WowObject.Unit;

namespace WowBot
{
	static class ObjectManager
	{
		static uint objectManager;
		static List<GameObject> gameObjects = new List<GameObject>();
		static DateTime lastScanTime;
		const int CollectAfterMillis = 500;

		static Dictionary<ulong, ReactionType> reactionCache = new Dictionary<ulong, ReactionType>();

		static ObjectManager()
		{
			uint CurMgr = Memory.Read<uint>((uint)ObjectManagerEnum.CurMgrPointer);
			objectManager = Memory.Read<uint>(CurMgr + (uint)ObjectManagerEnum.CurMgrOffset);
		}

		public static void Update()
		{
			if ((DateTime.Now - lastScanTime).TotalMilliseconds > CollectAfterMillis)
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

		public static uint GetPlayerBaseAddress()
		{
			ulong playerGUID = MemoryHandler.Instance.ReadUInt64((uint)Globals.LocalGUID);
			return gameObjects.FirstOrDefault(o => o.Guid == playerGUID).BaseAddress;
		}

		public static void CollectAllObjects()
		{
			List<GameObject> gameObjectsToRemove = new List<GameObject>(gameObjects);

			uint currObjPtr = objectManager + (uint)ObjectManagerEnum.FirstObject;
			uint currentObjBaseAddress = Memory.Read<uint>(currObjPtr);

			// don't know why but currentObjBase % 2 operation makes it work
			while (currentObjBaseAddress != uint.MinValue && currentObjBaseAddress%2 == uint.MinValue) 
			{
				GameObject go = AddToGameObjectsIfNotContainsGuid(currentObjBaseAddress);

				gameObjectsToRemove.Remove(go);

				currObjPtr = currentObjBaseAddress + (uint)ObjectManagerEnum.NextObject;
				currentObjBaseAddress = Memory.Read<uint>(currObjPtr);
			};

			if(gameObjectsToRemove.Count > 0)
				gameObjects.RemoveAll(go => gameObjectsToRemove.Contains(go));
		}

		private static GameObject AddToGameObjectsIfNotContainsGuid(uint currentObjBaseAddress)
		{
			ulong guid = Memory.Read<ulong>(currentObjBaseAddress + (int)ObjectOffsets.Guid);
			bool notContainsGuid = null == gameObjects.FirstOrDefault(go => go.Guid == guid);

			if (notContainsGuid)
			{
				GameObject go = CreateGameObject(currentObjBaseAddress);
				gameObjects.Add(go);
			}

			return gameObjects.First(go => go.Guid == guid);
		}

		private static GameObject CreateGameObject(uint currentObjBaseAddress)
		{
			ulong guid = Memory.Read<ulong>(currentObjBaseAddress + (int)ObjectOffsets.Guid);
			GOType type = (GOType)Memory.Read<short>(currentObjBaseAddress + (uint)ObjectOffsets.Type);

			GameObject go = null;

			switch (type)
			{
				case GOType.None:
					break;
				case GOType.Item:
					//do this later
					go = new GameObject(currentObjBaseAddress, guid, type);
					break;
				case GOType.Container:
					//do this later
					go = new GameObject(currentObjBaseAddress, guid, type);
					break;
				case GOType.Unit:
					string name = GetName(currentObjBaseAddress);

					if (false == reactionCache.ContainsKey(guid))
						reactionCache.Add(guid, LuaHelper.GetReactionType(guid));

					ReactionType reaction = reactionCache[guid];
					go = new Unit(currentObjBaseAddress, guid, type, name, reaction);
					break;
				case GOType.Player:
					//do this later
					go = new GameObject(currentObjBaseAddress, guid, type);
					break;
				case GOType.GameObject:
					go = new GameObject(currentObjBaseAddress, guid, type);
					break;
				case GOType.DynamicObject:
					//do this later
					go = new GameObject(currentObjBaseAddress, guid, type);
					break;
				case GOType.Corpse:
					//do this later
					go = new GameObject(currentObjBaseAddress, guid, type);
					break;
				default:
					break;
			}

			return go;
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
