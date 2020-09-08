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

    class HumanoidBot {

        private static Player player;
        private bool isMining;

        private Position3D posBeforeOtherCome;

        private Position3D[] startingPlaces;        
        private Position3D[][] farmingPlaces;

        public static List<String> ELITES;
        public static ulong eliteGUID = 0;

        static int currentPoint = 0;
        static int currentFarmingPlace = 0;

        private ulong enemyNearPos = 0;

        private List<ulong> enemiesToLoot = new List<ulong>();
        private List<ulong> enemiesToRemove = new List<ulong>();

        private bool goToVendor = false;
        private bool shouldCannibalize = false;

        List<WowObject> enemies;

        private int runNum = 0;
        private int maxRun;

        Vendor Darmend = new Vendor(new Position3D(845, -3029, -9), 0xF130004C4A00D2EE);

        private int triedToRun = 0;

        public HumanoidBot(Position3D[] startingPlaces, Position3D[][] farmingPlaces, string[] elitesArr, int maxRun, bool shouldCannibalize) {
            this.startingPlaces = startingPlaces;
            this.farmingPlaces = farmingPlaces;
            ELITES = new List<string>(elitesArr);
            this.maxRun = maxRun;
            this.shouldCannibalize = shouldCannibalize;
            
            player = new Player();
            posBeforeOtherCome = player.getPosition3D();
            isMining = false;
        }

        public void printLocation() {
            while (true) {
                System.Console.Clear();
                System.Console.WriteLine("x: " + player.getX() + " y: " + player.getY() + " z: " + player.getZ());
                Position3D playerPos = player.getPosition3D();
                System.Console.WriteLine( ObjectManager.sharedOM().isPlayerNear(playerPos));
                Thread.Sleep(200);
            }
        }

        public void startMining(object mountNum) {
            isMining = true;
            player.goToPlace(startingPlaces[currentFarmingPlace]);

            while (isMining) {
                enemies = ObjectManager.sharedOM().getEnemies();
                foreach (WowObject e in enemies) {
                    if (!enemiesToLoot.Contains(e.Guid)) {
                        enemiesToLoot.Add(e.Guid);
                    }
                }

                //ha a következő pont közelébe van elenfél, akkor meg fogom támadni a "menj a köv helyre" részben
                enemyNearPos = ObjectManager.sharedOM().getEnemyNearPos(farmingPlaces[currentFarmingPlace][currentPoint]);
                if (!enemiesToLoot.Contains(enemyNearPos) && enemyNearPos != 0) {
                    enemiesToLoot.Add(enemyNearPos);
                }

                //éledj
                if (player.isDead()) {
                    System.Console.WriteLine("Ha hallott vagy éledj");
                    player.ressurrect();
                    currentPoint = 0;
                    player.metElite = false;
                    eliteGUID = 0;
                    player.goToPlace(Darmend.landingPlace);
                    player.interactWithMouseOver(Darmend.guid, 5000);
                    for (int i = 0; i < 8; ++i) {
                        ChatWriter.hitKey(ChatWriter.W);
                        Thread.Sleep(60000);
                        ChatWriter.hitKey(ChatWriter.S);
                    }
                    player.goToPlace(startingPlaces[currentFarmingPlace]);
                    continue;
                }
                //lootolj
                else if(isThereEnemyToLoot()){
                    System.Console.WriteLine("Lootolj");
                    enemiesToRemove.Clear();
                    foreach (ulong guid in enemiesToLoot) {
                        if (ObjectManager.sharedOM().getHealthByGUID(guid) == 0) {
                            player.interactWithMouseOver(guid);
                            enemiesToRemove.Add(guid);
                            if (guid == eliteGUID) {
                                eliteGUID = 0;
                            }
                        }
                    }
                    foreach (ulong guid in enemiesToRemove) {
                        enemiesToLoot.Remove(guid);
                    }
                }
                //harcolj
                else if (enemies.Count > 0) {
                    System.Console.WriteLine("Harcolj");
                    
                    WowObject enemy = enemies[0];

                    if (eliteGUID == 0) {
                        player.attack(enemy.Guid);
                    }
                    else {
                        player.attack(eliteGUID);
                    }
                    player.figth();
                }
                //másik player
                /*else if(ObjectManager.sharedOM().isPlayerNear(player.getPosition3D())){
                    if (!player.isMounted()) {
                        Position3D lastPos = player.getPosition3D();
                    }
                    player.takeOff();
                    while (ObjectManager.sharedOM().isPlayerNear(player.getPosition3D())) {
                        Thread.Sleep(1000);
                    }
                    player.goToPlace(posBeforeOtherCome);
                }*/
                //egyél
                else if(0.5f > player.getHPPercentage()){
                    if (shouldCannibalize) {
                        player.cannibalize();
                    }
                    if (0.5f > player.getHPPercentage()) {
                        player.eat();
                    }
                }
                //menj a vendorhoz
                else if (goToVendor) {
                    goToVendor = false;                    
                    player.goToPlace(Darmend.landingPlace);
                    player.interactWithMouseOver(Darmend.guid, 5000);
                    player.goToPlace(startingPlaces[currentFarmingPlace]);
                    
                }
                //menj a köv helyre
                else if (ObjectManager.sharedOM().getEnemies().Count < 1) {
                    System.Console.WriteLine("Menj a köv helyre");
                    player.runTo(farmingPlaces[currentFarmingPlace][currentPoint]);
                    triedToRun++;
                    System.Console.WriteLine(currentPoint);
                    if (triedToRun == 100) {
                        triedToRun = 0;
                        currentPoint = 0;
                        eliteGUID = 0;

                        player.metElite = false;
                        currentFarmingPlace++;
                        if (currentFarmingPlace == startingPlaces.Length) {
                            currentFarmingPlace = 0;
                            runNum++;
                        }

                        player.goToPlace(startingPlaces[currentFarmingPlace]);
                    }
                    //ha van a pont közelébe ellenfél akkor megtámadom
                    else if(enemyNearPos != 0){
                        player.attack(enemyNearPos, true);
                    }
                    else if (player.nearPosition(farmingPlaces[currentFarmingPlace][currentPoint])) {
                        currentPoint++;
                        triedToRun = 0;

                        //ha az adott farming place végén vagy, menj a következő farming place re
                        if (currentPoint == farmingPlaces[currentFarmingPlace].Length) {
                            currentPoint = 0;
                            eliteGUID = 0;
                            
                            player.metElite = false;
                            currentFarmingPlace++;
                            if (currentFarmingPlace == startingPlaces.Length) {
                                currentFarmingPlace = 0;
                                runNum++;
                            }
                            if (runNum >= maxRun) {
                                runNum = 0;
                                goToVendor = true;
                            }
                            else {
                                player.goToPlace(startingPlaces[currentFarmingPlace]);
                            }
                        }
                    }
                }
            }

        }

        public static void metElite(WowObject enemy) {
            if (ELITES.Contains(enemy.Name)) {
                eliteGUID = enemy.Guid;
                player.metElite = true;
            }
        }

        public bool isThereEnemyToLoot() {
            foreach (ulong guid in enemiesToLoot) {
                if (ObjectManager.sharedOM().getHealthByGUID(guid) == 0) {
                    return true;
                }
            }
            return false;
        }

        public void stopMining() {
            isMining = false;
        }

        public bool isMiningTrue() {
            return isMining;
        }

        public Position getPlayerPosition() {
            return player.getPosition();
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