using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WowBot
{
	class ObjectManager
	{
		static int objectManager;

		static ObjectManager()
		{
			int CurMgr = Memory.ReadInt((int)ObjectManagerEnum.CurMgrPointer); ;
			objectManager = Memory.ReadInt(CurMgr + (int)ObjectManagerEnum.CurMgrOffset);
		}

		public static int GetPlayerAdress()
		{
			string playerGUID = Memory.ReadHexAsString(objectManager + (int)ObjectManagerEnum.LocalGUID);

			int currObjPtr = objectManager + (int)ObjectManagerEnum.FirstObject;
			int currObj = Memory.ReadInt(currObjPtr);

			while (currObj != 0)
			{
				currObj = Memory.ReadInt(currObjPtr);
				String GUID = Memory.ReadHexAsString(currObj + 0x30);

				if (GUID == playerGUID)
					return currObj;

				currObjPtr = currObj + (int)ObjectManagerEnum.NextObject;
			}

			return 0;
		}

		//public void writeToFile()
		//{
		//	int currObjPtr = objectManager + FIRST_OBJECT;
		//	int currObj = MemoryHandler.readInt(currObjPtr);
		//
		//	System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\például Lilian\Desktop\akarmi.txt");
		//
		//	int X = 0x97C1;
		//	int Y = 0x97DA;
		//
		//	while (currObj != 0)
		//	{
		//		currObj = MemoryHandler.readInt(currObjPtr);
		//		if (MemoryHandler.readInt(currObj + 0x14) == 5)
		//		{
		//			Position p = Zone.getRelativePosition(MemoryHandler.readFloat(currObj + X), MemoryHandler.readFloat(currObj + Y));
		//			file.WriteLine(p.x + ", " + p.y);
		//		}
		//		/*for (int i = 0; i < 100000; ++i) {
		//			file.WriteLine(MemoryReader.readFloat(0x0C632968 + 0x1 * i));
		//		}*/
		//
		//		currObjPtr = currObj + NEXT_OBJECT;
		//	}
		//	file.Close();
		//}
	}
}
