using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1 {
    class ObjectManager {
        private static ObjectManager sharedManager;

        public static int CUR_MGR_PTR = 0x00C79CE0;                 // 3.3.5a 12340
        public static int CUR_MGR_OFFSET = 0x2ED0;
        public static int NEXT_OBJECT = 0x3C;
        public static int FIRST_OBJECT = 0xAC;

        private int objectManager;

        private ObjectManager() {
            int CurMgr = MemoryHandler.readInt(CUR_MGR_PTR); ;
            objectManager = MemoryHandler.readInt(CurMgr + CUR_MGR_OFFSET);
        }

        public static ObjectManager sharedOM() {
            if (sharedManager == null) {
                sharedManager = new ObjectManager();
            }
            return sharedManager;
        }

        public int getPlayerAdress() {
            writeToFile();
            int LocalGUID = 0xC0;
            String playerGUID = MemoryHandler.readHexAsString(objectManager + LocalGUID);

            int currObjPtr = objectManager + FIRST_OBJECT;
            int currObj = MemoryHandler.readInt(currObjPtr);

            while(currObj != 0){
                currObj = MemoryHandler.readInt(currObjPtr);
                String GUID = MemoryHandler.readHexAsString(currObj + 0x30);

                if (GUID == playerGUID) {
                    return currObj;
                }
                currObjPtr = currObj + NEXT_OBJECT;
            }
            return 0;
        }

        public void writeToFile() {
            int currObjPtr = objectManager + FIRST_OBJECT;
            int currObj = MemoryHandler.readInt(currObjPtr);

            System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\például Lilian\Desktop\akarmi.txt");

            int X = 0x97C1;
            int Y = 0x97DA;

            while (currObj != 0) {
                currObj = MemoryHandler.readInt(currObjPtr);
                    if (MemoryHandler.readInt(currObj + 0x14) == 5) {
                        Position p = Zone.getRelativePosition(MemoryHandler.readFloat(currObj + X), MemoryHandler.readFloat(currObj + Y));
                        file.WriteLine(p.x + ", " + p.y);
                    }
                    /*for (int i = 0; i < 100000; ++i) {
                        file.WriteLine(MemoryReader.readFloat(0x0C632968 + 0x1 * i));
                    }*/
                
                currObjPtr = currObj + NEXT_OBJECT;
            }
            file.Close();
        }
    }
}
