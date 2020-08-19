using AmeisenBotUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmeisenBotFSM.Objects
{
    public struct Waypoint
    {
        public Vector3 Position { get; set; }
        public Vector3 OriginalPosition { get; set; }
    }
}
