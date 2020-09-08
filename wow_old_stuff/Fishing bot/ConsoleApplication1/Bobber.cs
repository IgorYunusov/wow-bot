using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FishingBot {
    class Bobber {
        public static int IS_BOBING = 0xBC;
        public static int CREATED_BY = 560;
        public static int GUID_OFFS = 0x30;

        public int baseAddr;

        public Int64 GUID;

        public Bobber(int baseAddr) {
            this.baseAddr = baseAddr;
            GUID = MemoryHandler.readHexAsInt64(baseAddr + GUID_OFFS);
        }

        public bool isBobbing() {
            return MemoryHandler.readInt16(baseAddr + IS_BOBING) == 1 ? true : false;
        }
    }
}
