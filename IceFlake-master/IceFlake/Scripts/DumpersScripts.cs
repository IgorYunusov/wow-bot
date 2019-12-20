﻿using System.Collections.Generic;
using System.Linq;
using IceFlake.Client;
using IceFlake.Client.Objects;
using IceFlake.Client.Patchables;
using IceFlake.Client.Scripts;

namespace IceFlake.Scripts
{

    #region UnitDumperScript

    public class UnitDumperScript : Script
    {
        public UnitDumperScript()
            : base("Units", "Dumper")
        {
        }

        public override void OnStart()
        {
            if (!Manager.ObjectManager.IsInGame)
                return;

            foreach (WoWUnit u in Manager.ObjectManager.Objects.Where(x => x.IsUnit).Cast<WoWUnit>())
            {
                Print("-- {0}", u.Name);
                Print("\tGUID: 0x{0}", u.Guid.ToString("X8"));
                Print("\tHealth: {0}/{1} ({2}%)", u.Health, u.MaxHealth, (int) u.HealthPercentage);
                Print("\tReaction: {0}", u.Reaction);
                Print("\tPosition: {0}", u.Location);
            }

            Stop();
        }
    }

    #endregion

    #region PlayerDumperScript

    public class PlayerDumperScript : Script
    {
        public PlayerDumperScript()
            : base("Players", "Dumper")
        {
        }

        public override void OnStart()
        {
            if (!Manager.ObjectManager.IsInGame)
                return;

            foreach (WoWPlayer p in Manager.ObjectManager.Objects.Where(x => x.IsPlayer).Cast<WoWPlayer>())
            {
                Print("-- {0}", p.Name);
                Print("\tGUID: 0x{0}", p.Guid.ToString("X8"));
                Print("\tLevel {0} {1} {2}", p.Level, p.Race, p.Class);
                Print("\tHealth: {0}/{1} ({2}%)", p.Health, p.MaxHealth, (int) p.HealthPercentage);
                Print("\t{0}: {1}/{2} ({3}%)", p.PowerType, p.Power, p.MaxPower, (int) p.PowerPercentage);
                Print("\tPosition: {0}", p.Location);
            }

            Stop();
        }
    }

    #endregion

    #region PartyDumperScript

    public class PartyDumperScript : Script
    {
        public PartyDumperScript()
            : base("Party", "Dumper")
        {
        }

        public override void OnStart()
        {
            if (!Manager.ObjectManager.IsInGame)
                return;

            foreach (WoWPlayer p in WoWParty.Members)
            {
                Print("-- {0}", p.Name);
                Print("\tGUID: 0x{0}", p.Guid.ToString("X8"));
                Print("\tLevel {0} {1} {2}", p.Level, p.Race, p.Class);
                Print("\tHealth: {0}/{1} ({2}%)", p.Health, p.MaxHealth, (int) p.HealthPercentage);
                Print("\tLocation: {0} ({1} yards)", p.Location, p.Distance);
                Print("\tLoS: {0}", p.InLoS);
            }

            //Print("Party:");
            //for (var i = 0; i < 4; i++)
            //    Print("\t{0}", Party.GetPartyMemberGuid(i));

            Stop();
        }
    }

    #endregion

    #region RaidDumperScript

    public class RaidDumperScript : Script
    {
        public RaidDumperScript()
            : base("Raid", "Dumper")
        {
        }

        public override void OnStart()
        {
            if (!Manager.ObjectManager.IsInGame)
                return;

            Print("--- [ RAID ] ---");
            Print("\tInstance Difficulty: {0}", WoWRaid.Difficulty);
            Print("\tRaid Members: {0}", WoWRaid.NumRaidMembers);
            Print("----------------");

            foreach (WoWPlayer p in WoWRaid.Members)
            {
                Print("-- {0}", p.Name);
                Print("\tGUID: 0x{0}", p.Guid.ToString("X8"));
                Print("\tLevel {0} {1} {2}", p.Level, p.Race, p.Class);
                Print("\tHealth: {0}/{1} ({2}%)", p.Health, p.MaxHealth, (int) p.HealthPercentage);
                Print("\tLocation: {0} ({1} yards)", p.Location, p.Distance);
                Print("\tLoS: {0}", p.InLoS);
            }

            //Print("Raid ({0} members):", Raid.NumRaidMembers);
            //for (var i = 0; i < Raid.NumRaidMembers; i++)
            //    Print("\t{0}", Raid.GetRaidMemberGuid(i));

            Stop();
        }
    }

    #endregion

    #region InventoryItemsDumperScript

