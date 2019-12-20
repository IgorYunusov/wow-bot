﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using IceFlake.Client.Patchables;

namespace IceFlake.Client.Objects
{
    public class WoWItem : WoWObject
    {
        #region Typedefs & Delegates

        private static UseItemDelegate _useItem;

        private static GetItemInfoBlockByIdDelegate _getItemInfoBlockById;

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate IntPtr GetItemInfoBlockByIdDelegate(
            IntPtr instance, uint id, ref ulong guid, int a4 = 0, int a5 = 0, int a6 = 0);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        private delegate void UseItemDelegate(IntPtr thisObj, ref ulong guid, int unkZero);

        #endregion

        public WoWItem(IntPtr pointer)
            : base(pointer)
        {
            if (IsValid)
            {
                ItemInfo = GetItemRecordFromId(Entry, Guid);
            }
        }

        public ulong OwnerGuid
        {
            get { return GetDescriptor<ulong>(WoWItemFields.ITEM_FIELD_OWNER); }
        }

        public ulong CreatorGuid
        {
            get { return GetDescriptor<ulong>(WoWItemFields.ITEM_FIELD_CREATOR); }
        }

        public uint StackCount
        {
            get { return GetDescriptor<uint>(WoWItemFields.ITEM_FIELD_STACK_COUNT); }
        }

        public ItemFlags Flags
        {
            get { return (ItemFlags) GetDescriptor<uint>(WoWItemFields.ITEM_FIELD_FLAGS); }
        }

        public uint RandomPropertiesId
        {
            get { return GetDescriptor<uint>(WoWItemFields.ITEM_FIELD_RANDOM_PROPERTIES_ID); }
        }

        public uint Durability
        {
            get { return GetDescriptor<uint>(WoWItemFields.ITEM_FIELD_DURABILITY); }
        }

        public uint MaxDurability
        {
            get { return GetDescriptor<uint>(WoWItemFields.ITEM_FIELD_MAXDURABILITY); }
        }

        public IEnumerable<ItemEnchantment> Enchants
        {
            get
            {
                for (int i = 0; i < 12; i++)
                    if (GetAbsoluteDescriptor<uint>((int) WoWItemFields.ITEM_FIELD_ENCHANTMENT_1_1*0x4 + (i*12)) > 0)
                        yield return
                            GetAbsoluteDescriptor<ItemEnchantment>((int) WoWItemFields.ITEM_FIELD_ENCHANTMENT_1_1*0x4 +
                                                                   (i*12));
            }
        }

        public bool IsSoulbound
        {
            get { return ((uint) Flags & 1u) != 0; }
        }

        public ItemCacheRecord ItemInfo { get; private set; }

        public void Use()
        {
            Use(Manager.LocalPlayer);
        }

        public void Use(WoWObject target)
        {
            if (_useItem == null)
                _useItem = Manager.Memory.RegisterDelegate<UseItemDelegate>((IntPtr) Pointers.Item.UseItem);
            ulong guid = target.Guid;
            _useItem(Manager.LocalPlayer.Pointer, ref guid, 0);
        }

        // TODO: This can probably be optimized (a lot)
        public bool GetSlotIndexes(out int container, out int slot)
        {
            // Remember that lua is 1 indexed based!
            container = -1;
            slot = -1;

            // Backpack
            for (int i = 0; i < 16; i++)
            {
                WoWItem item = Manager.LocalPlayer.GetBackpackItem(i);
                if (item == null || !item.IsValid) continue;
                if (item.Guid == Guid)
                {
                    container = 0;
                    slot = i + 1;
                    return true;
                }
            }

            // All the bags
            for (var i = (int) BagSlot.Bag1; i < (int) BagSlot.Bank7; i++)
            {
                WoWContainer bag = WoWContainer.GetBagByIndex(i);
                if (bag == null || !bag.IsValid) continue;
                for (int x = 0; x < bag.Slots; x++)
                {
                    ulong guid = bag.GetItemGuid(x);
                    if (guid == 0ul) continue;
                    if (guid == Guid)
                    {
                        container = i + 1;
                        slot = x + 1;
                        return true;
                    }
                }
            }
            return false;
        }

