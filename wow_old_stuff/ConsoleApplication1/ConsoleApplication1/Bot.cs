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

namespace MiningBot {

    class Bot {

        private Player player;
        private UI ui;
        private bool isMining;
        
        private MineralVein[] veinsAround;

        static Position[] miningPlaces = { new Position(3415.87f, 408.785f), new Position(3840.22f, 304.84f),
                                           new Position(3521.21f, 482.9f), new Position(3606.87f, 217.04f),
                                           new Position(3632.89f, 66.55f), new Position(4583.37f, -28.67f), 
                                           new Position(4700.14f, 27.03f), new Position(4801.25f, 136.44f), 
                                           new Position(4919.78f, 178.04f), new Position(4939.6f, -31.83f), 
                                           new Position(4941.62f, -163.1f), new Position(4728.34f, -713.15f), 
                                           new Position(4649.55f, -536.13f), new Position(4334.09f, -543.29f), 
                                           new Position(4175.67f, -1223.82f), new Position(3763.9f, -782.06f), 
                                           new Position(3579.18f, -936.07f), new Position(3303.78f, -1070.06f), 
                                           new Position(3501.09f, -1461.72f), new Position(3167.43f, -1337.81f), 
                                           new Position(2765.81f, -1406.3f), new Position(2535.31f, -1288.41f), 
                                           new Position(2200.87f, -1200.0f), new Position(1890.45f, -816.27f)
                                         };
        static Position3D[] prohibitedPositionsArray = { new Position3D(4986.97f, -160.1444f, 117.491f), new Position3D(4506.66f, -26.0816f, 82.6287f),
                                                         new Position3D(5101.0f, 394.0f, 21.0f), /*new Position(5084.0f, 60.0f), new Position(5114.0f, 203.0f),*/
                                                         new Position3D(2786, 450, 53), new Position3D(3600, 232, 125),
                                                         new Position3D(3602, 232, 125), new Position3D(5287, -597, 18)};
        public static List<Position3D> PROHIBITED_POSITIONS = prohibitedPositionsArray.ToList();
        static int currentPlace = 0;

        public Bot() {
            player = new Player();

            isMining = false;

            //player.takeOff();
            /*character.clickToMove(4797.0f, 3551.0f);
            character.clickToMove(4705.0f, 3548.0f);
            */
            /*Position playerPos;
            while (true) {
                System.Console.Clear();
                playerPos = Zone.getRelativePosition(player.getX(), player.getY());
                //System.Console.WriteLine("x: " + playerPos.x * 100 + ", y: " + playerPos.y * 100);
                System.Console.WriteLine(player.getX());
                System.Console.WriteLine(player.getY());
                //System.Console.WriteLine(player.getZ());
                Thread.Sleep(100);
            }*/

            //ui = new UI(this);
        }

        public void startMining(object mountNum) {
            isMining = true;

            player.setMountNum((int)mountNum);

            while(isMining){
                //a másik ciklusba nem ragadhat be, mert ha meghalok nincsenek a veinek az obj man be, így kilép belőle
                if (player.getCurrHP() < 2) {
                    System.Console.WriteLine("Ha hallott vagy éledj");
                    player.ressurrect();
                    continue;
                }
                
                System.Console.WriteLine("szállj fel");
                player.takeOff();
                
                System.Console.WriteLine("menj a kovetkezo helyre");
                player.flyTo(miningPlaces[currentPlace]);

                currentPlace++;
                if (currentPlace > miningPlaces.Length - 1) {
                    currentPlace = 0;
                }

                System.Console.WriteLine("nezd meg hogy van e mineral");
                veinsAround = ObjectManager.sharedOM().getMineralVeinsAround();
                //amig van :
                while (veinsAround.Length != 0 && player.getCurrHP() > 1 ) {
                    
                    System.Console.WriteLine("repulj kzvetlenul a node felé");
                    player.flyTo(veinsAround[0].position.to2DPosition());
                    Thread.Sleep(1500);
                    System.Console.WriteLine("szállj le a memoriábol kiolvasott node mellé 1 X nyivel");
                    player.clickToMove(veinsAround[0].getPointClose());

                    Thread.Sleep(500);

                    //System.Console.WriteLine("dismopunt");
                    //player.dismount();
                    //hogy dismount közbe ne legyen sebességem mert akkor elrepulok
                    //Thread.Sleep(300);

                    System.Console.WriteLine("ctloot a veinre, dismount helyett, mert ez mindig dismountol");
                    player.clickToLoot(veinsAround[0].position, veinsAround[0].guid);

                    System.Console.WriteLine("ctloot a veinre, dismount helyett, mert ez mindig dismountol");
                    player.clickToLoot(veinsAround[0].position, veinsAround[0].guid);

                    //várj egy kissé ha zuhannék egy keveset
                    Thread.Sleep(3000);
                    System.Console.WriteLine("ctloot a veinre");
                    player.clickToLoot(veinsAround[0].position, veinsAround[0].guid);
                    System.Console.WriteLine(veinsAround[0].position);
                    //várj 5 mp t amig kibányászom
                    Thread.Sleep(5000);
                    //ha meghaltam nem ezsek, ha keves a hp eszek
                    if (((float)player.getCurrHP() / (float)player.getMaxHP()) < 0.6f && player.getCurrHP() > 1) {
                        System.Console.WriteLine("ha keves a hp égy");
                        player.eat();
                    }
                    System.Console.WriteLine("szállj fel");
                    player.takeOff();
                    
                    System.Console.WriteLine("nézd meg  a mineral tombot ujra");
                    veinsAround = ObjectManager.sharedOM().getMineralVeinsAround();
                }  
            }
        }

        public void stopMining() {
            isMining = false;
        }

        public bool isMiningTrue() {
            return isMining;
        }

        public Position getPlayerPosition() {
            return player.getPosition() ;
        }
    }
}

//FÁJLBA IRASHOZ
/*using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\Franszöá\Desktop\akarmi.txt")) {
                for (int i = 0; i < 100000; ++i) {
                    file.WriteLine(readFloat(0xBD0A58 + 0x1 * i, processHandle));
                }
            }
*/