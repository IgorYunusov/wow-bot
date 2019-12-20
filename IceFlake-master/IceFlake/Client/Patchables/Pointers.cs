﻿namespace IceFlake.Client.Patchables
{
    internal static class Pointers
    {
        #region Nested type: DirectX

        internal class DirectX
        {
            internal uint DirectXBase = 0xC5DF88;
            internal uint Device = 0x397C;
            internal uint EndScene = 0xA8;
        }

        #endregion

        #region Nested type: ObjectManager

        // 3.3.5a: 12340
        internal class ObjectManager
        {
            internal static uint EnumVisibleObjects = 0x004D4B30;
            internal static uint GetObjectByGuid = 0x004D4DB0;
            internal static uint GetLocalPlayerGuid = 0x004D3790;
        }

        #endregion

        #region Nested type: Object

        // 3.3.5a: 12340
        internal class Object
        {
            internal static uint GetObjectName = 54;
            internal static uint GetObjectLocation = 12;
            internal static uint GetObjectFacing = 14;

            internal static uint Interact = 44;
            internal static uint SelectObject = 0x00524BF0;
        }

        #endregion

        #region Nested type: Item

        // 3.3.5a: 12340
        internal class Item
        {
            internal static uint UseItem = 0x00708C20;
            internal static uint CanUseItem = 0x006DC3F0;
        }

        #endregion

        #region Nested type: Container

        // 3.3.5a: 12340
        internal class Container
        {
            internal static uint GetBagAtIndex = 0x005D6F20;
            internal static uint LootWindowOffset = 0x00BFA8D8;
        }

        #endregion

        #region Nested type: Unit

        // 3.3.5a: 12340
        internal class Unit
        {
            internal static uint ChanneledCastingId = 0xA80;
            internal static uint CastingId = 0xA6C;

            internal static uint UpdateDisplayInfo = 0x73e410; // TODO: IMPLEMENT
            internal static uint UnitReaction = 0x007251C0;
            internal static uint HasAuraBySpellId = 0x007282A0;
            internal static uint GetAura = 0x00556E10;
            internal static uint GetAuraCount = 0x004F8850;
            internal static uint GetCreatureType = 0x0071F300;
            internal static uint GetCreatureRank = 0x00718A00;
            internal static uint ShapeshiftFormId = 0x0071AF70;
            internal static uint CalculateThreat = 0x007374C0;
        }

        #endregion

        #region Nested type: LocalPlayer

        // 3.3.5a: 12340
        internal class LocalPlayer
        {
            internal static uint ClickToMove = 0x00727400;
            internal static uint SetFacing = 0x0072EA50;
            internal static uint IsClickMoving = 0x00721F90;
            internal static uint StopCTM = 0x0072B3A0;
            internal static uint CorpsePosition = 0x0051F430;
            internal static uint ComboPoints = 0x00BD084D;
            internal static uint ComboPointsTarget = 0x00BD08A8; // ulong, guid
            internal static uint CompletedQuests = 0x00ACFDF4;

            internal static uint RuneState = 0xC24388;
            internal static uint RuneType = 0xC24304;
            internal static uint RuneCooldown = 0xC24364;
        }

        #endregion

        #region Nested type: Spell

        // 3.3.5a: 12340
        internal class Spell
        {
            internal static uint SpellCount = 0x00BE8D9C;
            internal static uint SpellBook = 0x00BE5D88;
            internal static uint CastSpell = 0x0080DA40;
            internal static uint GetSpellCooldown = 0x00809000;

            internal static uint FirstActionBarSpellId = 0x00C1E358;
                // Don't really need this unless we want to auto-set up actionbars?
        }

        #endregion

        #region Nested type: World

        // 3.3.5a: 12340
        internal class World
        {
            internal static uint Traceline = 0x007A3B70; //  0x0077F310
            internal static uint HandleTerrainClick = 0x00527830;
            internal static uint CurrentMapId = 0x00AB63BC;
            internal static uint InternalMapName = 0x00CE06D0;
            internal static uint ZoneID = 0x00BD080C;
            internal static uint ZoneText = 0x00BD0788;
            internal static uint SubZoneText = 0x00BD0784;
        }

        #endregion

        #region Nested type: LuaInterface

        // 3.3.5a: 12340
        internal class LuaInterface
        {
            internal static uint LuaState = 0x00D3F78C;
            internal static uint LuaLoadBuffer = 0x0084F860;
            internal static uint LuaPCall = 0x0084EC50;
            internal static uint LuaGetTop = 0x0084DBD0;
            internal static uint LuaSetTop = 0x0084DBF0;
            internal static uint LuaType = 0x0084DEB0;
            internal static uint LuaToNumber = 0x0084E030;
            internal static uint LuaToLString = 0x0084E0E0;
            internal static uint LuaToBoolean = 0x0084E0B0;
        };

        #endregion

        #region Nested type: Events

        // 3.3.5a: 12340
        internal class Events
        {
            internal static uint EventVictim = 0x00511C40;
        }

        #endregion

        #region Nested type: DBC

        // 3.3.5a: 12340
        internal class DBC
        {
            internal static uint RegisterBase = 0x006337D0;
            internal static uint GetRow = 0x004BB1C0;
            internal static uint GetLocalizedRow = 0x004CFD20;
        }

        #endregion

        #region Nested type: WDB

        // 3.3.5a: 12340
        internal class WDB
        {
            internal static uint DbWoWCache_GetInfoBlockById = 0x0067FA80;
            internal static uint DdItemCache_GetInfoBlockByID = 0x0067CA30;
            internal static uint DbQuestCache_GetInfoBlockByID = 0x0067DE90;

            internal static uint WdbItemCache = 0x00C5D828;
            internal static uint WdbQuestCache = 0x00C5DA48;
        }

        #endregion

        #region Nested type: Drawing

        // 3.3.5a: 12340
        internal class Drawing
        {
            internal static uint WorldFrame = 0x00B7436C;
            internal static uint ActiveCamera = 0x7E20;
            internal static uint RenderBackground = 0x2532E0; // UPDATE
        }

        #endregion

        #region Nested type: Other

        // 3.3.5a: 12340
        internal class Other
        {
            internal static uint PerformanceCounter = 0x0086AE20;
            internal static uint LastHardwareAction = 0x00B499A4;
            internal static uint IsBobbing = 0xBC;

            internal static uint WorldLoading = 0x00B6AA38;
            internal static uint WorldLoaded = 0x00BEBA40;
            internal static uint GameState = 0x00B6A9E0;
            internal static uint RealmName = 0x00C79B9E;

            // TODO: IMPLEMENT
            internal static uint AHListAuctions = 0xc0f448;
            internal static uint AHListNumAuctions = 0xc0f444;
            internal static uint AHListTotalAuctions = 0xc0f408;
        }

        #endregion

        #region Nested type: Party

        // 3.3.5a: 12340
        internal class Party
        {
            internal static uint PartyArray = 0x00BD1948;
            internal static uint DungeonDifficulty = 0x00BD0898;
        }

        #endregion

        #region Nested type: Raid

        // 3.3.5a: 12340
        internal class Raid
        {
            internal static uint RaidCount = 0x00BEB608;
            internal static uint RaidArray = 0x00BEB568;
            internal static uint RaidDifficulty = 0x00BD089C;
        }

        #endregion

        #region Nested type: Console

        // 3.3.5a: 12340
        internal class Console
        {
            internal static uint Enable = 0x00CABCC4;
            internal static uint WriteA = 0x00765360;
            internal static uint RegisterCommand = 0x00769100;
            internal static uint UnregisterCommand = 0x007689E0;
            internal static uint InvalidPtrCheck = 0x00D415B8;
        }

        #endregion

        #region Nested type: Packets

        // https://github.com/tomrus88/WowAddin/blob/master/WowAddin/CDataStore.cpp
        // 3.3.5a: 12340
        internal class Packets
        {
            internal static uint Initialize = 0x00401050;

            internal static uint PutInt8 = 0x0047AFE0;
            internal static uint PutInt16 = 0x0047B040;
            internal static uint PutInt32 = 0x0047B0A0;
            internal static uint PutInt64 = 0x0047B100;
            internal static uint PutFloat = 0x0047B160;
            internal static uint PutString = 0x0047B300;
            internal static uint PutBytes = 0x0047B1C0;

            internal static uint GetInt8 = 0x0047B340;
            internal static uint GetInt16 = 0x0047B380;
            internal static uint GetInt32 = 0x0047B3C0;
            internal static uint GetInt64 = 0x0047B400;
            internal static uint GetFloat = 0x0047B440;
            internal static uint GetString = 0x0047B480;
            internal static uint GetBytes = 0x0047B560;

            internal static uint Finalize = 0x00401130;
            internal static uint Destroy = 0x00403880;
        }

        #endregion

        #region Nested type: ClientServices

        // 3.3.5a: 12340
        internal class ClientServices
        {
            internal static uint SendPacket = 0x00406F40;
            internal static uint SendPacket2 = 0x00632B50;
            internal static uint GetCurrent = 0x006B0970;
            internal static uint SetMessageHandler = 0x006B0B80;
        }

        #endregion
    }
}
