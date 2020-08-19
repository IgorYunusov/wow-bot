using AmeisenBot.Character.Objects;
using AmeisenBotCombat.Interfaces;
using AmeisenBotCore;
using AmeisenBotUtilities;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotCombat.SpellStrategies
{
    public class RogueCombat : ICombatClass
    {
        public RogueCombat(List<Spell> spells)
        {
            Spells = spells;

            IsSinisterStrikeKnown = Spells.Where(spell => spell.Name == "Sinister Strike").ToList().Count > 0;
            IsKickKnown = Spells.Where(spell => spell.Name == "Kick").ToList().Count > 0;
            IsSliceAndDiceKnown = Spells.Where(spell => spell.Name == "Slice and Dice").ToList().Count > 0;
            IsRuptureKnown = Spells.Where(spell => spell.Name == "Rupture").ToList().Count > 0;
            IsEviscerateKnown = Spells.Where(spell => spell.Name == "Eviscerate").ToList().Count > 0;
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

            // main spell rotation
            if (targetDistance < 3.5)
            {
                if (IsKickKnown && AmeisenCore.GetUnitCastingInfo(LuaUnit.target).duration > 0)
                {
                    spellToUse = TryUseSpell("Kick", me);
                    if (spellToUse != null) { return spellToUse; }
                }

                // use Sinister Strike
                if (IsSinisterStrikeKnown)
                {
                    spellToUse = TryUseSpell("Sinister Strike", me);
                    if (spellToUse != null) { return spellToUse; }
                }

                // use Slice and Dice
                if (IsSliceAndDiceKnown && !myAuras.Contains("slice and dice"))
                {
                    spellToUse = TryUseSpell("Slice and Dice", me);
                    if (spellToUse != null) { return spellToUse; }
                }

                // use Rupture
                if (IsRuptureKnown && !targetAuras.Contains("rupture"))
                {
                    spellToUse = TryUseSpell("Rupture", me);
                    if (spellToUse != null) { return spellToUse; }
                }

                // use Eviscerate
                if (IsEviscerateKnown)
                {
                    spellToUse = TryUseSpell("Eviscerate", me);
                    if (spellToUse != null) { return spellToUse; }
                }
            }

            return null;
        }

        public void Startup(Me me, Unit target, Unit pet)
        {
        }

        private bool IsEviscerateKnown { get; set; }
        private bool IsKickKnown { get; set; }
        private bool IsRuptureKnown { get; set; }
        private bool IsSinisterStrikeKnown { get; set; }
        private bool IsSliceAndDiceKnown { get; set; }
        private List<Spell> Spells { get; set; }

        private Spell TryUseSpell(string spellname, Me me)
        {
            Spell spellToUse = Spells.Where(spell => spell.Name == spellname).FirstOrDefault();

            if (spellToUse == null) { return null; }
            if (me.Energy < spellToUse.Costs) { return null; }

            if (CombatUtils.GetSpellCooldown(spellToUse.Name) < 0)
            {
                return spellToUse;
            }
            return null;
        }
    }
}