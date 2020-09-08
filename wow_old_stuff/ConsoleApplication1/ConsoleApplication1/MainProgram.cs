using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MiningBot {
    class MainProgram {
        [STAThread]
        static void Main(string[] args) {
            MemoryHandler.createReader();

            Bot bot = new Bot();
            bot.startMining(1);

            Thread.Sleep(1111113000);
        }
    }
}
