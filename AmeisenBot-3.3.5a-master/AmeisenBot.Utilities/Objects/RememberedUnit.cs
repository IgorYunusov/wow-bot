using AmeisenBotUtilities.Enums;
using System.Collections.Generic;

namespace AmeisenBotUtilities.Objects
{
    public class RememberedUnit
    {
        public int MapID { get; set; }
        public string Name { get; set; }
        public Vector3 Position { get; set; }
        public List<UnitTrait> UnitTraits { get; set; }
        public string UnitTraitsString { get; set; }
        public int ZoneID { get; set; }
    }
}