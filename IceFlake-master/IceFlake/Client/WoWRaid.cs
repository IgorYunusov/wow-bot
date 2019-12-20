﻿using System;
using System.Collections.Generic;
using IceFlake.Client.Objects;
using IceFlake.Client.Patchables;

namespace IceFlake.Client
{
    public static class WoWRaid
    {
        public static int NumRaidMembers
        {
            get { return Manager.Memory.Read<int>((IntPtr) Pointers.Raid.RaidCount); }
        }

        public static IEnumerable<WoWPlayer> Members
        {
            get
            {
                for (int i = 0; i < 40; i++)
                {
                    WoWPlayer unit = GetRaidMember(i);
                    if (unit != null && unit.IsValid)
                        yield return unit;
                }
            }
        }

        public static RaidDifficulty Difficulty
        {
            get { return (RaidDifficulty) Manager.Memory.Read<int>((IntPtr) Pointers.Raid.RaidDifficulty); }
        }

        public static WoWPlayer GetRaidMember(int index)
        {
            return Manager.ObjectManager.GetObjectByGuid(GetRaidMemberGuid(index)) as WoWPlayer;
        }

        public static ulong GetRaidMemberGuid(int index)
        {
            return Manager.Memory.Read<ulong>(Manager.Memory.Read<IntPtr>(new IntPtr(Pointers.Raid.RaidArray + index)));
        }
    }
}