using AmeisenBot.Character.Enums;
using AmeisenBotLogger;

namespace AmeisenBot.Character.Objects
{
    public class InventoryItem : Item
    {
        public InventoryItem(RawInventoryItem rawInventoryItem)
        {
            try
            {
                Id = int.Parse(rawInventoryItem.id);
                Count = int.Parse(rawInventoryItem.count);
                Quality = (ItemQuality)int.Parse(rawInventoryItem.quality);
                DurabilityCurrent = int.Parse(rawInventoryItem.curDurability);
                DurabilityMax = int.Parse(rawInventoryItem.maxDurability);
                CooldownStart = int.Parse(rawInventoryItem.cooldownStart);
                CooldownEnd = int.Parse(rawInventoryItem.cooldownEnd);
                Lootable = int.Parse(rawInventoryItem.lootable) == 1;
                Readable = int.Parse(rawInventoryItem.readable) == 1;
                Name = rawInventoryItem.name;
                ItemLink = rawInventoryItem.link;
                Level = int.Parse(rawInventoryItem.level);
                RequiredLevel = int.Parse(rawInventoryItem.minLevel);
                ItemType = rawInventoryItem.type;
                ItemSubtype = rawInventoryItem.subtype;
                MaxStack = int.Parse(rawInventoryItem.maxStack);
                EquipLocation = rawInventoryItem.equiplocation;
                Price = int.Parse(rawInventoryItem.sellprice);
                Slot = -1;
            }
            catch { AmeisenLogger.Instance.Log(LogLevel.ERROR, "Error parsing InventoryItem", this); }
        }

        public bool Lootable { get; private set; }
        public bool Readable { get; private set; }
    }
}