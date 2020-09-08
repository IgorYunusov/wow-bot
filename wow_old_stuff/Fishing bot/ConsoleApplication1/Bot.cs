using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Windows.Forms;
using MouseKeyboardLibrary;
using System.Threading;

namespace FishingBot {

    class Bot {

        private Player player;
        private bool isBotting;

        public Bot() {
            player = new Player();           
        }

        public void startBotting(object mountNum) {
            isBotting = true;

            player.setMountNum((int)mountNum);

            while (isBotting) {
                //a másik ciklusba nem ragadhat be, mert ha meghalok nincsenek a veinek az obj man be, így kilép belőle
                if (player.getCurrHP() < 2) {
                    System.Console.WriteLine("Ha hallott vagy éledj");
                    player.ressurrect();
                    continue;
                }

                System.Console.WriteLine("horgassz");
                player.fish();
            }
        }

        public void stopBotting() {
            isBotting = false;
        }

        public bool isMiningTrue() {
            return isBotting;
        }

        public Position getPlayerPosition() {
            return player.getPosition() ;
        }
    }
}