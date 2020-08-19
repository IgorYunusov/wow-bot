using AmeisenBotCore;
using AmeisenBotUtilities;
using AmeisenBotUtilities.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace AmeisenBotCombat
{
    public abstract class CombatUtils
    {
        /// <summary>
        /// Get a target to attack by scanniung hat your partymembers attack
        /// </summary>
        /// <param name="me">me object</param>
        /// <param name="activeWowObjects">all active wow objects</param>
        /// <returns>a possible target unit</returns>
        public static Unit AssistParty(Me me, List<WowObject> activeWowObjects, List<Unit> partymembers)
        {
            // Get the one with the lowest hp and assist him/her
            List<Unit> units = GetPartymembersInCombat(me, partymembers);
            if (units.Count > 0)
            {
                foreach (Unit u in units.OrderBy(o => o.HealthPercentage).ToList())
                {
                    u.Update();

                    Unit targetToAttack = (Unit)GetWoWObjectFromGUID(u.TargetGuid, activeWowObjects);
                    if (IsUnitValid(targetToAttack))
                    {
                        continue;
                    }

                    targetToAttack.Update();

                    // check if the target is an invalid object
                    if (activeWowObjects.FindAll(o => o.Guid == targetToAttack.Guid).Count == 0)
                    {
                        continue;
                    }

                    AmeisenCore.TargetGUID(targetToAttack.Guid);
                    me.Update();

                    // if we can attack the target, do it
                    if (CanAttack(LuaUnit.target))
                    {
                        return targetToAttack;
                    }
                }
            }
            return null;
        }

        public static void AttackTarget()
            => AmeisenCore.RunSlashCommand("/startattack");

        /// <summary>
        /// Check the LuaUnit for its attacked to you
        /// </summary>
        /// <param name="luaUnit">luaunit to check</param>
        /// <returns>wether the unit can be attacked or not</returns>
        public static bool CanAttack(LuaUnit luaUnit)
            => AmeisenCore.CanAttack(luaUnit);

        /// <summary>
        /// Check the LuaUnit for its coop-state to you
        /// </summary>
        /// <param name="luaUnit">luaunit to check</param>
        /// <returns>wether the unit can be cooperated with or not</returns>
        public static bool CanCooperate(LuaUnit luaUnit)
            => AmeisenCore.CanCooperate(luaUnit);

        /// <summary>
        /// Cast a spell
        /// </summary>
        /// <param name="me">you</param>
        /// <param name="target">target to cast the spell on</param>
        /// <param name="spellname">spell to cast</param>
        /// <param name="onMyself">cast sepll on yourself</param>
        /// <param name="waitOnCastToFinish">wait for the cast to finish</param>
        public static void CastSpellByName(Me me, Unit target, string spellname, bool onMyself, bool waitOnCastToFinish = true)
        {
            SpellInfo spellInfo = GetSpellInfo(spellname);
            if (IsUnitValid(target))
            {
                if (!IsFacingMelee(me, target))
                {
                    FaceUnit(me, target);
                }
            }

            AmeisenCore.CastSpellByName(spellname, onMyself);

            if (waitOnCastToFinish)
            {
                int sleeptime = spellInfo.castTime - 100;
                Thread.Sleep(sleeptime > 0 ? sleeptime : 0);

                FaceUnit(me, target);
                Thread.Sleep(100);

                while (AmeisenCore.GetUnitCastingInfo(LuaUnit.player).duration >= 100)
                {
                    Thread.Sleep(100);
                }
            }
        }

        /// <summary>
        /// Turn into a specified Units direction
        /// </summary>
        /// <param name="me">me object</param>
        /// <param name="unit">unit to turn to</param>
        public static void FaceUnit(Me me, Unit unit)
        {
            if (IsUnitValid(unit))
            {
                /*unit.Update();
                AmeisenCore.MovePlayerToXYZ(
                    unit.pos,
                    InteractionType.FACETARGET,
                    0);*/
                AmeisenCore.FaceUnit(me, unit);
            }
        }

        public static bool IsUnitValid(Unit unit)
        {
            return unit != null || unit.Guid != 0;
        }

        /// <summary>
        /// Get a Units buffs & debuffs
        /// </summary>
        /// <param name="luaUnit">luaunit to check</param>
        /// <returns>buffs & debuffs as a string list</returns>
        public static List<string> GetAuras(LuaUnit luaUnit)
            => AmeisenCore.GetAuras(luaUnit);

        /// <summary>
        /// Get a Units buffs
        /// </summary>
        /// <param name="luaUnit">luaunit to check</param>
        /// <returns>buffs as a string list</returns>
        public static List<string> GetBuffs(LuaUnit luaUnit)
            => AmeisenCore.GetBuffs(luaUnit);

        /// <summary>
        /// Get a Units debuffs
        /// </summary>
        /// <param name="luaUnit">luaunit to check</param>
        /// <returns>debuffs as a string list</returns>
        public static List<string> GetDebuffs(LuaUnit luaUnit)
            => AmeisenCore.GetDebuffs(luaUnit);

        /// <summary>
        /// Check if any of our partymembers are in combat
        /// </summary>
        /// <returns>returns all partymembers in combat</returns>
        public static List<Unit> GetPartymembers(Me me, List<WowObject> activeWowObjects)
        {
            List<Unit> partymembers = new List<Unit>();
            try
            {
                foreach (WowObject obj in activeWowObjects)
                {
                    if (obj.GetType() == typeof(Unit)
                        || obj.GetType() == typeof(Player)
                        || obj.GetType() == typeof(Me))
                    {
                        if (me.PartymemberGuids.Contains(obj.Guid))
                        {
                            ((Unit)obj).Update();
                            partymembers.Add(((Unit)obj));
                        }
                    }
                }
            }
            catch { }

            if (!me.PartymemberGuids.Contains(me.PartyleaderGuid))
            {
                Unit leader = (Unit)GetWoWObjectFromGUID(me.PartyleaderGuid, activeWowObjects);
                partymembers.Add(leader);
            }

            return partymembers;
        }

        /// <summary>
        /// Check if any of our partymembers are in combat
        /// </summary>
        /// <returns>returns all partymembers in combat</returns>
        public static List<Unit> GetPartymembersInCombat(Me me, List<Unit> partymembers)
        {
            List<Unit> inCombatUnits = new List<Unit>();

            if (partymembers == null || partymembers.Count == 0)
            {
                return inCombatUnits;
            }

            try
            {
                foreach (Unit obj in partymembers)
                {
                    obj.Update();
                    if (obj.InCombat)
                    {
                        inCombatUnits.Add(obj);
                    }
                }
            }
            catch { }
            return inCombatUnits;
        }

        public static double GetSpellCooldown(string spellName)
            => double.Parse(
                AmeisenCore.GetLocalizedText($"start,duration,enabled = GetSpellCooldown(\"{spellName}\");cdLeft = (start + duration - GetTime()) * 1000;", "cdLeft"));

        /// <summary>
        /// Get the spellinfo of a spell that contains casttime
        /// </summary>
        /// <param name="spellname">spell to check</param>
        /// <returns>spellinfo</returns>
        public static SpellInfo GetSpellInfo(string spellname)
            => AmeisenCore.GetSpellInfo(spellname);

        /// <summary>
        /// Return a Player by the given GUID
        /// </summary>
        /// <param name="guid">guid of the player you want to get</param>
        /// <param name="activeWoWObjects">all wowo objects to search in</param>
        /// <returns>Player that you want to get</returns>
        public static WowObject GetWoWObjectFromGUID(ulong guid, List<WowObject> activeWoWObjects)
        {
            foreach (WowObject p in activeWoWObjects)
            {
                if (p.Guid == guid)
                {
                    return p;
                }
            }
            return null;
        }

        /// <summary>
        /// Check the LuaUnit for its enemy-state
        /// </summary>
        /// <param name="luaUnit">luaunit to check</param>
        /// <returns>wether the unit is an enemy or not</returns>
        public static bool IsEnemy(LuaUnit luaUnit)
            => AmeisenCore.IsEnemy(luaUnit);

        /// <summary>
        /// Check for facing a specific unit
        /// </summary
        /// <param name="me">you</param>
        /// <param name="unit">target</param>
        /// <returns>wether you're facing the unit or not</returns>
        public static bool IsFacingMelee(Me me, Unit unit, double minRotation = 0.6, double maxRotation = 1.4)
            => Utils.IsFacing(me.pos, me.Rotation, unit.pos, minRotation, maxRotation);

        /// <summary>
        /// Check for ranged combat facing a specific unit
        /// </summary
        /// <param name="me">you</param>
        /// <param name="unit">target</param>
        /// <returns>wether you're facing the unit or not</returns>
        public static bool IsFacingRanged(Me me, Unit unit, double minRotation = 0.9, double maxRotation = 1.1)
            => Utils.IsFacing(me.pos, me.Rotation, unit.pos, minRotation, maxRotation);

        /// <summary>
        /// Check the LuaUnit for its friend-state
        /// </summary>
        /// <param name="luaUnit">luaunit to check</param>
        /// <returns>wether the unit is a friend or not</returns>
        public static bool IsFriend(LuaUnit luaUnit)
            => AmeisenCore.IsFriend(luaUnit);

        /// <summary>
        /// Check the LuaUnit for its reaction to you
        /// </summary>
        /// <param name="luaUnit">luaunit to check</param>
        /// <returns>wether the unit reacts friendly or not</returns>
        public static bool IsFriendly(LuaUnit luaUnit)
        {
            UnitReaction reaction = AmeisenCore.GetUnitReaction(luaUnit);
            switch (reaction)
            {
                case UnitReaction.FRIENDLY:
                    return true;

                case UnitReaction.HONORED:
                    return true;

                case UnitReaction.REVERED:
                    return true;

                case UnitReaction.EXALTED:
                    return true;

                default:
                    return false;
            }
        }

        /// <summary>
        /// Check the LuaUnit for its reaction to you
        /// </summary>
        /// <param name="luaUnit">luaunit to check</param>
        /// <returns>wether the unit reacts hostile or not</returns>
        public static bool IsHostile(LuaUnit luaUnit)
            => AmeisenCore.GetUnitReaction(luaUnit) == UnitReaction.HOSTILE;

        /// <summary>
        /// Check if you're able to cast a certain spell at your current position
        /// </summary>
        /// <param name="me">me object</param>
        /// <param name="unitToAttack">unit to attack</param>
        /// <param name="distance">spell distance or something alike</param>
        /// <returns>true/false wether your are or arent in range to cast</returns>
        public static bool IsInRange(Me me, Unit unitToAttack, double distance)
            => Utils.GetDistance(me.pos, unitToAttack.pos) < distance;

        /// <summary>
        /// Check the LuaUnit for its reaction to you
        /// </summary>
        /// <param name="luaUnit">luaunit to check</param>
        /// <returns>wether the unit reacts neutral or not</returns>
        public static bool IsNeutral(LuaUnit luaUnit)
            => AmeisenCore.GetUnitReaction(luaUnit) == UnitReaction.NEUTRAL;

        public static bool IsSpellUseable(string spellname)
            => AmeisenCore.IsSpellUseable(spellname)
            && !AmeisenCore.IsOnCooldown(spellname);

        /// <summary>
        /// Move into a spell range
        /// </summary>
        /// <param name="me">me object</param>
        /// <param name="unitToAttack">unit to attack</param>
        /// <param name="distance">distance how close you want to get to the target</param>
        public static void MoveInRange(Me me, Unit unitToAttack, double distance)
        {
            int count = 0;
            while (Utils.GetDistance(me.pos, unitToAttack.pos) > distance 
                   && count < 5)
            {
                AmeisenCore.MovePlayerToXYZ(
                        unitToAttack.pos,
                        InteractionType.MOVE,
                        distance);
                me.Update();
                unitToAttack.Update();

                Thread.Sleep(50);
                count++;
            }

            AmeisenCore.InteractWithGUID(
                       unitToAttack.pos,
                       unitToAttack.Guid,
                       InteractionType.STOP);
        }

        public static void MoveToPos(Me me, Unit unitToAttack, double distance = 2.0)
        {
            if (Utils.GetDistance(me.pos, unitToAttack.pos) >= distance)
            {
                AmeisenCore.MovePlayerToXYZ(unitToAttack.pos, InteractionType.MOVE);
            }
        }

        public static void StopMovement(Me me)
            => AmeisenCore.InteractWithGUID(me.pos, me.Guid, InteractionType.STOP);

        public static void TargetLuaUnit(LuaUnit unit)
            => AmeisenCore.TargetLuaUnit(unit);

        public static void TargetNearestEnemy()
            => AmeisenCore.LuaDoString("TargetNearestEnemy();");

        /// <summary>
        /// Target a target that needs healing from your party
        /// </summary>
        /// <param name="me">me object</param>
        /// <param name="activeWowObjects">active wow objects</param>
        /// <returns>a teammember that needs healing</returns>
        public static Unit TargetTargetToHeal(Me me, List<WowObject> activeWowObjects)
        {
            // Get the one with the lowest hp and target him/her
            List<Unit> units = GetPartymembers(me, activeWowObjects);
            if (!units.Contains(me))
            {
                units.Add(me);
            }

            if (units.Count > 0)
            {
                List<Unit> unitsSorted =
                    units.Where(o => Utils.GetDistance(me.pos, o.pos) < 38)
                    .Where(o => o.HealthPercentage != 0)
                    .OrderBy(o => o.HealthPercentage).ToList();

                unitsSorted.First().Update();
                AmeisenCore.TargetGUID(unitsSorted.First().Guid);
                me.Update();
                return unitsSorted.First();
            }
            return null;
        }
    }
}