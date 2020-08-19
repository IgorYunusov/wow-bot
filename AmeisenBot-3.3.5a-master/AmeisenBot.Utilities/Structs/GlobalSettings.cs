using System.Collections.Generic;

namespace AmeisenBotUtilities.Structs
{
    public class GlobalSettings
    {
        public string wowExePath = "none";
        public string wowRealmlistPath = "none";
        public List<string> wowRealmlists = new List<string>() { "127.0.0.1" };
        public int wowSelectedRealmlist = 0;
    }
}