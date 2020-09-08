using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningBot {
    class MineralVein {

        private static int[] mineralIDsArray = { 181557, 181556, 181555, 185877, 185557, 181570 };
        public static List<int> mineralIDs = mineralIDsArray.ToList();

        public static int X = 0xEC;
        public static int Y = 0x1F4;
        public static int Z = 0x1E0;
        public static int TYPE_ID = 0x224;

        public Position3D position;
        public Int64 guid;

        public MineralVein(Position3D position, Int64 guid) {
            this.position = position;
            this.guid = guid;
        }

        //a node melleti pontba megyek hogy ne a tetejére szálljak, épp mellé
        public Position3D getPointClose(){
            Random rnd = new Random();
            return new Position3D(position.x - rnd.Next(3) - 1, position.y - rnd.Next(3) - 1, position.z);
        }
    }
}
