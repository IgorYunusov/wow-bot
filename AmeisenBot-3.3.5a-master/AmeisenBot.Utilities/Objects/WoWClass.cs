namespace AmeisenBotUtilities
{
    public abstract class WoWClass
    {
        public abstract class DeathKnight
        {
            public static readonly int classID = 6;
        }

        public abstract class Druid
        {
            public static readonly int classID = 11;

            public enum ShapeshiftForms
            {
                BEAR,
                AQUA,
                CAT,
                TRAVEL,
                MOONKIN,
                TREE
            }
        }

        public abstract class Hunter
        {
            public static readonly int classID = 3;
        }

        public abstract class Mage
        {
            public static readonly int classID = 8;
        }

        public abstract class Paladin
        {
            public static readonly int classID = 2;
        }

        public abstract class Priest
        {
            public static readonly int classID = 5;
        }

        public abstract class Rogue
        {
            public static readonly int classID = 4;

            public enum ShapeshiftForms
            {
                STEALTH
            }
        }

        public abstract class Shaman
        {
            public static readonly int classID = 7;
        }

        public abstract class Warlock
        {
            public static readonly int classID = 9;
        }

        public abstract class Warrior
        {
            public static readonly int classID = 1;

            public enum ShapeshiftForms
            {
                BATTLE,
                DEFENSIVE,
                BERSERKER,
            }
        }
    }
}