using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FishingBot {
    class SpiritHealer {
        private static Int64[] SH_GUIDS_ARRAY = { -1067353002776230427, -1067353002776230191, -1067353002776230193 };
        public static List<Int64> SH_GUIDS = SH_GUIDS_ARRAY.ToList();

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
