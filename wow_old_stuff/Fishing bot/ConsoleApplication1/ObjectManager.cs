using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace FishingBot {
    class ObjectManager {
        private static ObjectManager sharedManager;

        public static int CUR_MGR_PTR = 0x00C79CE0;                 // 3.3.5a 12340
        public static int CUR_MGR_OFFSET = 0x2ED0;
        public static int NEXT_OBJECT = 0x3C;
        public static int FIRST_OBJECT = 0xAC;
        public static int GUID_OFFS = 0x30;
        public static int MOUSE_OVER_GUID = 0x00BD07A0;

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

        public bool objExists(Int64 GUID) {
            int currObjPtr = objectManager + FIRST_OBJECT;
            int currObj = MemoryHandler.readInt(currObjPtr);

            while (currObj != 0) {
                currObj = MemoryHandler.readInt(currObjPtr);

                if (GUID == MemoryHandler.readHexAsInt64(currObj + GUID_OFFS)) {
                    return true;
                }
                currObjPtr = currObj + NEXT_OBJECT;
            }
            return false;
        }

        public int getPlayerAddress() {
            int LocalGUID = 0xC0;
            String playerGUID = MemoryHandler.readHexAsString(objectManager + LocalGUID);

            Player.PLAYER_GUID = MemoryHandler.readHexAsInt64(objectManager + LocalGUID);

            int currObjPtr = objectManager + FIRST_OBJECT;
            int currObj = MemoryHandler.readInt(currObjPtr);

            while(currObj != 0){
                currObj = MemoryHandler.readInt(currObjPtr);
                Int64 GUID = MemoryHandler.readHexAsInt64(currObj + GUID_OFFS);

                if (GUID == Player.PLAYER_GUID) {
                    return currObj;
                }
                currObjPtr = currObj + NEXT_OBJECT;
            }
            return 0;
        }

        public Int64 getSHGUID() {
            int currObjPtr = objectManager + FIRST_OBJECT;
            int currObj = MemoryHandler.readInt(currObjPtr);

            while (currObj != 0) {
                currObj = MemoryHandler.readInt(currObjPtr);
                if (SpiritHealer.SH_GUIDS.Contains(MemoryHandler.readHexAsInt64(currObj + GUID_OFFS))) {
                    return MemoryHandler.readHexAsInt64(currObj + GUID_OFFS);
                }                

                currObjPtr = currObj + NEXT_OBJECT;
            }

            return 0;
        }

        public int getBobberBase() {
            int currObjPtr = objectManager + FIRST_OBJECT;
            int currObj = MemoryHandler.readInt(currObjPtr);
            
            while (currObj != 0) {
                currObj = MemoryHandler.readInt(currObjPtr);

                if (MemoryHandler.readHexAsInt64(currObj + 560) == Player.PLAYER_GUID) {
                    return currObj;
                }

                currObjPtr = currObj + NEXT_OBJECT;
            }
            return -1;
        }

        public void test() {
            while (true) {
                Console.Clear();

                int currObjPtr = objectManager + FIRST_OBJECT;
                int currObj = MemoryHandler.readInt(currObjPtr);

                //System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\például Lilian\Desktop\akarmi2.txt");

                while (currObj != 0) {
                    currObj = MemoryHandler.readInt(currObjPtr);

                    /*if (
                        MemoryHandler.readHexAsInt64(Offset.MOUSE_OVER_GUID) != 0 &&
                        MemoryHandler.readHexAsInt64(Offset.MOUSE_OVER_GUID) == MemoryHandler.readHexAsInt64(currObj + GUID_OFFS)
                      ) {
                          System.Console.WriteLine(MemoryHandler.readInt(currObj + Offset.TYPE_ID));
                    }*/

                    if (MemoryHandler.readHexAsInt64(currObj + 560) == 75435293781275187) {
                        if (MemoryHandler.readInt(currObj + 0xBC) != 0) {
                            System.Console.WriteLine(MemoryHandler.readInt16(currObj + 0xBC));
                            if (MemoryHandler.readInt16(currObj + 0xBC) == 1) {
                                MemoryHandler.writeInt64(Offset.MOUSE_OVER_GUID, MemoryHandler.readHexAsInt64(currObj + GUID_OFFS));
                                ChatWriter.hitKey(0x42);
                            }
                        }
                    }

                    currObjPtr = currObj + NEXT_OBJECT;
                }
                Thread.Sleep(100);
            }
        }
    }
}
