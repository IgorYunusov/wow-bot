using AmeisenBot.Character.Comparators;
using AmeisenBot.Character.Interfaces;
using AmeisenBot.Character.Objects;
using AmeisenBotCore;
using AmeisenBotLogger;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AmeisenBot.Character
{
    public class AmeisenCharacterManager
    {
        public AmeisenCharacterManager()
        {
            Character = new MeCharacter();
        }

        public MeCharacter Character { get; private set; }

        /// <summary>
        /// This method equips better item if they are determined
        /// "better" by the supplied IItemComparator.
        ///
        /// Default Comparator is only looking for a better ItemLevel
        /// </summary>
        public bool CompareItems(Item currentItem, Item newItem, IItemComparator itemComparator = null)
        {
            if (itemComparator == null)
            {
                itemComparator = new BasicItemLevelComparator();
            }
            if (currentItem == null || itemComparator.Compare(newItem, currentItem))
            {
                return true;
            }
            return false;
        }

        public void EquipAllBetterItems()
        {
            bool replacedItem = false;
            if (Character.FullyLoaded)
            {
                foreach (Item item in Character.Equipment.AsList())
                {
                    if (item.Id != 0)
                    {
                        List<InventoryItem> itemsLikeItem = GetAllItemsLike(item);

                        if (itemsLikeItem.Count > 0)
                        {
                            Item possibleNewItem = itemsLikeItem.First();
                            if (CompareItems(item, possibleNewItem))
                            {
                                ReplaceItem(item, possibleNewItem);
                                replacedItem = true;
                            }
                        }
                    }
                    else
                    {
                        // we have no item equipped
                        List<InventoryItem> bestForSlotItem = GetAllItemsForSlot(item);
                        if (bestForSlotItem.Count > 0)
                        {
                            AmeisenCore.RunSlashCommand($"/equip {bestForSlotItem.First().Name}");
                            replacedItem = true;
                        }
                    }
                }
            }
            else { AmeisenLogger.Instance.Log(LogLevel.WARNING, "Could not Equip better items, Character is still loading", this); }

            if (replacedItem)
            {
                UpdateCharacterAsync();
            }
        }

        public bool INeedThatItem(string itemName)
        {
            Item itemToRollFor = new Item(itemName);
            AmeisenLogger.Instance.Log(LogLevel.DEBUG, $"Item to roll for: {itemToRollFor.ToString()}", this);

            List<InventoryItem> itemsLikeItem = GetAllItemsLike(itemToRollFor);

            if (itemsLikeItem.Count > 0)
            {
                Item possibleNewItem = itemsLikeItem.First();
                if (CompareItems(itemToRollFor, possibleNewItem))
                {
                    return true;
                }
            }
            return false;
        }

        public void ReplaceItem(Item currentItem, Item newItem)
        {
            AmeisenCore.LuaDoString($"EquipItemByName(\"{newItem.Name}\", {currentItem.Slot});");
            AmeisenCore.LuaDoString("ConfirmBindOnUse();");
            AmeisenCore.RunSlashCommand("/click StaticPopup1Button1");
            AmeisenLogger.Instance.Log(LogLevel.DEBUG, $"Equipped new Item...", this);
        }

        /// <summary>
        /// Update the whole character, may takes some time
        /// Updates stuff: Gear, Bags, Stats, Items
        /// </summary>
        public void UpdateCharacter() => Character.Update();

        /// <summary>
        /// Update the whole character, may takes some time
        /// Updates stuff: Gear, Bags, Stats, Items
        /// </summary>
        public void UpdateCharacterAsync() => new Thread(new ThreadStart(Character.Update)).Start();

        private List<InventoryItem> GetAllItemsForSlot(Item item)
            => Character.InventoryItems.Where(s => s.EquipLocation != "").Where(s => SlotToEquipLocation(item.Slot).Contains(s.EquipLocation)).OrderByDescending(x => x.Level).ToList();

        private List<InventoryItem> GetAllItemsLike(Item item)
            => Character.InventoryItems.Where(s => s.EquipLocation == item.EquipLocation).OrderByDescending(x => x.Level).ToList();

        private string SlotToEquipLocation(int slot)
        {
            switch (slot)
            {
                case 0: return "INVTYPE_AMMO";
                case 1: return "INVTYPE_HEAD";
                case 2: return "INVTYPE_NECK";
                case 3: return "INVTYPE_SHOULDER";
                case 4: return "INVTYPE_BODY";
                case 5: return "INVTYPE_CHEST|INVTYPE_ROBE";
                case 6: return "INVTYPE_WAIST";
                case 7: return "INVTYPE_LEGS";
                case 8: return "INVTYPE_FEET";
                case 9: return "INVTYPE_WRIST";
                case 10: return "INVTYPE_HAND";
                case 11: return "INVTYPE_FINGER";
                case 12: return "INVTYPE_FINGER";
                case 13: return "INVTYPE_TRINKET";
                case 14: return "INVTYPE_TRINKET";
                case 15: return "INVTYPE_CLOAK";
                case 16: return "INVTYPE_2HWEAPON|INVTYPE_WEAPON|INVTYPE_WEAPONMAINHAND";
                case 17: return "INVTYPE_SHIELD|INVTYPE_WEAPONOFFHAND|INVTYPE_HOLDABLE";
                case 18: return "INVTYPE_RANGED|INVTYPE_THROWN|INVTYPE_RANGEDRIGHT|INVTYPE_RELIC";
                case 19: return "INVTYPE_TABARD";
                case 20: return "INVTYPE_BAG|INVTYPE_QUIVER";
                case 21: return "INVTYPE_BAG|INVTYPE_QUIVER";
                case 22: return "INVTYPE_BAG|INVTYPE_QUIVER";
                case 23: return "INVTYPE_BAG|INVTYPE_QUIVER";
                default: return "none";
            }
        }
    }
}