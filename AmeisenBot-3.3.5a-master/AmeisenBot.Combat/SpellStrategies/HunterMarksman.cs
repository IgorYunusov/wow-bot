using AmeisenBot.Character.Objects;
using AmeisenBotCombat.Interfaces;
using AmeisenBotCore;
using AmeisenBotUtilities;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotCombat.SpellStrategies
{
    public class HunterMarksman : ICombatClass
    {
        public HunterMarksman(List<Spell> spells)
        {
            Spells = spells;

            IsAspectOfTheHawkKnown = Spells.Where(spell => spell.Name == "Aspect of the Hawk").ToList().Count > 0;
            IsKillShotKnown = Spells.Where(spell => spell.Name == "Kill Shot").ToList().Count > 0;
            IsSerpentStingKnown = Spells.Where(spell => spell.Name == "Serpent Sting").ToList().Count > 0;
            IsChimeraShotKnown = Spells.Where(spell => spell.Name == "Chimera Shot").ToList().Count > 0;
            IsAimedShotKnown = Spells.Where(spell => spell.Name == "Aimed Shot").ToList().Count > 0;
            IsArcaneShotKnown = Spells.Where(spell => spell.Name == "Arcane Shot").ToList().Count > 0;
            IsSteadyShotKnown = Spells.Where(spell => spell.Name == "Steady Shot").ToList().Count > 0;
            IsFrostTrapKnown = Spells.Where(spell => spell.Name == "Frost Trap").ToList().Count > 0;
            IsFreezingTrapKnown = Spells.Where(spell => spell.Name == "Freezing Trap").ToList().Count > 0;
            IsDisengageKnown = Spells.Where(spell => spell.Name == "Disengage").ToList().Count > 0;
            IsTrueshotAuraKnown = Spells.Where(spell => spell.Name == "Trueshot Aura").ToList().Count > 0;
            IsHuntersMarkKnown = Spells.Where(spell => spell.Name == "Hunter's Mark").ToList().Count > 0;
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

            // our aspect
            if (IsAspectOfTheHawkKnown && !myAuras.Contains("aspect of the hawk"))
            {
                spellToUse = TryUseSpell("Aspect of the Hawk", me);
                if (spellToUse != null) { return spellToUse; }
            }

            // Trueshot Aura
            if (IsTrueshotAuraKnown && !myAuras.Contains("trueshot aura"))
            {
                spellToUse = TryUseSpell("Trueshot Aura", me);
                if (spellToUse != null) { return spellToUse; }
            }

            // Hunters Mark
            if (IsHuntersMarkKnown && !targetAuras.Contains("hunter's mark"))
            {
                spellToUse = TryUseSpell("Hunter's Mark", me);
                if (spellToUse != null) { return spellToUse; }
            }

            // gtfo-combo
            if (targetDistance < 8)
            {
                if (IsFrostTrapKnown)
                {
                    spellToUse = TryUseSpell("Frost Trap", me); // Frost Trap
                    if (spellToUse != null) { return spellToUse; }
                }
                else if (IsFreezingTrapKnown)
                {
                    spellToUse = TryUseSpell("Freezing Trap", me); // Freezing Trap
                    if (spellToUse != null) { return spellToUse; }
                }

                if (IsDisengageKnown)
                {
                    spellToUse = TryUseSpell("Disengage", me); // GTFO
                    if (spellToUse != null) { return spellToUse; }
                }
            }

            // main spell rotation
            if (targetDistance > 5 && targetDistance < 33)
            {
                if (IsKillShotKnown && target.HealthPercentage < 20)
                {
                    spellToUse = TryUseSpell("Seal of Vengeance", me);
                    if (spellToUse != null) { return spellToUse; }
                }

                if (IsSerpentStingKnown && !targetAuras.Contains("serpent sting"))
                {
                    spellToUse = TryUseSpell("Serpent Sting", me);
                    if (spellToUse != null) { return spellToUse; }
                }

                if (IsChimeraShotKnown)
                {
                    spellToUse = TryUseSpell("Chimera Shot", me);
                    if (spellToUse != null) { return spellToUse; }
                }

                if (IsAimedShotKnown)
                {
                    spellToUse = TryUseSpell("Aimed Shot", me);
                    if (spellToUse != null) { return spellToUse; }
                }

                if (IsArcaneShotKnown)
                {
                    spellToUse = TryUseSpell("Arcane Shot", me);
                    if (spellToUse != null) { return spellToUse; }
                }

                if (IsSteadyShotKnown)
                {
                    spellToUse = TryUseSpell("Steady Shot", me);
                    if (spellToUse != null) { return spellToUse; }
                }
            }

            return null;
        }

        public void Startup(Me me, Unit target, Unit pet)
        {
        }

        private bool IsAimedShotKnown { get; set; }
        private bool IsArcaneShotKnown { get; set; }
        private bool IsAspectOfTheHawkKnown { get; set; }
        private bool IsChimeraShotKnown { get; set; }
        private bool IsDisengageKnown { get; set; }
        private bool IsFreezingTrapKnown { get; set; }
        private bool IsFrostTrapKnown { get; set; }
        private bool IsHuntersMarkKnown { get; set; }
        private bool IsKillShotKnown { get; set; }
        private bool IsSerpentStingKnown { get; set; }
        private bool IsSteadyShotKnown { get; set; }
        private bool IsTrueshotAuraKnown { get; set; }
        private List<Spell> Spells { get; set; }

        private Spell TryUseSpell(string spellname, Me me)
        {
            Spell spellToUse = Spells.Where(spell => spell.Name == spellname).FirstOrDefault();

            if (spellToUse == null) { return null; }
            if (me.Mana < spellToUse.Costs) { return null; }

            if (CombatUtils.GetSpellCooldown(spellToUse.Name) < 0)
            {
                return spellToUse;
            }
            return null;
        }
    }
}