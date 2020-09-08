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

        WowObject CurrentObject = new WowObject();
        WowObject TempObject = new WowObject();

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

        public ulong getEnemyNearPos(Position3D pos) {
            WowObject enemy = new WowObject();

            int currObjPtr = objectManager + FIRST_OBJECT;
            int currObj = MemoryHandler.readInt(currObjPtr);

            int LocalGUID = 0xC0;
            Int64 playerGUID = MemoryHandler.readHexAsInt64(objectManager + LocalGUID);

            while (currObj != 0) {
                currObj = MemoryHandler.readInt(currObjPtr);

                enemy.Guid = MemoryHandler.readUInt64((int)(currObj + ObjectOffsets.Guid));
                enemy.Type = (short)(MemoryHandler.readUInt32((int)(currObj + ObjectOffsets.Type)));
                enemy.XPos = MemoryHandler.readFloat((int)(currObj + ObjectOffsets.Pos_X));
                enemy.YPos = MemoryHandler.readFloat((int)(currObj + ObjectOffsets.Pos_Y));
                enemy.ZPos = MemoryHandler.readFloat((int)(currObj + ObjectOffsets.Pos_Z));
                enemy.CurrentHealth = MemoryHandler.readUint((int)(currObj + ObjectOffsets.Health));

                if (enemy.Type == 3 && enemy.CurrentHealth != 0) {
                    Position enemyPos = new Position(enemy.XPos, enemy.YPos);
                    if (5.0 > enemyPos.distance(pos.to2DPosition())) {
                        return enemy.Guid;
                    }
                }

                currObjPtr = currObj + NEXT_OBJECT;
            }

            return 0;
        }

        public int getPlayerAdress() {
            int LocalGUID = 0xC0;
            String playerGUID = MemoryHandler.readHexAsString(objectManager + LocalGUID);

            //System.Console.WriteLine(MemoryHandler.readHexAsInt64(objectManager + LocalGUID));

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

        public ulong getSpiritHealer() {
            int currObjPtr = objectManager + FIRST_OBJECT;
            int currObj = MemoryHandler.readInt(currObjPtr);

            while (currObj != 0) {
                currObj = MemoryHandler.readInt(currObjPtr);

                WowObject sh = new WowObject();

                sh.Guid = MemoryHandler.readUInt64((int)(currObj + ObjectOffsets.Guid));
                sh.Type = (short)(MemoryHandler.readUInt32((int)(currObj + ObjectOffsets.Type)));

                if (sh.Type == 3) {
                    string name = "Spirit Healer";

                    sh.Name = getMobNameFrmBase(currObj);
                    if(sh.Name == name){
                        return sh.Guid;
                    }
                }

                currObjPtr = currObj + NEXT_OBJECT;
            }
            return 0;
        }

        public void summonedByMe() {
            int currObjPtr = objectManager + FIRST_OBJECT;
            int currObj = MemoryHandler.readInt(currObjPtr);

            int LocalGUID = 0xC0;

            while (currObj != 0) {
                currObj = MemoryHandler.readInt(currObjPtr);

                if (MemoryHandler.readHexAsInt64(currObj + 560) == MemoryHandler.readHexAsInt64(objectManager + LocalGUID)) {
                    System.Console.WriteLine("ja");
                }

                currObjPtr = currObj + NEXT_OBJECT;
            }
        }

        public int getHealthByGUID(ulong GUID) {
            int currObjPtr = objectManager + FIRST_OBJECT;
            int currObj = MemoryHandler.readInt(currObjPtr);

            while (currObj != 0) {
                currObj = MemoryHandler.readInt(currObjPtr);

                if (MemoryHandler.readUInt64((int)(currObj + ObjectOffsets.Guid)) == (GUID)) {
                    return MemoryHandler.readInt((int)(currObj + ObjectOffsets.Health));
                }

                currObjPtr = currObj + NEXT_OBJECT;
            }

            return -1;
        }

        public bool isPlayerNear(Position3D playerPos) {
            int currObjPtr = objectManager + FIRST_OBJECT;
            int currObj = MemoryHandler.readInt(currObjPtr);

            WowObject target = new WowObject();

            while (currObj != 0) {
                currObj = MemoryHandler.readInt(currObjPtr);

                target.Type = (short)(MemoryHandler.readUInt32((int)(currObj + ObjectOffsets.Type)));

                if (target.Type == 4) {
                    target.XPos = MemoryHandler.readFloat((int)(currObj + ObjectOffsets.Pos_X));
                    target.YPos = MemoryHandler.readFloat((int)(currObj + ObjectOffsets.Pos_Y));
                    target.ZPos = MemoryHandler.readFloat((int)(currObj + ObjectOffsets.Pos_Z));
                    
                    float distance= playerPos.distance(new Position3D(target.XPos, target.YPos, target.ZPos));

                    if (160.0f > distance && 1.0f < distance) {
                        return true;
                    }
                }

                currObjPtr = currObj + NEXT_OBJECT;
            }

            return false;
        }

        public void printTargetData(Position3D playerPos) {
            while (true) {
                int currObjPtr = objectManager + FIRST_OBJECT;
                int currObj = MemoryHandler.readInt(currObjPtr);

                WowObject target = new WowObject();
                Position3D otherPos = new Position3D(0, 0, 0);

                while (currObj != 0) {
                    currObj = MemoryHandler.readInt(currObjPtr);


                    if (MemoryHandler.readUInt64((int)Globals.CURR_TARGET_GUID) == MemoryHandler.readUInt64((int)(currObj + ObjectOffsets.Guid))) {

                        target.Guid = MemoryHandler.readUInt64((int)(currObj + ObjectOffsets.Guid));
                        target.Name = getMobNameFrmBase(currObj);

                        target.XPos = MemoryHandler.readFloat((int)(currObj + ObjectOffsets.Pos_X));
                        target.YPos = MemoryHandler.readFloat((int)(currObj + ObjectOffsets.Pos_Y));
                        target.ZPos = MemoryHandler.readFloat((int)(currObj + ObjectOffsets.Pos_Z));

                        otherPos = new Position3D(target.XPos, target.YPos, target.ZPos);
                    }

                    currObjPtr = currObj + NEXT_OBJECT;
                }

                System.Console.Clear();
                System.Console.WriteLine(target.Name + ", " + target.Guid);
                System.Console.WriteLine(playerPos.distance(otherPos));
                Thread.Sleep(200);
            }
        }

        public List<WowObject> getEnemies() {
            List<WowObject> enemies = new List<WowObject>();
            int currObjPtr = objectManager + FIRST_OBJECT;
            int currObj = MemoryHandler.readInt(currObjPtr);

            int LocalGUID = 0xC0;
            Int64 playerGUID = MemoryHandler.readHexAsInt64(objectManager + LocalGUID);

            while (currObj != 0) {
                currObj = MemoryHandler.readInt(currObjPtr);

                WowObject enemy = new WowObject();

                enemy.Guid = MemoryHandler.readUInt64((int)(currObj + ObjectOffsets.Guid));
                enemy.Type = (short)(MemoryHandler.readUInt32((int)(currObj + ObjectOffsets.Type)));
                /*enemy.XPos = MemoryHandler.readFloat((int)(currObj + ObjectOffsets.Pos_X));
                enemy.YPos = MemoryHandler.readFloat((int)(currObj + ObjectOffsets.Pos_Y));
                enemy.ZPos = MemoryHandler.readFloat((int)(currObj + ObjectOffsets.Pos_Z));
                enemy.Rotation = MemoryHandler.readFloat((int)(currObj + ObjectOffsets.Rot));
                enemy.CurrentHealth = MemoryHandler.readUint((int)(currObj + ObjectOffsets.Health));*/

                if (enemy.Type == 3) {
                    enemy.Name = getMobNameFrmBase(currObj);
                    enemy.Target = MemoryHandler.readHexAsInt64((int)(currObj + ObjectOffsets.Target_GUID));

                    if (enemy.Target == playerGUID) {
                        HumanoidBot.metElite(enemy);
                        enemies.Add(enemy);
                    }
                }

                currObjPtr = currObj + NEXT_OBJECT;
            }

            return enemies;
        }


        public string getMobNameFrmBase(int objBase) {
            int nameAddress = MemoryHandler.readInt((MemoryHandler.readInt((objBase + 0x964)) + 0x05C));
            return MemoryHandler.readString(nameAddress);
        }

        /*public Int64 getSHGUID() {
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
        }*/

        
    }
}
