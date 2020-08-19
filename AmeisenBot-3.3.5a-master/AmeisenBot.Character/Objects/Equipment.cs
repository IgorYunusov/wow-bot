using AmeisenBot.Character.Enums;
using System.Collections.Generic;

namespace AmeisenBot.Character.Objects
{
    public class Equipment
    {
        public Equipment()
        {
            Update();
        }

        public Item Ammo { get; set; }
        public Item Back { get; set; }
        public Item Chest { get; set; }
        public Item Feet { get; set; }
        public Item Hands { get; set; }
        public Item Head { get; set; }
        public Item Legs { get; set; }
        public Item MainHand { get; set; }
        public Item Necklace { get; set; }
        public Item OffHand { get; set; }
        public Item Ranged { get; set; }
        public Item RingOne { get; set; }
        public Item RingTwo { get; set; }
        public Item Shirt { get; set; }
        public Item Shoulder { get; set; }
        public Item Tabard { get; set; }
        public Item TrinketOne { get; set; }
        public Item TrinketTwo { get; set; }
        public Item Waist { get; set; }
        public Item Wrist { get; set; }

        public IEnumerable<Item> AsList()
        {
            return new List<Item>() { Head, Necklace, Shoulder, Back, Shirt, Tabard, Chest, Hands, Wrist, Waist, Legs, Feet, RingOne, RingTwo, TrinketOne, TrinketTwo, MainHand, OffHand, Ammo };
        }

        public void Update()
        {
            Head = new Item((int)InventorySlot.INVSLOT_HEAD);
            Necklace = new Item((int)InventorySlot.INVSLOT_NECK);
            Shoulder = new Item((int)InventorySlot.INVSLOT_SHOULDER);
            Back = new Item((int)InventorySlot.INVSLOT_BACK);
            Shirt = new Item((int)InventorySlot.INVSLOT_SHIRT);
            Tabard = new Item((int)InventorySlot.INVSLOT_TABARD);
            Chest = new Item((int)InventorySlot.INVSLOT_CHEST);
            Hands = new Item((int)InventorySlot.INVSLOT_HANDS);
            Wrist = new Item((int)InventorySlot.INVSLOT_WRIST);
            Waist = new Item((int)InventorySlot.INVSLOT_WAIST);
            Legs = new Item((int)InventorySlot.INVSLOT_LEGS);
            Feet = new Item((int)InventorySlot.INVSLOT_FEET);
            RingOne = new Item((int)InventorySlot.INVSLOT_RING1);
            RingTwo = new Item((int)InventorySlot.INVSLOT_RING2);
            TrinketOne = new Item((int)InventorySlot.INVSLOT_TRINKET1);
            TrinketTwo = new Item((int)InventorySlot.INVSLOT_TRINKET2);
            MainHand = new Item((int)InventorySlot.INVSLOT_MAINHAND);
            OffHand = new Item((int)InventorySlot.INVSLOT_OFFHAND);
            Ammo = new Item((int)InventorySlot.INVSLOT_AMMO);
            Ranged = new Item((int)InventorySlot.INVSLOT_RANGED);
        }
    }
}