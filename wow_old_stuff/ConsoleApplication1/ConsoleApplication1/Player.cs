using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace MiningBot {
    class Player {
        private static int player = ObjectManager.sharedOM().getPlayerAdress();
        
        private static int MAX_HP = player + 6616;
        private static int CURR_HP = player + 4016;
        private static int MAX_MANA = MAX_HP + 0x4;
        private static int CURR_MANA = CURR_HP + 0x4;
        private static int CURR_RAGE = CURR_MANA + 0x4;

        private static int X = player + 0x79C;
        private static int Y = player + 0x798;
        private static int Z = player + 0x7A0;

        private Stopwatch stopwatch;

        private static int CTM_Base = 0x00CA11D8;
        private static int CTM_X = CTM_Base + 0x90;
        private static int CTM_Y = CTM_Base + 0x8C;
        private static int CTM_Z = CTM_Base + 0x94;
        private static int CTM_Distance = CTM_Base + 0xC;
        private static int CTM_Action = CTM_Base + 0x1C;
        private static int CTM_GUID = CTM_Base + 0x20;
        private static int IN_COMBAT = player + 0x4A290E0;

        private static int UNIT_FLAGS = player + 0x3B;

        private static int MOUSE_OVER_GUID = 0x00BD07A0;

        private int mountNum;
        private Position position;
        private Position3D position3D;

        public Player() {
            position = new Position(0, 0);
            position3D = new Position3D(0, 0, 0);
            stopwatch = new Stopwatch();

            /*while (true) {
                System.Console.Clear();
                System.Console.WriteLine(MemoryHandler.readHexAsInt64(player + 0x30));
                System.Console.WriteLine(getCurrRage());
                /*for (int i = 3000; i < 6000; ++i) {
                    if (MemoryHandler.readInt(player + i) == 1) {
                        System.Console.WriteLine(i + ": " + MemoryHandler.readInt(player + i));
                    }
                }

                Thread.Sleep(200);
            }*/
        }

        public int getCurrHP(){
            return MemoryHandler.readInt(CURR_HP);
        }

        public int getMaxHP() {
            return MemoryHandler.readInt(MAX_HP);
        }

        public int getCurrMana() {
            return MemoryHandler.readInt(CURR_MANA);
        }

        public int getMaxMana() {
            return MemoryHandler.readInt(MAX_MANA);
        }

        public int getCurrRage() {
            return MemoryHandler.readInt(CURR_RAGE);
        }

        public float getX() {
            return MemoryHandler.readFloat(X);
        }

        public float getY() {
            return MemoryHandler.readFloat(Y);
        }

        public float getZ() {
            return MemoryHandler.readFloat(Z);
        }



        public Position getPosition() {
            position.x = MemoryHandler.readFloat(X);
            position.y = MemoryHandler.readFloat(Y);
            return position;
        }

        public Position3D getPosition3D() {
            position3D.x = MemoryHandler.readFloat(X);
            position3D.y = MemoryHandler.readFloat(Y);
            position3D.z = MemoryHandler.readFloat(Z);
            return position3D;
        }

        public void clickToLoot(Position3D position, Int64 guid) {
            //az értékek beírása a memoriába
            MemoryHandler.writeInt64(MOUSE_OVER_GUID, guid);
            System.Console.WriteLine(guid);
            ChatWriter.hitKey(0x042);

            //MemoryHandler.writeInt(CTM_Action, 7);

            //amíg az action 7, azaz mozgásban van, addig várok
            while (MemoryHandler.readInt(CTM_Action) == 7) {
                Thread.Sleep(200);
            }
            Thread.Sleep(400);
        }

        

        public void clickToMove(float x, float y, float z = 0.0f) {
            //ha nem adok meg z értéket, akkor marad a magasság ugyanaz
            if (z == 0.0f) { z = MemoryHandler.readFloat(Z - 10); }

            //az értékek beírása a memoriába
            MemoryHandler.writeFloat(CTM_X, x);
            MemoryHandler.writeFloat(CTM_Y, y);
            MemoryHandler.writeFloat(CTM_Z, z);
            MemoryHandler.writeInt(CTM_Action, 4);
            //MemoryHandler.writeFloat(CTM_Distance, 0.5f);

            //amíg az action 4, azaz mozgásban van, addig várok
            while (MemoryHandler.readInt(CTM_Action) == 4) {
                Thread.Sleep(200);
            }
            Thread.Sleep(400);
        }

        public void clickToMove(Position3D position) {
            //az értékek beírása a memoriába
            MemoryHandler.writeFloat(CTM_X, position.x);
            MemoryHandler.writeFloat(CTM_Y, position.y);
            MemoryHandler.writeFloat(CTM_Z, position.z);
            MemoryHandler.writeInt(CTM_Action, 4);
            //MemoryHandler.writeFloat(CTM_Distance, 0.5f);

            //amíg az action 4, azaz mozgásban van, addig várok
            while (MemoryHandler.readInt(CTM_Action) == 4) {
                Thread.Sleep(200);
            }
            Thread.Sleep(400);
        }

        public void flyTo(Position position) {
            //míg az x,y távolság nagyobb mint 1 várok
            stopwatch.Reset();
            stopwatch.Start();
            while (
                (Math.Abs(MemoryHandler.readFloat(X) - position.x) > 10.0f || 
                Math.Abs(MemoryHandler.readFloat(Y) - position.y) > 10.0f)
                ) {
                //ha sokat baszakodik, akkor ujra felmountolok
                if (stopwatch.ElapsedMilliseconds > 60000) {
                    System.Console.WriteLine("nem nagyon jutok a célhoz, ujramountolok");
                    MemoryHandler.writeInt(CTM_Action, 11);
                    takeOff();
                    stopwatch.Reset();
                    stopwatch.Start();
                }

                MemoryHandler.writeFloat(CTM_X, position.x);
                MemoryHandler.writeFloat(CTM_Y, position.y);
                MemoryHandler.writeFloat(CTM_Z, MemoryHandler.readFloat(Z));
                MemoryHandler.writeFloat(CTM_Distance, 0.5f);
                MemoryHandler.writeInt(CTM_Action, 4);
                Thread.Sleep(200);
                //System.Console.WriteLine(MemoryHandler.readFloat(CTM_X) + ", " + MemoryHandler.readFloat(CTM_Y) + ", " + MemoryHandler.readFloat(CTM_Z));
                //System.Console.WriteLine(position.x + ", " + position.y);
                //System.Console.WriteLine(MemoryHandler.readFloat(X) + ", " + MemoryHandler.readFloat(Y));
            }
            MemoryHandler.writeInt(CTM_Action, 11);
            Thread.Sleep(400);
        }

        public void takeOff() {
            //ChatWriter.send(MemoryHandler.process.MainWindowHandle, "/run if IsMounted() == nil then CallCompanion('MOUNT', " + mountNum + ") end");//"/click MultiBarRightButton1"

            if (getCurrRage() > 0) {
                System.Console.WriteLine("megtámadtak, bladestorm");
                ChatWriter.hitKey(ChatWriter.THREE);
                ChatWriter.hitKey(ChatWriter.THREE);
                ChatWriter.hitKey(ChatWriter.TWO);
                Thread.Sleep(8000);
                ChatWriter.hitKey(ChatWriter.FOUR);
                Thread.Sleep(2000);
            }

            ChatWriter.hitKey(ChatWriter.ONE);
            Thread.Sleep(2500);

            MemoryHandler.writeFloat(CTM_X, MemoryHandler.readFloat(X));
            MemoryHandler.writeFloat(CTM_Y, MemoryHandler.readFloat(Y));
            MemoryHandler.writeFloat(CTM_Z, MemoryHandler.readFloat(Z) + 800.0f);
            MemoryHandler.writeInt(CTM_Action, 4);

            //néha nem repül fel egyből, hanem megáll 1 méterre a föld felett, ezért ujra kell kattintani
            Thread.Sleep(1000);
            MemoryHandler.writeInt(CTM_Action, 4);

            Thread.Sleep(10000);
        }

        public void ressurrect() {
            ChatWriter.send(MemoryHandler.process.MainWindowHandle, "/script RepopMe()");
            Thread.Sleep(2000);
            //MemoryHandler.writeFloat(CTM_Distance, 0.5f);
            MemoryHandler.writeInt64(MOUSE_OVER_GUID, ObjectManager.sharedOM().getSHGUID());
            Thread.Sleep(500);
            ChatWriter.hitKey(0x042);
            Thread.Sleep(6000);
            ChatWriter.send(MemoryHandler.process.MainWindowHandle, "/run SelectGossipOption(1) AcceptXPLoss()");
            Thread.Sleep(6000);
        }

        public void eat() {
            ChatWriter.send(MemoryHandler.process.MainWindowHandle, "/click MultiBarRightButton2");
            Thread.Sleep(25000);
        }

        public void setMountNum(int mountNum) {
            this.mountNum = mountNum;
        }

        public bool isMoving() {
            return MemoryHandler.readInt(CTM_Action) == 4 ? true : false;
        }

        public void dismount() {
            ChatWriter.send(MemoryHandler.process.MainWindowHandle, "/run Dismount();");
        }
    }
}
