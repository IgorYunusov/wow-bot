using AmeisenBotLogger;
using AmeisenBotUtilities;
using AmeisenBotUtilities.Enums;
using AmeisenBotUtilities.Structs;
using Magic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace AmeisenBotCore
{
    /// <summary>
    /// Abstract class that contains various static method's to interact with WoW's memory and the
    /// EndScene hook.
    /// </summary>
    public abstract class AmeisenCore
    {
        public static AmeisenHook AmeisenHook { get; set; }
        public static BlackMagic BlackMagic { get; set; }

        public static bool IsFalling => ParseLuaIntResult("isFalling = IsFalling();", "isFalling");
        public static bool IsFlying => ParseLuaIntResult("isFlying = IsFlying();", "isFlying");
        public static bool IsInFlyableArea => ParseLuaIntResult("isFlyableArea = IsFlyableArea();", "isFlyableArea");
        public static bool IsInStealth => ParseLuaIntResult("isStealthed = IsStealthed();", "isStealthed");
        public static bool IsMounted => ParseLuaIntResult("isMounted = IsMounted();", "isMounted");
        public static bool IsOutdoors => ParseLuaIntResult("isOutdoor = IsOutdoors();", "isOutdoor");
        public static bool IsPvPFlagged => ParseLuaIntResult("isPvp = GetPVPDesired();", "isPvp");
        public static bool IsResting => ParseLuaIntResult("isResting = IsResting();", "isResting");
        public static bool IsSwimming => ParseLuaIntResult("isSwimming = IsSwimming();", "isSwimming");

        /// <summary>
        /// Acceptthe  group invite
        /// </summary>
        public static void AcceptGroupInvite()
            => LuaDoString("AcceptGroup();");

        /// <summary>
        /// Accept a resurrection request
        /// </summary>
        public static void AcceptResurrect() 
            => LuaDoString("AcceptResurrect();");

        /// <summary>
        /// Accept a summon request
        /// </summary>
        public static void AcceptSummon() 
            => LuaDoString("ConfirmSummon();");

        /// <summary>
        /// AntiAFK, write the current tick count to lastardwareAction of WoW
        /// </summary>
        public static void AntiAFK() => BlackMagic.WriteInt(Offsets.tickCount, Environment.TickCount);

        /// <summary>
        /// Check if we can attack a specified LuaUnit
        /// </summary>
        /// <param name="luaunit">example: player</param>
        /// <param name="otherluaunit">example: target</param>
        /// <returns>true if we can attack false if not</returns>
        public static bool CanAttack(LuaUnit luaunit, LuaUnit otherluaunit = LuaUnit.player)
            => ParseLuaIntResult($"canAttack = UnitCanAttack({luaunit.ToString()}, {otherluaunit.ToString()});", "canAttack");

        /// <summary>
        /// Check if we can cooperate with a specified LuaUnit
        /// </summary>
        /// <param name="luaunit">example: player</param>
        /// <param name="otherluaunit">example: target</param>
        /// <returns>true if we can cooperate false if not</returns>
        public static bool CanCooperate(LuaUnit luaunit, LuaUnit otherluaunit = LuaUnit.player)
            => ParseLuaIntResult($"canCoop = UnitCanCooperate({luaunit.ToString()}, {otherluaunit.ToString()});", "canCoop");

        /// <summary>
        /// Switch shapeshift forms, use for example "WoWDruid.ShapeshiftForms.Bear"
        /// </summary>
        /// <param name="index">shapeshift index</param>
        public static void CastShapeshift(int index)
        {
            AmeisenLogger.Instance.Log(LogLevel.VERBOSE, $"Casting ShapeshiftForm:{index}", "AmeisenCore");
            LuaDoString($"CastShapeshiftForm(\"{index}\");");
        }

        /// <summary>
        /// Cast a spell by its id, WowApi: CastSpell(spellid);
        /// </summary>
        /// <param name="spellId">spell to cast</param>
        public static void CastSpellById(int spellId)
            => LuaDoString($"CastSpell({spellId});");

        /// <summary>
        /// Cast a spell by its name
        /// </summary>
        /// <param name="spellname">spell to cast</param>
        /// <param name="onMyself">cast spell on myself</param>
        public static void CastSpellByName(string spellname, bool onMyself)
        {
            AmeisenLogger.Instance.Log(LogLevel.VERBOSE, $"Casting Spell:{spellname}", "AmeisenCore");
            if (onMyself)
            {
                LuaDoString($"CastSpellByName(\"{spellname}\", true);");
            }
            else
            {
                LuaDoString($"CastSpellByName(\"{spellname}\");");
            }
        }

        /// <summary>
        /// Let the bot jump by pressing the spacebar once for 20-40ms
        ///
        /// This runs Async.
        /// </summary>
        public static void CharacterJumpAsync()
        {
            AmeisenLogger.Instance.Log(LogLevel.VERBOSE, "Jumping", "AmeisenCore");
            new Thread(CharacterJump).Start();
        }

        /// <summary>
        /// Clear out target if it is friendly and can't be attacked
        /// </summary>
        public static void ClearTargetIfItIsFriendly() 
            => LuaDoString("if not UnitExists(\"target\") or UnitIsFriend(\"player\", \"target\") or UnitIsDead(\"target\") then ClearTarget() end");

        /// <summary>
        /// Clear out target if it is hostile and can be attacked
        /// </summary>
        public static void ClearTargetIfItIsNotFriendly() 
            => LuaDoString("if not UnitExists(\"target\") or not UnitIsFriend(\"player\", \"target\") or UnitIsDead(\"target\") then ClearTarget() end");

        /// <summary>
        /// You are ready
        /// </summary>
        public static void ConfirmReadyCheck()
            => LuaDoString("ConfirmReadyCheck(1);");

        /// <summary>
        /// Decline a ressurrection request
        /// </summary>
        public static void DeclineResurrect() 
            => LuaDoString("DeclineResurrect();");

        /// <summary>
        /// Delicne a summon request
        /// </summary>
        public static void DeclineSummon() 
            => LuaDoString("CancelSummon();");

        /// <summary>
        /// You are not ready
        /// </summary>
        public static void DenyReadyCheck()
            => LuaDoString("ConfirmReadyCheck(0);");

        /// <summary>
        /// Face a unit
        /// </summary>
        /// <param name="me">me, needed for rotation</param>
        /// <param name="unit">unit to face</param>
        public static void FaceUnit(Me me, Unit unit)
        {
            BlackMagic.WriteFloat(me.BaseAddress + 0x7A8, Utils.GetFacingAngle(me.pos, me.Rotation, unit.pos));
            //LuaDoString("MoveBackwardStart();MoveBackwardStop();MoveBackwardStop();MoveBackwardStop();"); This is trash lel
            SendKey(new IntPtr(0x41), 0, 0); // the "S" key to go a bit backwards TODO: find better method 0x53
        }

        /// <summary>
        /// Reads all WoWObject out of WoW's ObjectManager
        /// </summary>
        /// <returns>all WoWObjects as a List</returns>
        public static List<WowObject> GetAllWoWObjects()
        {
            List<WowObject> objects = new List<WowObject>();

            try
            {
                uint currentObjectManager = BlackMagic.ReadUInt(Offsets.currentClientConnection);
                currentObjectManager = BlackMagic.ReadUInt(currentObjectManager + Offsets.currentManagerOffset);

                uint activeObject = BlackMagic.ReadUInt(currentObjectManager + Offsets.firstObjectOffset);
                uint objectType = BlackMagic.ReadUInt(activeObject + Offsets.gameobjectTypeOffset);

                // loop through the objects until an object is bigger than 7 or lower than 1 to get all
                // Objects from manager
                while (objectType <= 7 && objectType > 0)
                {
                    WowObject wowObject = ReadWoWObjectFromWoW(activeObject, (WowObjectType)objectType);
                    wowObject.MapId = GetMapId();
                    wowObject.ZoneId = GetZoneId();
                    objects.Add(wowObject);

                    activeObject = BlackMagic.ReadUInt(activeObject + Offsets.nextObjectOffset);
                    objectType = BlackMagic.ReadUInt(activeObject + Offsets.gameobjectTypeOffset);
                }
            }
            catch { AmeisenLogger.Instance.Log(LogLevel.ERROR, "Failed to read WowObjects...", "AmeisenCore"); }

            return objects;
        }

        /// <summary>
        /// Check for Auras/Buffs
        /// </summary>
        /// <param name="luaUnit">LuaUnit to get the Auras of</param>
        /// <returns>returns unit Auras as string list</returns>
        public static List<string> GetAuras(LuaUnit luaUnit)
        {
            List<string> result = new List<string>(GetBuffs(luaUnit));
            result.AddRange(GetDebuffs(luaUnit));
            return result;
        }

        /// <summary>
        /// Check for Buffs
        /// </summary>
        /// <param name="luaUnit">LuaUnit to get the Buffs of</param>
        /// <returns>returns unit Buffs as string list</returns>
        public static List<string> GetBuffs(LuaUnit LuaUnit)
        {
            List<string> resultLowered = new List<string>();
            StringBuilder cmdBuffs = new StringBuilder();
            cmdBuffs.Append("local buffs, i = { }, 1;");
            cmdBuffs.Append($"local buff = UnitBuff(\"{LuaUnit.ToString()}\", i);");
            cmdBuffs.Append("while buff do\n");
            cmdBuffs.Append("buffs[#buffs + 1] = buff;");
            cmdBuffs.Append("i = i + 1;");
            cmdBuffs.Append($"buff = UnitBuff(\"{LuaUnit.ToString()}\", i);");
            cmdBuffs.Append("end;");
            cmdBuffs.Append("if #buffs < 1 then\n");
            cmdBuffs.Append("buffs = \"\";");
            cmdBuffs.Append("else\n");
            cmdBuffs.Append("activeUnitBuffs = table.concat(buffs, \", \");");
            cmdBuffs.Append("end;");
            string[] buffs = GetLocalizedText(cmdBuffs.ToString(), "activeUnitBuffs").Split(',');

            foreach (string s in buffs)
            {
                resultLowered.Add(s.Trim().ToLower());
            }

            return resultLowered;
        }

        /// <summary>
        /// Returns the current combat state
        /// </summary>
        /// <param name="LuaUnit">LuaUnit to check</param>
        /// <returns>true if unit is in combat, false if not</returns>
        public static bool GetCombatState(LuaUnit LuaUnit)
            => ParseLuaIntResult($"affectingCombat = UnitAffectingCombat(\"{LuaUnit.ToString()}\");", "affectingCombat");

        /// <summary>
        /// Get our active Corpse position
        /// </summary>
        /// <returns>corpse position</returns>
        public static Vector3 GetCorpsePosition() => new Vector3
        (
            BlackMagic.ReadFloat(Offsets.corpseX),
            BlackMagic.ReadFloat(Offsets.corpseY),
            BlackMagic.ReadFloat(Offsets.corpseZ)
        );

        /// <summary>
        /// Check for Debuffs
        /// </summary>
        /// <param name="luaUnit">LuaUnit to get the Debuffs of</param>
        /// <returns>returns unit Debuffs as string list</returns>
        public static List<string> GetDebuffs(LuaUnit LuaUnit)
        {
            List<string> resultLowered = new List<string>();
            StringBuilder cmdDebuffs = new StringBuilder();
            cmdDebuffs.Append("local buffs, i = { }, 1;");
            cmdDebuffs.Append($"local buff = UnitDebuff(\"{LuaUnit.ToString()}\", i);");
            cmdDebuffs.Append("while buff do\n");
            cmdDebuffs.Append("buffs[#buffs + 1] = buff;");
            cmdDebuffs.Append("i = i + 1;");
            cmdDebuffs.Append($"buff = UnitDebuff(\"{LuaUnit.ToString()}\", i);");
            cmdDebuffs.Append("end;");
            cmdDebuffs.Append("if #buffs < 1 then\n");
            cmdDebuffs.Append("buffs = \"\";");
            cmdDebuffs.Append("else\n");
            cmdDebuffs.Append("activeUnitDebuffs = table.concat(buffs, \", \");");
            cmdDebuffs.Append("end;");
            string[] debuffs = GetLocalizedText(cmdDebuffs.ToString(), "activeUnitDebuffs").Split(',');

            foreach (string s in debuffs)
            {
                resultLowered.Add(s.Trim().ToLower());
            }

            return resultLowered;
        }

        /// <summary>
        /// Get Localized Text for command aka. read a lua variable
        /// </summary>
        /// <param name="command">lua command to run</param>
        /// <param name="variable">variable to read</param>
        /// <returns>
        /// Localized text for the executed functions
        /// return value, variable content or whatever
        /// you want to read.
        /// </returns>
        public static string GetLocalizedText(string command, string variable)
        {
            if (command.Length > 0 && variable.Length > 0)
            {
                // allocate memory for our command
                uint argCCCommand = BlackMagic.AllocateMemory(Encoding.UTF8.GetBytes(command).Length + 1);
                BlackMagic.WriteBytes(argCCCommand, Encoding.UTF8.GetBytes(command));

                string[] asmDoString = new string[]
                {
                    $"MOV EAX, {(argCCCommand) }",
                    "PUSH 0",
                    "PUSH EAX",
                    "PUSH EAX",
                    $"CALL {(Offsets.luaDoString)}",
                    "ADD ESP, 0xC",
                    "RETN",
                };

                // allocate memory for our variable
                uint argCC = BlackMagic.AllocateMemory(Encoding.UTF8.GetBytes(variable).Length + 1);
                BlackMagic.WriteBytes(argCC, Encoding.UTF8.GetBytes(variable));

                string[] asmLocalText = new string[]
                {
                    $"CALL {(Offsets.clientObjectManagerGetActivePlayerObject)}",
                    "MOV ECX, EAX",
                    "PUSH -1",
                    $"PUSH {(argCC)}",
                    $"CALL {(Offsets.luaGetLocalizedText)}",
                    "RETN",
                };

                HookJob hookJobLocaltext = new HookJob(asmLocalText, true);
                ReturnHookJob hookJobDoString = new ReturnHookJob(asmDoString, false, hookJobLocaltext);

                // add our hook-job to be executed
                AmeisenHook.AddHookJob(ref hookJobDoString);

                // wait for our hook-job to return
                while (!hookJobDoString.IsFinished) { Thread.Sleep(1); }

                // parse the result bytes to a readable string
                string result = Encoding.UTF8.GetString((byte[])hookJobDoString.ReturnValue);
                AmeisenLogger.Instance.Log(LogLevel.VERBOSE, "DoString(" + command + "); => " + variable + " = " + result, "AmeisenCore");

                // free our memory
                BlackMagic.FreeMemory(argCCCommand);
                BlackMagic.FreeMemory(argCC);
                return result;
            }
            return "";
        }

        /// <summary>
        /// Get our current MapID, example: 0 = Eastern Kingdoms, 1 = Kalimdor
        /// </summary>
        /// <returns>the mapid currently loaded</returns>
        public static int GetMapId() => BlackMagic.ReadInt(Offsets.mapId);

        /// <summary>
        /// Run through the WoWObjects and find the
        /// BaseAdress of the given GUID's object
        /// </summary>
        /// <param name="guid">guid to search for</param>
        /// <param name="woWObjects">all active WowObjects</param>
        /// <returns>BaseAdress of the WoWObject</returns>
        public static uint GetMemLocByGUID(ulong guid, List<WowObject> woWObjects)
        {
            AmeisenLogger.Instance.Log(LogLevel.VERBOSE, $"Reading: GUID [{guid}]", "AmeisenCore");

            if (woWObjects != null)
            {
                foreach (WowObject obj in woWObjects)
                {
                    if (obj != null && obj.Guid == guid)
                    {
                        return obj.BaseAddress;
                    }
                }
            }

            return 0;
        }

        /// <summary>
        /// Returns the running WoW's in a WoWExe List containing the
        /// logged in playername and Process object.
        /// </summary>
        /// <returns>A list containing all the runnign WoW processes</returns>
        public static List<WowExe> GetRunningWows()
        {
            List<WowExe> wows = new List<WowExe>();
            List<Process> processList = new List<Process>(Process.GetProcessesByName("Wow"));

            foreach (Process p in processList)
            {
                AmeisenLogger.Instance.Log(LogLevel.DEBUG, $"Found WoW Process! PID: {p.Id}", "AmeisenCore");

                BlackMagic blackmagic = new BlackMagic(p.Id);
                uint pDevice = blackmagic.ReadUInt(Offsets.devicePtr1);
                uint pEnd = blackmagic.ReadUInt(pDevice + Offsets.devicePtr2);
                uint pScene = blackmagic.ReadUInt(pEnd);
                uint endscene = blackmagic.ReadUInt(pScene + Offsets.endScene);

                bool isAlreadyHooked = false;
                try
                {
                    isAlreadyHooked = BlackMagic.ReadByte(endscene + 0x2) == 0xE9;
                }
                catch { }

                string name = blackmagic.ReadASCIIString(Offsets.playerName, 12);
                if (name == "")
                {
                    name = "not logged in";
                }

                wows.Add(new WowExe
                {
                    characterName = name,
                    process = p,
                    alreadyHooked = isAlreadyHooked
                });
                blackmagic.Close();
            }

            return wows;
        }

        /// <summary>
        /// Get some information about a certain spell by its name
        /// </summary>
        /// <param name="spell">spellname</param>
        /// <returns>SpellInfo containing spellName, castTime and cost</returns>
        public static SpellInfo GetSpellInfo(string spell)
        {
            SpellInfo info = new SpellInfo();
            string cmd = $"_, _, _, cost, _, _, castTime, _ = GetSpellInfo(\"{spell}\"); spellInfo = cost..\"|\"..castTime;";
            string str = GetLocalizedText(cmd, "spellInfo");

            info.name = spell;
            info.castTime = Utils.TryParseInt(str.Split('|')[0]);
            info.cost = Utils.TryParseInt(str.Split('|')[1]);

            AmeisenLogger.Instance.Log(LogLevel.DEBUG, $"[SpellInfo] Name: {info.name}, Cost: {info.cost}, CastTime: {info.castTime}s", "AmeisenCore");
            return info;
        }

        /// <summary>
        /// Get Units casting Info, example check if we can interrupt it or something alike
        /// </summary>
        /// <param name="player">LuaUnit to check</param>
        /// <returns>CastingInfo containing spellname, castEndTime and canInterrupt</returns>
        public static CastingInfo GetUnitCastingInfo(LuaUnit luaunit)
        {
            CastingInfo info = new CastingInfo();
            string cmd = $"abCastingInfo = \"none,0\"; abSpellName, x, x, x, x, abSpellEndTime = UnitCastingInfo(\"{luaunit.ToString()}\"); abDuration = ((abSpellEndTime/1000) - GetTime()) * 1000; abCastingInfo = abSpellName..\",\"..abDuration;";
            string str = GetLocalizedText(cmd, "abCastingInfo");

            info.name = str.Split(',')[0];
            info.duration = Utils.TryParseInt(str.Split(',')[1]);

            AmeisenLogger.Instance.Log(LogLevel.DEBUG, $"[CastingInfo] Name: {info.name}, Duration: {info.duration} ms", "AmeisenCore");
            return info;
        }

        /// <summary>
        /// Get the reaction of unit a to unit b
        /// </summary>
        /// <param name="luaunit">unit a example: player</param>
        /// <param name="otherluaunit">unit b example: target</param>
        /// <returns>the reaction of unit a towards b</returns>
        public static UnitReaction GetUnitReaction(LuaUnit luaunit, LuaUnit otherluaunit = LuaUnit.player)
        {
            try
            {
                string cmd = $"reaction = UnitReaction(\"{luaunit.ToString()}\", \"{otherluaunit.ToString()}\");";
                return (UnitReaction)int.Parse(GetLocalizedText(cmd, "reaction"));
            }
            catch { return UnitReaction.NONE; }
        }

        /// <summary>
        /// Returns WoW's window size as a native RECT struct by a given windowHandle
        /// </summary>
        /// <param name="mainWindowHandle">WoW's windowHandle</param>
        /// <returns>WoW's window size as a RECT</returns>
        public static SafeNativeMethods.Rect GetWowDiemsions(IntPtr mainWindowHandle)
        {
            SafeNativeMethods.Rect rect = new SafeNativeMethods.Rect();
            SafeNativeMethods.GetWindowRect(mainWindowHandle, ref rect);
            return rect;
        }

        /// <summary>
        /// Get our active ZoneID, example: 12 for Northshire
        /// </summary>
        /// <returns>zoneid that we're currently in</returns>
        public static int GetZoneId() 
            => BlackMagic.ReadInt(Offsets.zoneId);

        /// <summary>
        /// Move the player to the given guid npc, object or whatever and iteract with it.
        /// </summary>
        /// <param name="pos">Vector3 containing the position to interact with</param>
        /// <param name="guid">guid of the entity</param>
        /// <param name="action">CTM Interaction to perform</param>
        public static void InteractWithGUID(Vector3 pos, ulong guid, InteractionType action)
        {
            AmeisenLogger.Instance.Log(LogLevel.DEBUG, $"Interacting[{action}]: X [{pos.X}] Y [{pos.Y}] Z [{pos.Z}] GUID [{guid}]", "AmeisenCore");
            BlackMagic.WriteUInt64(Offsets.ctmGuid, guid);
            MovePlayerToXYZ(pos, action);
        }

        /// <summary>
        /// returns true if the LuaUnit is dead
        /// </summary>
        /// <param name="LuaUnit">LuaUnit to read the data from</param>
        /// <returns>is LuaUnit dead</returns>
        public static bool IsDead(LuaUnit LuaUnit)
            => ParseLuaIntResult($"isDead = UnitIsDead(\"{LuaUnit.ToString()}\");", "isDead");

        /// <summary>
        /// returns true if the selected LuaUnit is a ghost or dead
        /// </summary>
        /// <param name="LuaUnit">LuaUnit to read the data from</param>
        /// <returns>is LuaUnit a ghost or dead</returns>
        public static bool IsDeadOrGhost(LuaUnit LuaUnit)
            => ParseLuaIntResult($"isDeadOrGhost = UnitIsDeadOrGhost(\"{LuaUnit.ToString()}\");", "isDeadOrGhost");

        /// <summary>
        /// Returns true or false, wether the Target is hostile or not
        /// </summary>
        /// <param name="luaunit">example: player</param>
        /// <param name="otherluaunit">example: target</param>
        /// <returns>true if unit is hostile, false if not</returns>
        public static bool IsEnemy(LuaUnit luaunit, LuaUnit otherluaunit = LuaUnit.player)
            => ParseLuaIntResult($"isEnemy = UnitIsEnemy({luaunit.ToString()}, {otherluaunit.ToString()});", "isEnemy");

        /// <summary>
        /// Returns true or false, wether the Target is friendly or not
        /// </summary>
        /// <returns>true if unit is friendly, false if not</returns>
        public static bool IsFriend(LuaUnit luaunit, LuaUnit otherluaunit = LuaUnit.player)
            => ParseLuaIntResult($"isFriendly = UnitIsFriend({luaunit.ToString()}, {otherluaunit.ToString()});", "isFriendly");

        /// <summary>
        /// returns true if the LuaUnit is a ghost
        /// </summary>
        /// <param name="LuaUnit">LuaUnit to read the data from</param>
        /// <returns>is LuaUnit a ghost notk</returns>
        public static bool IsGhost(LuaUnit LuaUnit)
            => ParseLuaIntResult($"isGhost = UnitIsDeadOrGhost(\"{LuaUnit.ToString()}\");", "isGhost");

        /// <summary>
        /// Check if the player's world is in a loadingscreen 
        /// basiccaly inverting the output of IsWorldLoaded();
        /// 
        /// We need to stop object updates etc. in loading screens.
        /// 
        /// HooksJobs can't happen in loadingscreens either because
        /// the hooked method wont update in there.
        /// </summary>
        /// <returns>true if yes, false if no</returns>
        public static bool IsInLoadingScreen() => !IsWorldLoaded();

        /// <summary>
        /// Check if the spell is on cooldown
        /// </summary>
        /// <param name="spell">spellname</param>
        /// <returns>true if it is on cooldown, false if not</returns>
        public static bool IsOnCooldown(string spell)
            => ParseLuaIntResult($"start, abDuration, enabled = GetSpellCooldown(\"{spell}\");", "abDuration");

        /// <summary>
        /// Check if we know a spell based on its name
        /// </summary>
        /// <param name="spellId"></param>
        /// <returns>true if we know it false if not</returns>
        public static bool IsSpellKnown(int spellId)
             => ParseLuaIntResult($"abIsKnown = GetSpellInfo({spellId}); if abIsKnown then abIsKnown = 1 else abIsKnown = 0 end;", "abIsKnown");

        /// <summary>
        /// Checks wether you can or can't cast the specific spell right now
        /// </summary>
        /// <param name="name"></param>
        /// <returns>true if we can it false if not</returns>
        public static bool IsSpellUseable(string spellname)
            => ParseLuaIntResult($"usable, nomana = IsUsableSpell(\"{spellname}\");if usable then resultUseable = 1 else resultUseable = 0 end;", "resultUseable");

        /// <summary>
        /// Check if the player's world is loaded
        /// </summary>
        /// <returns>true if yes, false if no</returns>
        public static bool IsWorldLoaded() 
            => BlackMagic.ReadInt(Offsets.worldLoaded) == 1;

        /// <summary>
        /// Throw the two vendor out of your mammoth, can be used to kick
        /// players out of your mammoth, chopper to
        /// </summary>
        public static void KickNpcsOutOfMammoth() 
            => LuaDoString("for i = 1, 2 do EjectPassengerFromSeat(i) end");

        /// <summary>
        /// Loot all items that you can
        /// </summary>
        public static void LootEveryThing()
            => LuaDoString("abLootCount=GetNumLootItems();for i = abLootCount,1,-1 do LootSlot(i); ConfirmLootSlot(i); end");
        
        /// <summary>
        /// Execute the given LUA command inside WoW's MainThread
        /// </summary>
        /// <param name="command">lua command to run</param>
        public static void LuaDoString(string command)
        {
            AmeisenLogger.Instance.Log(LogLevel.VERBOSE, $"Doing string: Command [{command}]", "AmeisenCore");
            // reserve memory for our command and write its bytes to the memory
            uint argCC = BlackMagic.AllocateMemory(Encoding.UTF8.GetBytes(command).Length + 1);
            BlackMagic.WriteBytes(argCC, Encoding.UTF8.GetBytes(command));

            string[] asm = new string[]
            {
                $"MOV EAX, {(argCC)}",
                "PUSH 0",
                "PUSH EAX",
                "PUSH EAX",
                $"CALL {(Offsets.luaDoString)}",
                "ADD ESP, 0xC",
                "RETN",
            };

            // add our hook job to be executed on hook
            HookJob hookJob = new HookJob(asm, false);
            AmeisenHook.AddHookJob(ref hookJob);

            // wait for our hook to return
            while (!hookJob.IsFinished) { Thread.Sleep(1); }

            AmeisenLogger.Instance.Log(LogLevel.VERBOSE, $"Command returned: Command [{command}]", "AmeisenCore");
            BlackMagic.FreeMemory(argCC); // free our codecaves memory
        }

        /// <summary>
        /// Get onto a random mount that you supply.
        /// it will prefer the flying mount if flying is possible
        /// </summary>
        /// <param name="landMounts">example: "Horse,Chopper"</param>
        /// <param name="flyingMounts">example: "Bird,Helicopter"</param>
        public static void MountRandomMount(string landMounts, string flyingMounts = "")
        {
            if (landMounts.Length > 0 || flyingMounts.Length > 0)
            {
                StringBuilder cmd = new StringBuilder();
                cmd.Append("/castrandom ");
                if (flyingMounts.Length > 0)
                {
                    cmd.Append($"[flyable] {flyingMounts} ; ");
                }
                if (landMounts.Length > 0)
                {
                    cmd.Append($"{landMounts} ;");
                }
                RunSlashCommand(cmd.ToString());
            }
        }

        /// <summary>
        /// Try to move right or left random, used for "unstuck"
        /// </summary>
        public static void MoveLeftRight()
        {
            if (new Random().Next(0, 2) == 1)
            {
                // the "A" key to go a bit Left TODO: find better method
                SendKey(new IntPtr(0x51), 60, 380);
            }
            else
            {
                // the "D" key to go a bit Right TODO: find better method
                SendKey(new IntPtr(0x45), 60, 380);
            }
        }

        /// <summary>
        /// Move the Player to the given x, y and z coordinates
        /// using memory-writing to the CTM variables
        /// </summary>
        /// <param name="pos">Vector3 containing the position to go to</param>
        /// <param name="action">CTM Interaction to perform</param>
        /// <param name="distance">distance how close to move to the position</param>
        public static void MovePlayerToXYZ(Vector3 pos, InteractionType action, double distance = 1.5)
        {
            AmeisenLogger.Instance.Log(LogLevel.VERBOSE, $"Moving to: X [{pos.X}] Y [{pos.Y}] Z [{pos.Z}]", "AmeisenCore");
            WriteXYZToMemory(pos, action, (float)distance);
        }

        /// <summary>
        /// Get the bot's char's GUID
        /// </summary>
        /// <returns>the GUID</returns>
        public static ulong ReadPlayerGUID() 
            => BlackMagic.ReadUInt64(Offsets.localPlayerGuid);

        /// <summary>
        /// Read the items name that is currently beeing rolled for
        /// </summary>
        /// <param name="id">item id supplied by the roll event</param>
        /// <returns>RollInfo containing name, count and quality</returns>
        public static RollInfo ReadRollItemName(string id)
        {
            RollInfo info = new RollInfo();
            string cmd = $"rollInfo = \"none|0|0\"; _, abItemName, abItemCount, abItemQuality = GetLootRollItemInfo({id}); rollInfo = abItemName..\"|\"..abItemCount..\"|\"..abItemQuality;";
            string str = GetLocalizedText(cmd, "rollInfo");

            info.name = str.Split('|')[0];
            info.count = Utils.TryParseInt(str.Split('|')[1]);
            info.quality = Utils.TryParseInt(str.Split('|')[2]);

            AmeisenLogger.Instance.Log(LogLevel.DEBUG, $"[RollInfo] Name: {info.name}, Count: {info.count}, Quality: {info.quality}", "AmeisenCore");
            return info;
        }

        /// <summary>
        /// Get the bot's char's target's GUID
        /// </summary>
        /// <returns>targets guid</returns>
        public static ulong ReadTargetGUID() 
            => BlackMagic.ReadUInt64(Offsets.localTargetGuid);

        /// <summary>
        /// Read WoWObject from WoW's memory by its BaseAddress
        /// </summary>
        /// <param name="baseAddress">baseAddress of the object</param>
        /// <param name="woWObjectType">objectType of the object</param>
        /// <returns>the WoWObject</returns>
        public static WowObject ReadWoWObjectFromWoW(uint baseAddress, WowObjectType woWObjectType)
        {
            //AmeisenLogger.Instance.Log(LogLevel.VERBOSE, $"Reading: baseAddress [{baseAddress}]", "AmeisenCore");

            if (baseAddress == 0)
            {
                return null;
            }

            switch (woWObjectType)
            {
                case WowObjectType.CONTAINER:
                    return new Container(baseAddress, BlackMagic);

                case WowObjectType.ITEM:
                    return new Item(baseAddress, BlackMagic);

                case WowObjectType.GAMEOBJECT:
                    return new GameObject(baseAddress, BlackMagic);

                case WowObjectType.DYNOBJECT:
                    return new DynObject(baseAddress, BlackMagic);

                case WowObjectType.CORPSE:
                    return new Corpse(baseAddress, BlackMagic);

                case WowObjectType.PLAYER:
                    Player obj = new Player(baseAddress, BlackMagic);

                    if (obj.Guid == ReadPlayerGUID())
                    {
                        // thats me
                        return new Me(baseAddress, BlackMagic);
                    }

                    return obj;

                case WowObjectType.UNIT:
                    return new Unit(baseAddress, BlackMagic);

                default:
                    break;
            }
            return null;
        }


        /// <summary>
        /// Repair all items by calling WoWApi:"RepopMe()";
        /// </summary>
        public static void ReleaseSpirit() 
            => LuaDoString("RepopMe();");

        /// <summary>
        /// Repair all items by calling WoWApi:"RepairAllItems()";
        /// </summary>
        public static void RepairAllItems() 
            => LuaDoString("RepairAllItems();");

        /// <summary>
        /// Wait until we can recover our corpse and revive our character
        /// </summary>
        /// <param name="checkAndWaitForCorpseDelay">check for remaining ressurection wait time</param>
        public static void RetrieveCorpse(bool checkAndWaitForCorpseDelay = true)
        {
            if (checkAndWaitForCorpseDelay)
            {
                int corpseDelay = int.Parse(
                GetLocalizedText($"corpseDelay = GetCorpseRecoveryDelay();", "corpseDelay"));
                Thread.Sleep((corpseDelay * 1000) + 100);
            }

            LuaDoString("RetrieveCorpse();");
        }

        /// <summary>
        /// Roll on loot dropped
        /// </summary>
        /// <param name="slot">lootdrop slot, you can obtain this via the event args</param>
        /// <param name="lootRoll">what to roll on the item, need, greed...</param>
        public static void RollOnLoot(int slot, LootRoll lootRoll)
        {
            //LuaDoString($"local b = _G[\"GroupLootFrame\"..{slot}].{lootRoll.ToString()} if b:IsVisible() then b:Click() StaticPopup1Button1:Click() end");
        }

        /// <summary>
        /// Run the given slash-commando
        /// </summary>
        /// <param name="slashCommand">Example: /target player</param>
        public static void RunSlashCommand(string slashCommand)
            => LuaDoString($"DEFAULT_CHAT_FRAME.editBox:SetText(\"{slashCommand}\") ChatEdit_SendText(DEFAULT_CHAT_FRAME.editBox, 0)");

        /// <summary>
        /// Sell all gray item from your bag
        /// </summary>
        public static void SellAllGrayItems() 
            => LuaDoString("local p,N,n=0 for b=0,4 do for s=1,GetContainerNumSlots(b) do n=GetContainerItemLink(b,s) if n and string.find(n,\"9d9d9d\") then N={GetItemInfo(n)} p=p+N[11] UseContainerItem(b,s) print(\"Sold: \"..n) end end end print(\"Total: \"..GetCoinText(p))");

        /// <summary>
        /// Set WoW's window position and dimensions
        /// </summary>
        /// <param name="mainWindowHandle">WoW's windowHandle</param>
        /// <param name="x">x position on screen</param>
        /// <param name="y">y position on screen</param>
        /// <param name="width">window width</param>
        /// <param name="height">window height</param>
        public static void SetWindowPosition(IntPtr mainWindowHandle, int x, int y, int width, int height)
            => SafeNativeMethods.MoveWindow(mainWindowHandle, x, y, height, width, true);

        /// <summary>
        /// Target a GUID by calling WoW's clientGameUITarget function
        /// </summary>
        /// <param name="guid">guid to target</param>
        public static void TargetGUID(ulong guid, [CallerMemberName]string functionName = "")
        {
            AmeisenLogger.Instance.Log(LogLevel.DEBUG, $"TargetGUID: {guid}", "AmeisenCore", functionName);
            //BlackMagic.WriteUInt64(Offsets.localTargetGuid, guid);

            byte[] guidBytes = BitConverter.GetBytes(Convert.ToUInt64(guid));
            string[] asm = new string[]
            {
                $"PUSH {BitConverter.ToUInt32(guidBytes, 4)}",
                $"PUSH {BitConverter.ToUInt32(guidBytes, 0)}",
                $"CALL {Offsets.clientGameUITarget}",
                "ADD ESP, 0x8",
                "RETN"
            };

            // add our hook-job to process it
            HookJob hookJob = new HookJob(asm, false);
            AmeisenHook.AddHookJob(ref hookJob);

            // wait for the hook-job to return to us
            while (!hookJob.IsFinished) { Thread.Sleep(1); }
        }

        /// <summary>
        /// Target specific LuaUnit for example player, party1-5, raid1-40...
        /// </summary>
        /// <param name="unit">LuaUnit to target</param>
        public static void TargetLuaUnit(LuaUnit unit) 
            => LuaDoString($"TargetUnit(\"{unit.ToString()}\");");

        /// <summary>
        /// Target a Unit by its name, for example you know that the
        /// vendor in you mammoth is and will always be called "Gnimo"
        /// then its easier to just hardcode the name so we dont have to
        /// dig throught all the active objects ti find it.
        /// 
        /// Evend partial names can be used if you set the second parameter
        /// to true.
        /// </summary>
        /// <param name="name">name of the unit to target</param>
        /// <param name="exactMatch">if the name have to be an exact match</param>
        public static void TargetUnitByName(string name, bool exactMatch = true) => LuaDoString($"TargetUnit(\"{name}\", {(exactMatch ? "true" : "false")});");

        internal static void EnableAutoBoPConfirm()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("abAutoBoPFrame = CreateFrame(\"Frame\", \"ABot\");");
            sb.Append("abAutoBoPFrame:RegisterEvent(\"CONFIRM_LOOT_ROLL\");");
            sb.Append("abAutoBoPFrame:SetScript(\"OnEvent\", function(self,event,...) ");
            sb.Append("if event == \"CONFIRM_LOOT_ROLL\" then ");
            sb.Append("RollID = select(1, ...);");
            sb.Append("roll = select(2, ...);");
            sb.Append("ConfirmLootRoll( RollID, roll );");
            sb.Append("end;");
            sb.Append("end);");

            LuaDoString(sb.ToString());
        }

        /// <summary>
        /// Let the Character Jump by sending a spacebar key to the game
        /// </summary>
        private static void CharacterJump() 
            => SendKey(new IntPtr(0x20));

        private static bool ParseLuaIntResult(string command, string outputVariable)
        {
            try
            {
                return int.Parse(GetLocalizedText(command, outputVariable)) > 0;
            }
            catch { return false; }
        }

        /// <summary>
        /// Hold WoW's main thread, be careful things get dangerous here
        /// </summary>
        private static void PauseMainThread()
        => SThread.SuspendThread(
              SThread.OpenThread(
                  SThread.GetMainThread(BlackMagic.ProcessId).Id));

        /// <summary>
        /// Resumes WoW's main thread
        /// </summary>
        private static void ResumeMainthread()
        => SThread.ResumeThread(
              SThread.OpenThread(
                  SThread.GetMainThread(BlackMagic.ProcessId).Id));

        /// <summary>
        /// Send a vKey to WoW example: "0x20" for Spacebar (VK_SPACE)
        /// </summary>
        /// <param name="vKey">virtual key id</param>
        private static void SendKey(IntPtr vKey, int minDelay = 20, int maxDelay = 40)
        {
            const uint KEYDOWN = 0x100;
            const uint KEYUP = 0x101;

            IntPtr windowHandle = BlackMagic.WindowHandle;

            // 0x20 = Spacebar (VK_SPACE)
            SafeNativeMethods.SendMessage(windowHandle, KEYDOWN, vKey, new IntPtr(0));
            Thread.Sleep(new Random().Next(minDelay, maxDelay)); // make it look more human-like :^)
            SafeNativeMethods.SendMessage(windowHandle, KEYUP, vKey, new IntPtr(0));
        }

        /// <summary>
        /// Write the coordinates and action to the memory.
        /// </summary>
        /// <param name="pos">Vector3 containing the position to go to</param>
        /// <param name="action">CTM Interaction to perform</param>
        private static void WriteXYZToMemory(Vector3 pos, InteractionType action, float distance = 1.5f)
        {
            AmeisenLogger.Instance.Log(LogLevel.VERBOSE, $"Writing: X [{pos.X},{pos.Y},{pos.Z}] Action [{action}] Distance [{distance}]", "AmeisenCore");
            BlackMagic.WriteFloat(Offsets.ctmX, (float)pos.X);
            BlackMagic.WriteFloat(Offsets.ctmY, (float)pos.Y);
            BlackMagic.WriteFloat(Offsets.ctmZ, (float)pos.Z);
            BlackMagic.WriteInt(Offsets.ctmAction, (int)action);
            BlackMagic.WriteFloat(Offsets.ctmDistance, distance);
        }
    }
}