using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningBot {
    class Vendor {
        public Position3D landingPlace;
        public ulong guid;

        public Vendor(Position3D landingPlace, ulong guid) {
            this.landingPlace = landingPlace;
            this.guid = guid;
        }
    }    
}
