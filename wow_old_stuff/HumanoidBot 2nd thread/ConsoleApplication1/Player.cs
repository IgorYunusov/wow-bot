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
        private static int CTM_X = 0x00CA11D8 + 0x90;
        private static int CTM_Y = 0x00CA11D8 + 0x8C;
        private static int CTM_Z = 0x00CA11D8 + 0x94;
        private static int CTM_Distance = 0x00CA11D8 + 0xC;
        private static int CTM_Action = 0x00CA11D8 + 0x1C;
        private static int CTM_GUID = 0x00CA11D8 + 0x20;

        private static int CTM_STOP = 7;

        private static int IN_COMBAT = player + 0x4A290E0;

        private static int UNIT_FLAGS = player + 0x3B;

        private static int MOUSE_OVER_GUID = 0x00BD07A0;

        public bool metElite = false;


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

        public bool isMounted() {
            int movementFieldPtr = MemoryHandler.readInt((int)(player + PlayerOffsets.movementFieldPtr));
            int isMounted = MemoryHandler.readInt((int)(movementFieldPtr + PlayerOffsets.IsFlyingOffset)) & (int)PlayerOffsets.IsFlyingMount_Mask;
            return 0 != isMounted;
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

        public bool isDead(){
            return MemoryHandler.readInt(CURR_HP) < 2;
        }


        public Position getPosition() {
            position.x = MemoryHandler.readFloat(X);
            position.y = MemoryHandler.readFloat(Y);
            return position;
        }

        public void charge() {
            ChatWriter.hitKey(ChatWriter.ZERO);
        }

        public void figth() {
            ChatWriter.hitKey(ChatWriter.NINE);
            if(0.7f > getHPPercentage() && metElite){
                ChatWriter.hitKey(ChatWriter.EIGTH);
            }
            if (0.2f > getHPPercentage()) {
                ChatWriter.hitKey(ChatWriter.V);
            }
            ChatWriter.hitKey(ChatWriter.SEVEN);
            if (800 < getCurrRage()) {
                ChatWriter.hitKey(ChatWriter.SIX);
            }
            if(metElite){
                ChatWriter.hitKey(ChatWriter.FIVE);
            }
            
            ChatWriter.hitKey(ChatWriter.FOUR);
            ChatWriter.hitKey(ChatWriter.THREE);
            ChatWriter.hitKey(ChatWriter.TWO);
            ChatWriter.hitKey(ChatWriter.ONE);
        }

        public void cannibalize() {
            stop();
            Thread.Sleep(1000);
            ChatWriter.hitKey(ChatWriter.I);
            Thread.Sleep(11000);
        }

        public Position3D getPosition3D() {
            position3D.x = MemoryHandler.readFloat(X);
            position3D.y = MemoryHandler.readFloat(Y);
            position3D.z = MemoryHandler.readFloat(Z);
            return position3D;
        }

        public void interactWithMouseOver(ulong guid, int sleepTime = 1300) {
            //mozgás közben nem lehet lootolni
            stop();

            //az értékek beírása a memoriába
            MemoryHandler.writeULong(MOUSE_OVER_GUID, guid);

            ChatWriter.hitKey(ChatWriter.K);          

            Thread.Sleep(sleepTime);
        }



        public void attack(ulong guid, bool shouldCharge = false) {
            //az értékek beírása a memoriába
            MemoryHandler.writeULong(MOUSE_OVER_GUID, guid);

            ChatWriter.hitKey(ChatWriter.K);

            if (shouldCharge) {
                charge();
            }

            Thread.Sleep(800);
            //mer össze vissza rohangál
            stop();

            Thread.Sleep(200);
        }

        public void clickToLoot(Position3D position, ulong guid) {
            //az értékek beírása a memoriába
            MemoryHandler.writeULong(MOUSE_OVER_GUID, guid);
            //System.Console.WriteLine(guid.ToString("X"));
            ChatWriter.hitKey(ChatWriter.K);

            //MemoryHandler.writeInt(CTM_Action, 7);

            //amíg az action 7, azaz mozgásban van, addig várok

            Thread.Sleep(200);
        }

        public void runTo(Position3D pos) {
            MemoryHandler.writeFloat(CTM_X, pos.x);
            MemoryHandler.writeFloat(CTM_Y, pos.y);
            MemoryHandler.writeFloat(CTM_Z, pos.z);//MemoryHandler.readFloat(Z));
            MemoryHandler.writeInt(CTM_Action, 4);

            Thread.Sleep(200);
            if (nearPosition(pos, 6)) {
                ChatWriter.hitKey(ChatWriter.W);
                Thread.Sleep(100);
            }
        }

        public bool nearPosition(Position3D pos, int precision = 4) {
            return Math.Abs((getX() - pos.x)) < 4 && Math.Abs(getY() - pos.y) < 4;
        }

        public void mount() {
            ChatWriter.hitKey(ChatWriter.F);
            Thread.Sleep(4000);
        }

        public void dismount() {
            ChatWriter.hitKey(ChatWriter.Z);
            Thread.Sleep(4000);
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
            while (MemoryHandler.readInt(CTM_Action) == 4 && !isDead()) {
                Thread.Sleep(200);
            }
            Thread.Sleep(400);
        }

        public void goToPlace(Position3D place) {
            takeOff();
            flyTo(place.to2DPosition());
            clickToMove(place);
            dismount();
            clickToMove(place);
            Thread.Sleep(1000);
            //ha leszálltam és ctm eltem a megfelelo helyre, de még mindig nem vagyok ott, akkor ujraprobalmom az egeszet
        }

        public void stop() {
            MemoryHandler.writeInt(CTM_Action, CTM_STOP);
        }

        public void flyTo(Position position) {
            //míg az x,y távolság nagyobb mint 10 várok
            stopwatch.Reset();
            stopwatch.Start();
            while (
                (Math.Abs(MemoryHandler.readFloat(X) - position.x) > 10.0f || 
                Math.Abs(MemoryHandler.readFloat(Y) - position.y) > 10.0f) && !isDead()
                ) {
                //ha sokat baszakodik, akkor ujra felmountolok
                /*if (stopwatch.ElapsedMilliseconds > 60000) {
                    System.Console.WriteLine("nem nagyon jutok a célhoz, ujramountolok");
                    MemoryHandler.writeInt(CTM_Action, 11);
                    takeOff();
                    stopwatch.Reset();
                    stopwatch.Start();
                }*/

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
            stop();
            Thread.Sleep(500);

            mount();

            while(!isMounted()){
                while(ObjectManager.sharedOM().getEnemies().Count > 0){
                    attack(ObjectManager.sharedOM().getEnemies()[0].Guid);
                    figth();
                }
                mount();
            }

            MemoryHandler.writeFloat(CTM_X, MemoryHandler.readFloat(X));
            MemoryHandler.writeFloat(CTM_Y, MemoryHandler.readFloat(Y));
            MemoryHandler.writeFloat(CTM_Z, MemoryHandler.readFloat(Z) + 800.0f);
            MemoryHandler.writeInt(CTM_Action, 4);

            //néha nem repül fel egyből, hanem megáll 1 méterre a föld felett, ezért ujra kell kattintani
            Thread.Sleep(1000);
            MemoryHandler.writeInt(CTM_Action, 4);

            Thread.Sleep(7000);
        }

        public void ressurrect() {
            while (getCurrHP() < 2) {
                ChatWriter.hitKey(ChatWriter.J);
                Thread.Sleep(2000);
                ulong shGUID = ObjectManager.sharedOM().getSpiritHealer();
                interactWithMouseOver(shGUID, 4000);
                ChatWriter.hitKey(ChatWriter.U);
            }
        }

        public float getHPPercentage(){
            return (float)getCurrHP() / getMaxHP();
        }

        public void eat() {
            stop();
            ChatWriter.hitKey(ChatWriter.G);
            int i = 0;
            bool combat = false;
            while (i < 5 && !combat) {
                Thread.Sleep(3000);
                if (ObjectManager.sharedOM().getEnemies().Count > 0) {
                    combat = true;
                }
                ++i;
            }
        }

        public void setMountNum(int mountNum) {
            this.mountNum = mountNum;
        }

        public bool isMoving() {
            return MemoryHandler.readInt(CTM_Action) == 4 ? true : false;
        }

        /*public void dismount() {
            ChatWriter.send(MemoryHandler.process.MainWindowHandle, "/run Dismount();");
        }*/
    }
}
