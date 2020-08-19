using AmeisenBot.Character.Objects;
using AmeisenBotCombat.Interfaces;
using AmeisenBotCore;
using AmeisenBotUtilities;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotCombat.SpellStrategies
{
    public class WarriorFury : ICombatClass
    {
        public WarriorFury(List<Spell> spells)
        {
            Spells = spells;

            IsSlamKnown = Spells.Where(spell => spell.Name == "Slam").ToList().Count > 0;
            IsBloodthirstKnown = Spells.Where(spell => spell.Name == "Bloodthirst").ToList().Count > 0;
            IsWhirlwindKnown = Spells.Where(spell => spell.Name == "Whirlwind").ToList().Count > 0;
            IsBerserkerRageKnown = Spells.Where(spell => spell.Name == "Berserker Rage").ToList().Count > 0;
            IsHeroicStrikeKnown = Spells.Where(spell => spell.Name == "Heroic Strike").ToList().Count > 0;
            IsHeroicThrowKnown = Spells.Where(spell => spell.Name == "Heroic Throw").ToList().Count > 0;
            IsExecuteKnown = Spells.Where(spell => spell.Name == "Execute").ToList().Count > 0;
            IsRecklessnessKnown = Spells.Where(spell => spell.Name == "Recklessness").ToList().Count > 0;
            IsDeathWishKnown = Spells.Where(spell => spell.Name == "Death Wish").ToList().Count > 0;
            IsEnragedRegenerationKnown = Spells.Where(spell => spell.Name == "Enraged Regeneration").ToList().Count > 0;
            IsInterceptKnown = Spells.Where(spell => spell.Name == "Intercept").ToList().Count > 0;
            IsHamstringKnown = Spells.Where(spell => spell.Name == "Hamstring").ToList().Count > 0;
            IsBattleShoutKnown = Spells.Where(spell => spell.Name == "Battle Shout").ToList().Count > 0;
            IsBerserkerStanceKnown = Spells.Where(spell => spell.Name == "Berserker Stance").ToList().Count > 0;

            IsInMainCombo = false;
        }

        public Spell DoRoutine(Me me, Unit target, Unit pet)
        {
            if (CombatUtils.IsUnitValid(me)
                && CombatUtils.IsUnitValid(target))
            {
                return null;
            }

            List<string> myAuras = AmeisenCore.GetAuras(LuaUnit.player);
            List<string> targetAuras = AmeisenCore.GetAuras(LuaUnit.target);

            Spell spellToUse = null;
            double targetDistance = Utils.GetDistance(me.pos, target.pos);

            // if we are low on HP try to use Enraged Regeneration
            /*if (me.HealthPercentage < 40)
            {
                if (IsEnragedRegenerationKnown)
                {
                    spellToUse = TryUseSpell("Enraged Regeneration", me);
                    if (spellToUse != null) { return spellToUse; }
                }
            }*/

            // hold Berserker Rage on cooldown
            if (IsBerserkerRageKnown && !IsInMainCombo)
            {
                spellToUse = TryUseSpell("Berserker Rage", me);
                if (spellToUse != null) { return spellToUse; }
            }

            // main spell rotation
            if (targetDistance < 3)
            {
                // if we got enough rage and nothing better to do, use Heroic Strike
                if (me.Rage > 50 && target.HealthPercentage > 15)
                {
                    // Heroic Strike wont't interrupt main-combo
                    if (IsHeroicStrikeKnown)
                    {
                        CombatUtils.CastSpellByName(me, target, "Heroic Strike", false, false); // don't wait on cooldown
                    }
                }

                // if we are in our main-combo, use the second part of it, whirlwind
                if (IsWhirlwindKnown && IsInMainCombo && IsBerserkerStanceKnown)
                {
                    // dont't interrupt main-combo
                    spellToUse = TryUseSpell("Whirlwind", me);
                    if (spellToUse != null) { IsInMainCombo = false; }
                    // normally whirlwind has 10s cooldown, bloodthirst only 5 so use it if we are still in that 10s
                    else if (IsBloodthirstKnown) { spellToUse = TryUseSpell("Bloodthirst", me); }

                    return spellToUse;
                }
                else if (!IsBerserkerStanceKnown)
                {
                    IsInMainCombo = false;
                }

                // use hamstring so our enemy can't escape
                if (IsHamstringKnown && !targetAuras.Contains("hamstring"))
                {
                    spellToUse = TryUseSpell("Hamstring", me);
                    if (spellToUse != null) { return spellToUse; }
                }

                // when da slam procs, use it
                if (IsSlamKnown && myAuras.Contains("slam!"))
                {
                    spellToUse = TryUseSpell("Slam", me);
                    if (spellToUse != null) { return spellToUse; }
                }

                // start our main-combo
                if (IsBloodthirstKnown)
                {
                    spellToUse = TryUseSpell("Bloodthirst", me);

                    if (spellToUse != null)
                    {
                        IsInMainCombo = true;
                        return spellToUse;
                    }
                }

                // hold Recklessness on cooldown
                if (IsRecklessnessKnown && !IsInMainCombo)
                {
                    spellToUse = TryUseSpell("Recklessness", me);
                    if (spellToUse != null) { return spellToUse; }
                }

                // hold Death Wish on cooldown
                if (IsDeathWishKnown && !IsInMainCombo)
                {
                    spellToUse = TryUseSpell("Death Wish", me);
                    if (spellToUse != null) { return spellToUse; }
                }

                // hold Battleshout on cooldown
                if (IsBattleShoutKnown && !IsInMainCombo && !myAuras.Contains("battle shout"))
                {
                    spellToUse = TryUseSpell("Battle Shout", me);
                    if (spellToUse != null) { return spellToUse; }
                }

                if (me.Rage > 50)
                {
                    if (IsExecuteKnown && target.HealthPercentage < 15 && !IsInMainCombo)
                    {
                        spellToUse = TryUseSpell("Execute", me);
                        if (spellToUse != null) { return spellToUse; }
                    }
                }
            }
            else if (targetDistance > 8 && targetDistance < 25)
            {
                // try to charge to our target
                if (IsInterceptKnown)
                {
                    spellToUse = TryUseSpell("Intercept", me);
                    if (spellToUse != null) { return spellToUse; }
                }
            }
            else if (targetDistance < 30)
            {
                // if there is really nothing other to do, throw something
                if (IsHeroicThrowKnown)
                {
                    spellToUse = TryUseSpell("Heroic Throw", me);
                    if (spellToUse != null) { return spellToUse; }
                }
            }
            return null;
        }

        public void Startup(Me me, Unit target, Unit pet)
        {
        }

        private bool IsBattleShoutKnown { get; set; }
        private bool IsBerserkerRageKnown { get; set; }
        private bool IsBerserkerStanceKnown { get; set; }
        private bool IsBloodthirstKnown { get; set; }
        private bool IsDeathWishKnown { get; set; }
        private bool IsEnragedRegenerationKnown { get; set; }
        private bool IsExecuteKnown { get; set; }
        private bool IsHamstringKnown { get; set; }
        private bool IsHeroicStrikeKnown { get; set; }
        private bool IsHeroicThrowKnown { get; set; }
        private bool IsInMainCombo { get; set; }
        private bool IsInterceptKnown { get; set; }
        private bool IsRecklessnessKnown { get; set; }
        private bool IsSlamKnown { get; set; }
        private bool IsWhirlwindKnown { get; set; }
        private List<Spell> Spells { get; set; }

        private Spell TryUseSpell(string spellname, Me me)
        {
            Spell spellToUse = Spells.Where(spell => spell.Name == spellname).FirstOrDefault();

            if (spellToUse == null) { return null; }
            if (me.Rage < spellToUse.Costs) { return null; }

            if (CombatUtils.GetSpellCooldown(spellToUse.Name) < 0)
            {
                IsInMainCombo = false;
                return spellToUse;
            }
            return null;
        }
    }
}