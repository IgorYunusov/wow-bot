using AmeisenBot.Character.Lua;
using AmeisenBot.Character.Structs;
using AmeisenBotCore;
using AmeisenBotLogger;
using AmeisenBotUtilities;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace AmeisenBot.Character.Objects
{
    public class MeCharacter
    {
        public MeCharacter()
        {
        }

        public Equipment Equipment { get; set; }
        public bool FullyLoaded { get; private set; }
        public List<InventoryItem> InventoryItems { get; set; }
        public int Money { get; set; }
        public PrimaryStats PrimaryStats { get; set; }
        public Resistances Resistances { get; set; }
        public SecondaryStats SecondaryStats { get; set; }
        public List<Spell> Spells { get; set; }

        public void Update()
        {
            FullyLoaded = false;

            PrimaryStats = new PrimaryStats();
            PrimaryStats.UpdateFromPlayer();
            SecondaryStats = new SecondaryStats();
            Resistances = new Resistances();

            UpdateSpells();

            Equipment = new Equipment();

            UpdateInventory();

            UpdateMoney();
            FullyLoaded = true;

            string characterJson = JsonConvert.SerializeObject(this);
            AmeisenLogger.Instance.Log(LogLevel.DEBUG, $"Updated Character: {characterJson}", this);
        }

        public void UpdateInventory()
        {
            InventoryItems = new List<InventoryItem>();
            string inventoryItemsJson = AmeisenCore.GetLocalizedText(GetInventoryItems.Lua(), GetInventoryItems.OutVar());
            List<RawInventoryItem> rawInventoryItems = new List<RawInventoryItem>();

            try
            {
                rawInventoryItems = JsonConvert.DeserializeObject<List<RawInventoryItem>>(inventoryItemsJson);
            }
            catch
            {
                InventoryItems = new List<InventoryItem>();
                AmeisenLogger.Instance.Log(LogLevel.ERROR, $"Failes to parse InventoryItems", this);
            }

            foreach (RawInventoryItem rawInventoryItem in rawInventoryItems)
            {
                InventoryItems.Add(new InventoryItem(rawInventoryItem));
            }
        }

        public void UpdateMoney() => Money = Utils.TryParseInt(AmeisenCore.GetLocalizedText("moneyX = GetMoney();", "moneyX"));

        public void UpdateSpells()
        {
            Spells = new List<Spell>();
            string spellJson = AmeisenCore.GetLocalizedText(GetSpells.Lua(), GetSpells.OutVar());
            List<RawSpell> rawSpells = new List<RawSpell>();

            try
            {
                rawSpells = JsonConvert.DeserializeObject<List<RawSpell>>(spellJson);
            }
            catch
            {
                InventoryItems = new List<InventoryItem>();
                AmeisenLogger.Instance.Log(LogLevel.ERROR, $"Failes to parse Spells", this);
            }

            foreach (RawSpell rawSpell in rawSpells)
            {
                Spells.Add(new Spell(rawSpell));
            }
        }
    }
}