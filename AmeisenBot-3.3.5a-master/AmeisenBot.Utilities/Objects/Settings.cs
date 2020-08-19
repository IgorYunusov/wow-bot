namespace AmeisenBotUtilities
{
    /// <summary>
    /// Class containing the default and loaded settings
    /// </summary>
    public class Settings
    {
        public string accentColor = "#FFAAAAAA";
        public string ameisenServerIp = "127.0.0.1";
        public string ameisenServerName = Utils.GenerateRandonString(12, "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890");
        public int ameisenServerPort = 5000;
        public bool assistParty = true;
        public bool autoUltralowGfx = true;
        public string backgroundColor = "#FF323232";
        public bool behaviourAttack = false;
        public bool behaviourBuff = true;
        public bool behaviourHeal = false;
        public bool behaviourTank = false;
        public string combatClassPath = "none";
        public bool databaseAutoConnect = true;
        public string databaseIp = "127.0.0.1";
        public string databaseName = "ameisenbot";
        public string databasePasswort = "AmeisenPassword";
        public int databasePort = 3306;
        public string databaseUsername = "ameisenbot";
        public int dataRefreshRate = 250;
        public bool doOwnStuff = false;
        public string energyColor = "#FF60D4FF";
        public string expColor = "#FFD4A0FF";
        public string flyingMounts = "";
        public double followDistance = 3.0;
        public bool followMaster = false;
        public string healthColor = "#FFFF6060";
        public string holoLogoColor = "#19FFFFFF";
        public string landMounts = "";
        public string masterName = "";
        public int maxFpsBackground = 20;
        public int maxFpsForeground = 30;
        public string meNodeColor = "#FFFF6060";
        public double movementJumpThreshold = 0.1;
        public bool navigationServerAutoConnect = true;
        public string navigationServerIp = "127.0.0.1";
        public int navigationServerPort = 47110;
        public double oldXindowPosX = 0;
        public double oldXindowPosY = 0;
        public int pathfindingSearchRadius = 2;
        public double pathfindingUsageThreshold = 10;
        public string picturePath = "";
        public string randomEmoteList = "dance,shrug,laugh,train,joke,fart,bravo,chicken";
        public bool randomEmotes = true;
        public bool releaseSpirit = true;
        public bool revive = true;
        public bool saveBotWindowPosition = true;
        public bool saveWoWWindowPosition = true;
        public bool serverAutoConnect = true;
        public int stateMachineStateUpdateMillis = 250;
        public int stateMachineUpdateMillis = 250;
        public string targetEnergyColor = "#FFFFA160";
        public string targetHealthColor = "#FFFF6060";
        public string textColor = "#FFFFFFFF";
        public bool topMost = false;
        public double useMountFollowDistance = 20;
        public bool usePathfinding = true;
        public string walkableNodeColorHigh = "#FFA0FFFF";
        public string walkableNodeColorLow = "#FFA0FF00";
        public double wowRectB = 0;
        public double wowRectL = 0;
        public double wowRectR = 0;
        public double wowRectT = 0;
        public string relationshipsPath = "";
    }
}