    public class InventoryItemsDumperScript : Script
    {
        public InventoryItemsDumperScript()
            : base("Inventory Items", "Dumper")
        {
        }

        public override void OnStart()
        {
            if (!Manager.ObjectManager.IsInGame)
                return;

            Print("Inventory Items");
            foreach (WoWItem item in Manager.Inventory.InventoryItems)
            {
                if (item == null || !item.IsValid) continue;

                int x, y;
                if (item.GetSlotIndexes(out x, out y))
                    Print("\t({0},{1}) [{2}]{3}", x, y, item.Name, item.StackCount > 1 ? "x" + item.StackCount : "");
                else
                    Print("\t[{0}]{1}", item.Name, item.StackCount > 1 ? "x" + item.StackCount : "");
                GameError errcode;
                if (Manager.LocalPlayer.CanUseItem(item, out errcode))
                    Print("\tUsable");
                else
                    Print("\tNot usable ({0})", errcode);
            }

            Stop();
        }
    }

    #endregion

    #region EquippedItemsDumperScript

    public class EquippedItemsDumperScript : Script
    {
        public EquippedItemsDumperScript()
            : base("Equipped Items", "Dumper")
        {
        }

        public override void OnStart()
        {
            if (!Manager.ObjectManager.IsInGame)
                return;

            Print("Equipped Items:");
            for (var i = (int) EquipSlot.Head; i < (int) EquipSlot.Tabard + 1; i++)
            {
                WoWItem item = Manager.LocalPlayer.GetEquippedItem(i);
                if (item == null || !item.IsValid) continue;
                Print("[{0}] {1} ({2})", (EquipSlot) i, item.Name, item.Entry);
                ItemCacheRecord itemInfo = item.ItemInfo;
                Print("\tQuality: {0}", itemInfo.Quality);
                Print("\tBonding: {0}", itemInfo.Bonding);
                Print("\tClass: {0}", itemInfo.Class);
                switch (itemInfo.Class)
                {
                    case ItemClass.Armor:
                        Print("\tArmor Class: {0}", (ItemArmorClass) itemInfo.SubClassId);
                        break;
                    case ItemClass.Weapon:
                        Print("\tWeapon Class: {0}", (ItemWeaponClass) itemInfo.SubClassId);
                        break;
                }
                Print("\tStats:");
                foreach (var pair in itemInfo.Stats)
                    Print("\t\t{0}: {1}", pair.Key, pair.Value);
                Print("\tEnchants:");
                foreach (ItemEnchantment e in item.Enchants)
                    Print("\t\t#{0}: {1} {2} {3}", e.Id, e.SpellItemEnchantment.Name, e.Charges, e.Duration);
                Print("\tFits in:");
                foreach (EquipSlot s in WoWItem.GetInventorySlotsByEquipSlot(itemInfo.InventoryType))
                    Print("\t\t{0}", s);
            }

            Stop();
        }
    }

    #endregion

    #region SpellDumperScript

    public class SpellDumperScript : Script
    {
        public SpellDumperScript()
            : base("Spells", "Dumper")
        {
        }

        public override void OnStart()
        {
            if (!Manager.ObjectManager.IsInGame)
                return;

            Print("Spellbook:");
            foreach (WoWSpell spell in Manager.Spellbook)
                Print("#{0}: {1}", spell.Id, spell.Name);

            Stop();
        }
    }

    #endregion

    #region QuestDumperScript

    public class QuestDumperScript : Script
    {
        public QuestDumperScript()
            : base("Quests", "Dumper")
        {
        }

        public override void OnStart()
        {
            if (!Manager.ObjectManager.IsInGame)
                return;

            IEnumerable<int> completedQuests = Manager.Quests.CompletedQuestIds;
            if (completedQuests.Count() == 0)
            {
                Print("Querying server for data... Please run again.");
                Stop();
                return;
            }

            Print("Completed Quests:");
            foreach (int q in completedQuests)
                Print("\t{0}", q);

            Stop();
        }
    }

    #endregion

    #region CameraDumperScript

    public class CameraDumperScript : Script
    {
        public CameraDumperScript()
            : base("Camera", "Dumper")
        {
        }

        public override void OnStart()
        {
            if (!Manager.ObjectManager.IsInGame)
                return;

            CameraInfo camera = Manager.Camera.GetCamera();
            Print("Camera:");
            Print("\tPosition: [{0}]", camera.Position);
            Print("\tNearZ: {0}", camera.NearPlane);
            Print("\tFarZ: {0}", camera.FarPlane);
            Print("\tField of View: {0}", camera.FieldOfView);
            Print("\tAspect Ratio: {0}", camera.Aspect);

            Stop();
        }
    }

    #endregion
}