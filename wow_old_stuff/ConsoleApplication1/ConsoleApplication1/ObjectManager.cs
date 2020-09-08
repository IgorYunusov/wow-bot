using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MiningBot {
    class ObjectManager {
        private static ObjectManager sharedManager;

        public static int CUR_MGR_PTR = 0x00C79CE0;                 // 3.3.5a 12340
        public static int CUR_MGR_OFFSET = 0x2ED0;
        public static int NEXT_OBJECT = 0x3C;
        public static int FIRST_OBJECT = 0xAC;
        public static int GUID_OFFS = 0x30;

        public static int CREATED_BY = 560;
        public static int TYPE_ID = 0x224;
        public static int BOBBER = 0xBC;

        private static Int64[] SH_GUIDS_ARRAY = { -1067353002776230427 , -1067353002776230191, -1067353002776230193 };
        //private static string[] SH_GUIDS_ARRAY = { "F13000195B9385E5 ", "F13000195B9386D1", "F13000195B9386CF" };
        public static List<Int64> SH_GUIDS = SH_GUIDS_ARRAY.ToList();

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
            int LocalGUID = 0xC0;
            String playerGUID = MemoryHandler.readHexAsString(objectManager + LocalGUID);

            System.Console.WriteLine(MemoryHandler.readHexAsInt64(objectManager + LocalGUID));

            int currObjPtr = objectManager + FIRST_OBJECT;
            int currObj = MemoryHandler.readInt(currObjPtr);

            while(currObj != 0){
                currObj = MemoryHandler.readInt(currObjPtr);
                String GUID = MemoryHandler.readHexAsString(currObj + GUID_OFFS);

                if (GUID == playerGUID) {
                    System.Console.WriteLine(currObj);
                    return currObj;
                }
                currObjPtr = currObj + NEXT_OBJECT;
            }
            return 0;
        }

        public bool anyOneTargetingMe() {
            int LocalGUID = 0xC0;
            Int64 playerGUID = MemoryHandler.readHexAsInt64(objectManager + LocalGUID);

            int currObjPtr = objectManager + FIRST_OBJECT;
            int currObj = MemoryHandler.readInt(currObjPtr);
            
            while (currObj != 0) {
                currObj = MemoryHandler.readInt(currObjPtr);
                Int64 targetGUID = MemoryHandler.readHexAsInt64(MemoryHandler.readInt( currObj) + 0x12);

                if (1 == 1) {
                    System.Console.WriteLine( MemoryHandler.readInt(currObj + 0x3B) & 0x0080000);
                }
                currObjPtr = currObj + NEXT_OBJECT;
            }

            return false;
        }

        public Int64 getSHGUID() {
            int currObjPtr = objectManager + FIRST_OBJECT;
            int currObj = MemoryHandler.readInt(currObjPtr);

            while (currObj != 0) {
                currObj = MemoryHandler.readInt(currObjPtr);
                if (SH_GUIDS.Contains(MemoryHandler.readHexAsInt64(currObj + GUID_OFFS))) {
                    return MemoryHandler.readHexAsInt64(currObj + GUID_OFFS);
                }                

                currObjPtr = currObj + NEXT_OBJECT;
            }

            return 0;
        }

        public MineralVein[] getMineralVeinsAround() {
            int currObjPtr = objectManager + FIRST_OBJECT;
            int currObj = MemoryHandler.readInt(currObjPtr);

            List<MineralVein> mineralsAround = new List<MineralVein>();

            while (currObj != 0) {
                currObj = MemoryHandler.readInt(currObjPtr);

                if (MineralVein.mineralIDs.Contains(MemoryHandler.readInt(currObj + MineralVein.TYPE_ID))) {
                    Position3D position = new Position3D( 
                        MemoryHandler.readFloat(currObj + MineralVein.X),
                        MemoryHandler.readFloat(currObj + MineralVein.Y),
                        MemoryHandler.readFloat(currObj + MineralVein.Z));
                    if(!Bot.PROHIBITED_POSITIONS.Contains(position)){
                        mineralsAround.Add(new MineralVein(position, MemoryHandler.readInt64(currObj + 0x30)));
                    }
                }
                currObjPtr = currObj + NEXT_OBJECT;
            }

            return mineralsAround.ToArray();
        }

        public void writeToFile() {
            while (true) {
                Console.Clear();

            int currObjPtr = objectManager + FIRST_OBJECT;
            int currObj = MemoryHandler.readInt(currObjPtr);

            //System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\például Lilian\Desktop\akarmi2.txt");

            int X = 236;
            int Y = 500;
            int Z = 480;

           
                while (currObj != 0) {
                    currObj = MemoryHandler.readInt(currObjPtr);
                    Position p = Zone.getRelativePosition(MemoryHandler.readFloat(currObj + X), MemoryHandler.readFloat(currObj + Y));
                    //if (/*p.x * 100 < 100 && p.y * 100 < 100 &&*/ MemoryHandler.readInt(currObj + 548) == 181557) {
                    //file.WriteLine(MemoryHandler.readHexAsInt64(currObj + 0x20));
                    /*if (MemoryHandler.readHexAsInt64(currObj + 48) == 75435293781275187) {
                        System.Console.WriteLine(MemoryHandler.readFloat(currObj + X) + ", " + MemoryHandler.readFloat(currObj + Y));
                    }*/
                    //}
                    /*
                        if (MemoryHandler.readInt(currObj + 0x14) == 6500) {
                            Position p = Zone.getRelativePosition(MemoryHandler.readFloat(currObj + X), MemoryHandler.readFloat(currObj + Y));
                            file.WriteLine(p.x + ", " + p.y);
                        }*/
                    //file.WriteLine(currObj - 0x0AFA6DE0);

                    //file.WriteLine("-------------------------------");
                    //if (MemoryHandler.readInt(currObj + 548) == 181557) {
                    /* for (int i = 0; i < 1000; ++i) {
                         file.WriteLine(i + ":" + MemoryHandler.readHexAsInt64(currObj + 0x1 * i));
                     }
                     file.WriteLine("-------------------------------");*/
                    //}

                    /**/
                    /*while (true) {
                        System.Console.Clear();
                        System.Console.WriteLine(MemoryHandler.readString(0x0369A30C, 20));
                        Thread.Sleep(200);
                    }*/

                    //System.Console.WriteLine(MemoryHandler.readString(currObj + 0x2, 10));

                    /*if (MemoryHandler.readHexAsInt64(0x00BD07A0) == MemoryHandler.readHexAsInt64(currObj + GUID_OFFS) &&
                        MemoryHandler.readHexAsInt64(0x00BD07A0) != 0
                        ) {*/
                            if (MemoryHandler.readHexAsInt64(currObj + 560) == 75435293781275187) {
                                if (MemoryHandler.readInt(currObj + 0xBC) != 0) {
                                    System.Console.WriteLine(MemoryHandler.readInt16(currObj + 0xBC));
                                    //Thread.Sleep(20000);
                                }
                            }
                        /*for (int i = 0; i < 10000; ++i) {
                            if (MemoryHandler.readHexAsInt64(currObj + 0x1 * i) == 75435293781275187) {
                                System.Console.WriteLine(i);
                            }
                            //file.WriteLine(MemoryHandler.readHexAsInt64(currObj + 0x1 * i));
                         }*/
                    //}

                    currObjPtr = currObj + NEXT_OBJECT;
                }
                //System.Console.WriteLine(MemoryHandler.readHexAsString(0x00BD07A0));
                //file.Close();
                Thread.Sleep(10);            
            } 
        }
    }
}
