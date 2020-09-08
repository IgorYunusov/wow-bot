﻿// WoW Radar
// Copyright (C) 2010 John Moore
// 
// Original code by jbrauman of MMOwned.com (http://www.mmowned.com/forums/world-of-warcraft/bots-programs/203457-source-code-wow-radar-application.html)
// MemoryReader.dll is not an original work of John Moore
// 
// This program is not associated with or endorsed by Blizzard Entertainment in any way. 
// World of Warcraft is copyright of Blizzard Entertainment.
//
//
// http://www.programiscellaneous.com/programming-projects/world-of-warcraft/wow-radar/what-is-it/
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningBot{
    //Offsets for 3.3.5 build 12340
    //Thanks to MMOwned.com users

    public enum Globals : int {
        CURR_TARGET_GUID = 0x00BD07B0
    }

    public enum PlayerOffsets : int {
        speed = 0x814,
        movementFieldPtr = 0xD8,
        IsFlyingOffset = 0x44,
        IsFlyingMount_Mask = 0x1000000, 
    }

    public enum ClientOffsets : uint
    {
        StaticClientConnection = 0x00C79CE0,
        ObjectManagerOffset = 0x2ED0,
        FirstObjectOffset = 0xAC,
        LocalGuidOffset = 0xC0,
        NextObjectOffset = 0x3C,
        LocalPlayerGUID = 0xBD07A8,
        LocalTargetGUID = 0x00BD07B0,
        CurrentContinent = 0x00ACCF04
    }

    public enum NameOffsets : ulong
    {
        nameStore = 0x00C5D938 + 0x8,
        nameMask = 0x24,
        nameBase = 0x1C,
        nameString = 0x20
    }

    public enum ObjectOffsets : int
    {
        Type = 0x14,
        Pos_X = 0x79C,
        Pos_Y = 0x798,
        Pos_Z = 0x7A0,
        Rot = 0x7A8,
        Guid = 0x30,
        UnitFields = 0x8,
        Node_Pos_X = 0xEC,
        Node_Pos_Y = 0xE8,
        Node_Pos_Z = 0xF0,
        Target_GUID = 4056,
        Health = 4016
    }

    public enum UnitOffsets : uint
    {
        Level = 0x36 * 4,
        Health = 0x18 * 4,
        Energy = 0x19 * 4,
        MaxHealth = 0x20 * 4,
        SummonedBy = 0xE * 4,
        MaxEnergy = 0x21 * 4
    }

    public enum MineNodes : int
    {
        Copper = 310,
        Tin = 315,
        Incendicite = 384,
        Silver = 314,
        Iron = 312,
        Indurium = 384,
        Gold = 311,
        LesserBloodstone = 48,
        Mithril = 313,
        Truesilver = 314,
        DarkIron = 2571,
        SmallThorium = 3951,
        RichThorium = 3952,
        ObsidianChunk = 6650,
        FelIron = 6799,
        Adamantite = 6798,
        Cobalt = 7881,
        Nethercite = 6650,
        Khorium = 6800,
        Saronite = 7804,
        Titanium = 6798
    }

    public enum HerbNodes : int
    {
        Peacebloom = 269,
        Silverleaf = 270,
        Earthroot = 414,
        Mageroyal = 268,
        Briarthorn = 271,
        Stranglekelp = 700,
        Bruiseweed = 358,
        WildSteelbloom = 371,
        GraveMoss = 357,
        Kingsblood = 320,
        Liferoot = 677,
        Fadeleaf = 697,
        Goldthorn = 698,
        KhadgarsWhisker = 701,
        Wintersbite = 699,
        Firebloom = 2312,
        PurpleLotus = 2314,
        ArthasTears = 2310,
        Sungrass = 2315,
        Blindweed = 2311,
        GhostMushroom = 389,
        Gromsblood = 2313,
        GoldenSansam = 4652,
        Dreamfoil = 4635,
        MountainSilversage = 4633,
        Plaguebloom = 4632,
        Icecap = 4634,
        BlackLotus = 4636,
        Felweed = 6968,
        DreamingGlory = 6948,
        Terocone = 6969,
        Ragveil = 6949,
        FlameCap = 6966,
        AncientLichen = 6967,
        Netherbloom = 6947,
        NightmareVine = 6946,
        ManaThistle = 6945,
        TalandrasRose = 7865,
        Goldclover = 7844,
        AddersTongue = 8084
    }
    public enum WoWGameObjectType : uint
    {
        Door = 0,
        Button = 1,
        QuestGiver = 2,
        Chest = 3,
        Binder = 4,
        Generic = 5,
        Trap = 6,
        Chair = 7,
        SpellFocus = 8,
        Text = 9,
        Goober = 0xa,
        Transport = 0xb,
        AreaDamage = 0xc,
        Camera = 0xd,
        WorldObj = 0xe,
        MapObjTransport = 0xf,
        DuelArbiter = 0x10,
        FishingNode = 0x11,
        Ritual = 0x12,
        Mailbox = 0x13,
        AuctionHouse = 0x14,
        SpellCaster = 0x16,
        MeetingStone = 0x17,
        Unkown18 = 0x18,
        FishingPool = 0x19,
        FORCEDWORD = 0xFFFFFFFF,
    }
}
