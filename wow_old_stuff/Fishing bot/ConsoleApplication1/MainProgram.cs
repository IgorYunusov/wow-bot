using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace FishingBot {
    class MainProgram {
        [STAThread]
        static void Main(string[] args) {
            //a memoriaolvasó inicializálása
            MemoryHandler.createReader();
            Bot fishingBot = new Bot();
            fishingBot.startBotting(1);
            //ObjectManager.sharedOM().test();
            Thread.Sleep(1111113000);
        }
    }
}
