/*
--> GetItemStats LUA Class
> Description:		Get all item stats and return it as a JSON
> Parameters:		itemslot = The itemslot number to be scanned (1 = HEAD, ...)
> Output Lua var:	abotItemStatsResult <-- read this using GetLocalizedText or something alike...
> Example output:
{
   "armor":"5",
   "stamina":"4",
   "agility":"0",
   "strenght":"8",
   "intellect":"0",
   "spirit":"0",
   "attackpower":"20",
   "spellpower":"0",
   "mana":"0"
}
*/

namespace AmeisenBot.Character.Lua
{
    public static class GetItemStats
    {
        public static string Lua(int itemslot) => $"abotItemSlot={itemslot};abotItemStatsResult='noItem';stats={{}};abStats=GetItemStats(GetInventoryItemLink('player',abotItemSlot),stats);abotItemStatsResult='{{'..'\"armor\": \"'..tostring(stats[\"ITEM_MOD_ARMOR_SHORT\"] or 0)..'\",'..'\"stamina\": \"'..tostring(stats[\"ITEM_MOD_STAMINA_SHORT\"] or 0)..'\",'..'\"agility\": \"'..tostring(stats[\"ITEM_MOD_AGILITY_SHORT\"] or 0)..'\",'..'\"strenght\": \"'..tostring(stats[\"ITEM_MOD_STRENGHT_SHORT\"] or 0)..'\",'..'\"intellect\": \"'..tostring(stats[\"ITEM_MOD_INTELLECT_SHORT\"]or 0)..'\",'..'\"spirit\": \"'..tostring(stats[\"ITEM_MOD_SPIRIT_SHORT\"] or 0)..'\",'..'\"attackpower\": \"'..tostring(stats[\"ITEM_MOD_ATTACK_POWER_SHORT\"] or 0)..'\",'..'\"spellpower\": \"'..tostring(stats[\"ITEM_MOD_SPELL_POWER_SHORT\"] or 0)..'\",'..'\"mana\": \"'..tostring(stats[\"ITEM_MOD_MANA_SHORT\"] or 0)..'\"'..'}}';";

        public static string OutVar() => "abotItemStatsResult";
    }
}