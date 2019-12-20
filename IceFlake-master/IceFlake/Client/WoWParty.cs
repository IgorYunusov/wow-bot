﻿using System;
using System.Collections.Generic;
using IceFlake.Client.Objects;
using IceFlake.Client.Patchables;

namespace IceFlake.Client
{
    public static class WoWParty
    {
        public static int NumPartyMembers
        {
            get
            {
                int ret = 0;
                for (int i = 0; i < 5; i++)
                {
                    if (GetPartyMemberGuid(i) != 0)
                        ret++;
                }
                return ret;
            }
        }

        public static IEnumerable<WoWPlayer> Members
        {
            get
            {
                for (int i = 0; i < 4; i++)
                {
                    WoWPlayer unit = GetPartyMember(i);
                    if (unit != null && unit.IsValid)
                        yield return unit;
                }
            }
        }

        public static DungeonDifficulty Difficulty
        {
            get { return (DungeonDifficulty) Manager.Memory.Read<int>((IntPtr) Pointers.Party.DungeonDifficulty); }
        }

        public static WoWPlayer GetPartyMember(int index)
        {
            return Manager.ObjectManager.GetObjectByGuid(GetPartyMemberGuid(index)) as WoWPlayer;
        }

        public static ulong GetPartyMemberGuid(int index)
        {
            return Manager.Memory.Read<ulong>(new IntPtr(Pointers.Party.PartyArray + index));
        }
    }
}