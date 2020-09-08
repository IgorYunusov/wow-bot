using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MiningBot.resources;

namespace MiningBot {
    class MainProgram {
        [STAThread]
        static void Main(string[] args) {
            MemoryHandler.createReader();

            //ObjectManager.sharedOM().getSpell();

            /*while (true) {
                ulong a = ObjectManager.sharedOM().getSpiritHealer();
                System.Console.Clear();
                System.Console.WriteLine(a);
                Thread.Sleep(200);
            }*/

            //InterceptKeys.writeToLocationsTxt(); //X et kell nyomi, hogy hozzáírjon valamit            

            HumanoidBot bot = new HumanoidBot(BotData.startingPlaces1, BotData.farmingPlaces1, BotData.elitesArr1, true);
            //bot.printLocation();
            bot.startMining(1);

            Thread.Sleep(1111113000);
        }
    }
}
