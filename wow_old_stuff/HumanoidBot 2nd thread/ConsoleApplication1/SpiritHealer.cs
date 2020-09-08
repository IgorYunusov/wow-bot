using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningBot {
    class SpiritHealer {
        public static string[] SH_GUIDS_ARRAY = { "F13000195B9385E5 ", "F13000195B9386D1", "F13000195B9386CF"};
        public static List<String> SH_GUIDS = SH_GUIDS_ARRAY.ToList();

        public static int X = 0xEC;
        public static int Y = 0x1F4;
        public static int Z = 0x1E0;

        public Position3D position;
        public Int64 guid;

        public SpiritHealer(Position3D position, Int64 guid) {
            this.position = position;
            this.guid = guid;
        }

        public override string ToString() {
            return "x: " + position.x + ", y: " + position.y + ", z: " + position.z + ", guid: " + guid;
        }
    }
}