        public static IntPtr GetItemRecordPointerFromId(uint id, ulong guid = 0ul)
        {
            if (_getItemInfoBlockById == null)
                _getItemInfoBlockById =
                    Manager.Memory.RegisterDelegate<GetItemInfoBlockByIdDelegate>(
                        (IntPtr) Pointers.WDB.DdItemCache_GetInfoBlockByID);

            return _getItemInfoBlockById((IntPtr) Pointers.WDB.WdbItemCache, id, ref guid);
        }

        public static ItemCacheRecord GetItemRecordFromId(uint id, ulong guid = 0ul)
        {
            IntPtr ptr = GetItemRecordPointerFromId(id, guid);
            if (ptr == IntPtr.Zero)
                return default(ItemCacheRecord);
            return Manager.Memory.Read<ItemCacheRecord>(ptr);
        }

        public static IEnumerable<EquipSlot> GetInventorySlotsByEquipSlot(InventoryType type)
        {
            switch (type)
            {
                case InventoryType.Head:
                    yield return EquipSlot.Head;
                    break;
                case InventoryType.Neck:
                    yield return EquipSlot.Neck;
                    break;
                case InventoryType.Shoulders:
                    yield return EquipSlot.Shoulders;
                    break;
                case InventoryType.Body:
                    yield return EquipSlot.Body;
                    break;
                case InventoryType.Chest:
                case InventoryType.Robe:
                    yield return EquipSlot.Chest;
                    break;
                case InventoryType.Waist:
                    yield return EquipSlot.Waist;
                    break;
                case InventoryType.Legs:
                    yield return EquipSlot.Legs;
                    break;
                case InventoryType.Feet:
                    yield return EquipSlot.Feet;
                    break;
                case InventoryType.Wrists:
                    yield return EquipSlot.Wrists;
                    break;
                case InventoryType.Hands:
                    yield return EquipSlot.Hands;
                    break;
                case InventoryType.Finger:
                    yield return EquipSlot.Finger1;
                    yield return EquipSlot.Finger2;
                    break;
                case InventoryType.Trinket:
                    yield return EquipSlot.Trinket1;
                    yield return EquipSlot.Trinket2;
                    break;
                case InventoryType.Weapon:
                    yield return EquipSlot.MainHand;
                    yield return EquipSlot.OffHand;
                    break;
                case InventoryType.Shield:
                case InventoryType.WeaponOffHand:
                case InventoryType.Holdable:
                    yield return EquipSlot.OffHand;
                    break;
                case InventoryType.Ranged:
                case InventoryType.Thrown:
                case InventoryType.RangedRight:
                case InventoryType.Relic:
                    yield return EquipSlot.Ranged;
                    break;
                case InventoryType.Cloak:
                    yield return EquipSlot.Back;
                    break;
                case InventoryType.TwoHandedWeapon:
                {
                    yield return EquipSlot.MainHand;
                    // TODO: Check for titan's grip
                    //bool flag = Manager.LocalPlayer.Class == WoWClass.Warrior && WoWScript.Execute<int>(InventoryManager.#a(61464), 4u) > 0;
                    //var mainHand = Manager.LocalPlayer.GetEquippedItem(EquipSlot.MainHand);
                    //if (flag && mainHand != null && mainHand.ItemInfo.InventoryType == InventoryType.TwoHandedWeapon)
                    //{
                    //    yield return EquipSlot.OffHand;
                    //}
                    break;
                }
                case InventoryType.Bag:
                case InventoryType.Quiver:
                    yield return EquipSlot.Bag1;
                    yield return EquipSlot.Bag2;
                    yield return EquipSlot.Bag3;
                    yield return EquipSlot.Bag4;
                    break;
                case InventoryType.Tabard:
                    yield return EquipSlot.Tabard;
                    break;
                case InventoryType.WeaponMainHand:
                    yield return EquipSlot.MainHand;
                    break;
            }
        }
    }
}