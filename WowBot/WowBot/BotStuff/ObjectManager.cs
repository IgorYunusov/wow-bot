using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WowBot.Utils.WowObject;
using static WowBot.Utils.WowObject.Unit;

namespace WowBot
{
	class ObjectManager
	{
		static uint objectManager;
		static List<GameObject> gameObjects = new List<GameObject>();

		static ObjectManager()
		{
			uint CurMgr = Memory.Read<uint>((uint)ObjectManagerEnum.CurMgrPointer);
			objectManager = Memory.Read<uint>(CurMgr + (uint)ObjectManagerEnum.CurMgrOffset);
		}

		public static List<T> GetObjects<T>()
		{
			CollectAllObjects();

			return gameObjects.OfType<T>().ToList();
		}

		//public static int GetPlayerAdress()
		//{
		//	string playerGUID = Memory.ReadHexAsString(objectManager + (int)ObjectManagerEnum.LocalGUID);
		//
		//	int currObjPtr = objectManager + (int)ObjectManagerEnum.FirstObject;
		//	int currObj = Memory.ReadInt(currObjPtr);
		//
		//	while (currObj != 0)
		//	{
		//		currObj = Memory.ReadInt(currObjPtr);
		//		String GUID = Memory.ReadHexAsString(currObj + 0x30);
		//
		//		if (GUID == playerGUID)
		//			return currObj;
		//
		//		currObjPtr = currObj + (int)ObjectManagerEnum.NextObject;
		//	}
		//
		//	return 0;
		//}
		

		public static void CollectAllObjects()
		{
			gameObjects.Clear();

			uint currObjPtr = objectManager + (uint)ObjectManagerEnum.FirstObject;
			uint currentObjBase = Memory.Read<uint>(currObjPtr);

			// don't know why but currentObjBase % 2 operation makes it work
			while (currentObjBase != uint.MinValue && currentObjBase%2 == uint.MinValue) 
			{
				ulong guid = Memory.Read<ulong>(currentObjBase + (int)ObjectOffsets.Guid);
				GOType type = (GOType) Memory.Read<short>(currentObjBase + (uint)ObjectOffsets.Type);

				if (type == GOType.Unit)
				{
					string name = GetName(currentObjBase);
					//ReactionType reaction = LuaHelper.GetReactionType(guid.ToString("X"));
					ReactionType reaction = ReactionType.Friendly;
					gameObjects.Add(new Unit(guid, type, name, reaction));
					Console.WriteLine(name);
				}
				else
				{
					gameObjects.Add(new GameObject(guid, type));
				}

				currObjPtr = currentObjBase + (uint)ObjectManagerEnum.NextObject;
				currentObjBase = Memory.Read<uint>(currObjPtr);

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
