using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ConsoleApplication1 {
    class Player {
        private static int player = ObjectManager.sharedOM().getPlayerAdress();
        private static int MAX_HP = player + 6616;
        private static int CURR_HP = player + 4016;
        private static int MAX_MANA = MAX_HP + 0x4;
        private static int CURR_MANA = CURR_HP + 0x4;

        private static int X = player + 0x79C;
        private static int Y = player + 0x798;
        private static int Z = player + 0x7A0;

        private static int CTM_Base = 0x00CA11D8;
        private static int CTM_X = CTM_Base + 0x90;
        private static int CTM_Y = CTM_Base + 0x8C;
        private static int CTM_Z = CTM_Base + 0x94;
        private static int CTM_Action = CTM_Base + 0x1C;

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

        public double getX() {
            return MemoryHandler.readFloat(X);
        }

        public double getY() {
            return MemoryHandler.readFloat(Y);
        }

        public void flyTo(float x, float y, float z = 0.0f) {
            //ha nem adok meg z értéket, akkor marad a magasság ugyanaz
            if (z == 0.0f) { z = MemoryHandler.readFloat(Z); }

            //az értékek beírása a memoriába
            MemoryHandler.writeFloat(CTM_X, x);
            MemoryHandler.writeFloat(CTM_Y, y);
            MemoryHandler.writeFloat(CTM_Z, z);
            MemoryHandler.writeInt(CTM_Action, 4);

            //amíg az action 4, azaz mozgásban van, addig várok
            while (MemoryHandler.readInt(CTM_Action) == 4) {
                Thread.Sleep(200);
            }
        }

        public void takeOff() {
            ChatWriter.send(MemoryHandler.process.MainWindowHandle, "/run if IsMounted() == nil then CallCompanion(\"MOUNT\", 5) end");
            Thread.Sleep(2000);
            MemoryHandler.writeFloat(CTM_X, MemoryHandler.readFloat(X));
            MemoryHandler.writeFloat(CTM_Y, MemoryHandler.readFloat(Y));
            MemoryHandler.writeFloat(CTM_Z, MemoryHandler.readFloat(Z) + 800.0f);
            MemoryHandler.writeInt(CTM_Action, 4);

            Thread.Sleep(15000);
        }

        public void land() {

        }
    }
}
