using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace FishingBot {
    class Player {
        private static int player = ObjectManager.sharedOM().getPlayerAddress();
        
        private static int MAX_HP = player + 6616;
        private static int CURR_HP = player + 4016;
        private static int MAX_MANA = MAX_HP + 0x4;
        private static int CURR_MANA = CURR_HP + 0x4;

        private static int X = player + 0x79C;
        private static int Y = player + 0x798;
        private static int Z = player + 0x7A0;

        public static Int64 PLAYER_GUID;

        private int mountNum;
        private Position position;
        private Position3D position3D;
        private Bobber bobber;
        private Stopwatch stopwatch = new Stopwatch();

        private int fishCounter = 0;

        public Player() {
            position = new Position(0, 0);
            position3D = new Position3D(0, 0, 0);
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

        public float getX() {
            return MemoryHandler.readFloat(X);
        }

        public float getY() {
            return MemoryHandler.readFloat(Y);
        }

        public float getZ() {
            return MemoryHandler.readFloat(Z);
        }

        public void destroyTillUncommon() {
            ChatWriter.send(MemoryHandler.process.MainWindowHandle, "/click MultiBarRightButton4");
        }

        public void fish() {
            if (fishCounter > 50) {
                destroyTillUncommon();
                fishCounter = 0;
            }

            ChatWriter.send(MemoryHandler.process.MainWindowHandle, "/click MultiBarRightButton3");
            Thread.Sleep(500);

            if (ObjectManager.sharedOM().getBobberBase() == -1) {
                System.Console.WriteLine("nincs bobber");
                return;
            }

            bobber = new Bobber(ObjectManager.sharedOM().getBobberBase());

            stopwatch.Reset();
            stopwatch.Start();
            while (stopwatch.ElapsedMilliseconds < 20000) {
                if (bobber.isBobbing()) {
                    System.Console.WriteLine("bite");
                    MemoryHandler.writeInt64(Offset.MOUSE_OVER_GUID, (bobber.GUID));
                    ChatWriter.hitKey(ChatWriter.B);
                    fishCounter++;
                    return;
                }
                Thread.Sleep(100);
            }
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
            MemoryHandler.writeInt64(Offset.MOUSE_OVER_GUID, guid);
            ChatWriter.hitKey(0x042);

            //MemoryHandler.writeInt(CTM_Action, 7);

            //amíg az action 4, azaz mozgásban van, addig várok
            while (MemoryHandler.readInt(Offset.CTM_Action) == 7) {
                Thread.Sleep(200);
            }
            Thread.Sleep(400);
        }

        public void clickToMove(float x, float y, float z = 0.0f) {
            //ha nem adok meg z értéket, akkor marad a magasság ugyanaz
            if (z == 0.0f) { z = MemoryHandler.readFloat(Z - 10); }

            //az értékek beírása a memoriába
            MemoryHandler.writeFloat(Offset.CTM_X, x);
            MemoryHandler.writeFloat(Offset.CTM_Y, y);
            MemoryHandler.writeFloat(Offset.CTM_Z, z);
            MemoryHandler.writeInt(Offset.CTM_Action, 4);

            //amíg az action 4, azaz mozgásban van, addig várok
            while (
                Math.Abs(MemoryHandler.readFloat(X) - position.x) > 10.0f &&
                Math.Abs(MemoryHandler.readFloat(Y) - position.y) > 10.0f
                ) {
                Thread.Sleep(200);
            }
            Thread.Sleep(400);
        }

        public void clickToMove(Position3D position) {
            //az értékek beírása a memoriába
            MemoryHandler.writeFloat(Offset.CTM_X, position.x);
            MemoryHandler.writeFloat(Offset.CTM_Y, position.y);
            MemoryHandler.writeFloat(Offset.CTM_Z, position.z);
            MemoryHandler.writeInt(Offset.CTM_Action, 4);
            //MemoryHandler.writeFloat(CTM_Distance, 0.5f);

            //amíg az action 4, azaz mozgásban van, addig várok
            while (
                    Math.Abs(MemoryHandler.readFloat(X) - position.x) > 10.0f &&
                    Math.Abs(MemoryHandler.readFloat(Y) - position.y) > 10.0f
                ) {
                Thread.Sleep(200);
            }
            Thread.Sleep(400);
        }

        public void clickToMove(Position position) {
            //az értékek beírása a memoriába
            MemoryHandler.writeFloat(Offset.CTM_X, position.x);
            MemoryHandler.writeFloat(Offset.CTM_Y, position.y);
            MemoryHandler.writeFloat(Offset.CTM_Z, MemoryHandler.readFloat(Z) - 10);
            MemoryHandler.writeInt(Offset.CTM_Action, 4);
            //MemoryHandler.writeFloat(CTM_Distance, 0.5f);

            //míg az x,y távolság nagyobb mint 1 várok
            while (
                Math.Abs(MemoryHandler.readFloat(X) - position.x) > 10.0f && 
                Math.Abs(MemoryHandler.readFloat(Y) - position.y) > 10.0f
                ) {
                Thread.Sleep(200);
                System.Console.WriteLine(MemoryHandler.readFloat(Offset.CTM_X) + ", " + MemoryHandler.readFloat(Offset.CTM_Y) + ", " + MemoryHandler.readFloat(Offset.CTM_Z));
                //System.Console.WriteLine(position.x + ", " + position.y);
                System.Console.WriteLine(MemoryHandler.readFloat(X) + ", " + MemoryHandler.readFloat(Y));
            }
            MemoryHandler.writeInt(Offset.CTM_Action, 11);
            Thread.Sleep(400);
        }

        public void takeOff() {
            ChatWriter.send(MemoryHandler.process.MainWindowHandle, "/run if IsMounted() == nil then CallCompanion('MOUNT', " + mountNum + ") end");//"/click MultiBarRightButton1"
            Thread.Sleep(2500);

            MemoryHandler.writeFloat(Offset.CTM_X, MemoryHandler.readFloat(X));
            MemoryHandler.writeFloat(Offset.CTM_Y, MemoryHandler.readFloat(Y));
            MemoryHandler.writeFloat(Offset.CTM_Z, MemoryHandler.readFloat(Z) + 800.0f);
            MemoryHandler.writeInt(Offset.CTM_Action, 4);

            //néha nem repül fel egyből, hanem megáll 1 méterre a föld felett, ezért ujra kell kattintani
            Thread.Sleep(1000);
            MemoryHandler.writeInt(Offset.CTM_Action, 4);

            Thread.Sleep(15000);
        }

        public void ressurrect() {
            ChatWriter.send(MemoryHandler.process.MainWindowHandle, "/script RepopMe()");
            Thread.Sleep(2000);
            MemoryHandler.writeInt64(Offset.MOUSE_OVER_GUID, ObjectManager.sharedOM().getSHGUID());
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
            return MemoryHandler.readInt(Offset.CTM_Action) == 4 ? true : false;
        }

        public void dismount() {
            ChatWriter.send(MemoryHandler.process.MainWindowHandle, "/run Dismount();");
        }
    }
}
