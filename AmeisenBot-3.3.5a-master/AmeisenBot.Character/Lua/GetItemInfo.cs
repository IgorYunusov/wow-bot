/*
--> GetItemInfo LUA Class
> Description:		Get all item information and return it as a JSON
> Parameters:		itemslot = The itemslot number to be scanned (1 = HEAD, ...)
> Output Lua var:	abotItemInfoResult <-- read this using GetLocalizedText or something alike...
> Example output:
{
    "id": "12345",
    "count": "10",
    "quality": "2",
    "curDurability": "40",
    "maxDurability": "100",
    "cooldownStart": "1",
    "cooldownEnd": "5",
    "name": "sampleItem",
    "link": "sampleLink",
    "level": "260",
    "minLevel": "80",
    "type": "sampletype",
    "subtype": "sampleSubtype",
    "maxStack": "80",
    "equiplocation": "sampleEquiplocation",
    "sellprice": "500",
    "classId": "6",
    "subclassId": "7",
    "bindtype": "sampleBindtype",
    "expacId": "666",
    "setId": "855",
    "isCraftingReagent": "0"
}
*/

namespace AmeisenBot.Character.Lua
{
    public static class GetItemInfo
    {
        public static string Lua(int itemslot) => $"abotItemSlot={itemslot};abotItemInfoResult='noItem';abId=GetInventoryItemID('player',abotItemSlot);abCount=GetInventoryItemCount('player',abotItemSlot);abQuality=GetInventoryItemQuality('player',abotItemSlot);abCurrentDurability,abMaxDurability=GetInventoryItemDurability(abotItemSlot);abCooldownStart,abCooldownEnd=GetInventoryItemCooldown('player',abotItemSlot);abName,abLink,abRarity,abLevel,abMinLevel,abType,abSubType,abStackCount,abEquipLoc,abIcon,abSellPrice=GetItemInfo(GetInventoryItemLink('player',abotItemSlot));abotItemInfoResult='{{'..'\"id\": \"'..tostring(abId or 0)..'\",'..'\"count\": \"'..tostring(abCount or 0)..'\",'..'\"quality\": \"'..tostring(abQuality or 0)..'\",'..'\"curDurability\": \"'..tostring(abCurrentDurability or 0)..'\",'..'\"maxDurability\": \"'..tostring(abMaxDurability or 0)..'\",'..'\"cooldownStart\": \"'..tostring(abCooldownStart or 0)..'\",'..'\"cooldownEnd\": '..tostring(abCooldownEnd or 0)..','..'\"name\": \"'..tostring(abName or 0)..'\",'..'\"link\": \"'..tostring(abLink or 0)..'\",'..'\"level\": \"'..tostring(abLevel or 0)..'\",'..'\"minLevel\": \"'..tostring(abMinLevel or 0)..'\",'..'\"type\": \"'..tostring(abType or 0)..'\",'..'\"subtype\": \"'..tostring(abSubType or 0)..'\",'..'\"maxStack\": \"'..tostring(abStackCount or 0)..'\",'..'\"equiplocation\": \"'..tostring(abEquipLoc or 0)..'\",'..'\"sellprice\": \"'..tostring(abSellPrice or 0)..'\"'..'}}';";

        public static string Lua(string itemName) => $"abotItemName=\"{itemName}\";abotItemInfoResult='noItem';abName,abLink,abRarity,abLevel,abMinLevel,abType,abSubType,abStackCount,abEquipLoc,abIcon,abSellPrice=GetItemInfo(abotItemName);abotItemInfoResult='{{'..'\"id\": \"0\",'..'\"count\": \"1\",'..'\"quality\": \"'..tostring(abRarity or 0)..'\",'..'\"curDurability\": \"0\",'..'\"maxDurability\": \"0\",'..'\"cooldownStart\": \"0\",'..'\"cooldownEnd\": \"0\",'..'\"name\": \"'..tostring(abName or 0)..'\",'..'\"link\": \"'..tostring(abLink or 0)..'\",'..'\"level\": \"'..tostring(abLevel or 0)..'\",'..'\"minLevel\": \"'..tostring(abMinLevel or 0)..'\",'..'\"type\": \"'..tostring(abType or 0)..'\",'..'\"subtype\": \"'..tostring(abSubType or 0)..'\",'..'\"maxStack\": \"'..tostring(abStackCount or 0)..'\",'..'\"equiplocation\": \"'..tostring(abEquipLoc or 0)..'\",'..'\"sellprice\": \"'..tostring(abSellPrice or 0)..'\"'..'}}';";

        public static string OutVar() => "abotItemInfoResult";
    }
}