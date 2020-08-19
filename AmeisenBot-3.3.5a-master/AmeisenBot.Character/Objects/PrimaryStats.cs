using AmeisenBot.Character.Lua;
using AmeisenBot.Character.Structs;
using AmeisenBotCore;
using AmeisenBotLogger;
using Newtonsoft.Json;

namespace AmeisenBot.Character.Objects
{
    public class PrimaryStats
    {
        /// <summary>
        /// Get the stats for yourself
        /// </summary>
        public PrimaryStats()
        {
            UpdateFromPlayer();
        }

        /// <summary>
        /// Get the stats of an item
        /// </summary>
        /// <param name="item">Item to get the stats from</param>
        public PrimaryStats(Item item)
        {
            UpdateFromItem(item);
        }

        public double Agility { get; set; }
        public double Armor { get; set; }
        public double Attackpower { get; set; }
        public double Intellect { get; set; }
        public double Mana { get; set; }
        public double Spellpower { get; set; }
        public double Spirit { get; set; }
        public double Stamina { get; set; }
        public double Strenght { get; set; }

        public void UpdateFromItem(Item item)
        {
            // Experimental! but should be 100x faster
            string itemStatsJson = AmeisenCore.GetLocalizedText(GetItemStats.Lua(item.Slot), GetItemStats.OutVar());
            AmeisenLogger.Instance.Log(LogLevel.DEBUG, $"GetItemStatsLuaJSON: {itemStatsJson}", this);
            // parse this JSON
            try
            {
                RawStats rawItem = JsonConvert.DeserializeObject<RawStats>(itemStatsJson);
                Armor = rawItem.armor;
                Strenght = rawItem.strenght;
                Agility = rawItem.agility;
                Stamina = rawItem.stamina;
                Intellect = rawItem.intellect;
                Spirit = rawItem.spirit;
                Attackpower = rawItem.attackpower;
                Spellpower = rawItem.spellpower;
                Mana = rawItem.mana;
            }
            catch { }
        }

        public void UpdateFromPlayer()
        {
        }
    }
}