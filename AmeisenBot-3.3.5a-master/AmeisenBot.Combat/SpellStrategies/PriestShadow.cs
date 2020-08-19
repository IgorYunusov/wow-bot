using AmeisenBot.Character.Objects;
using AmeisenBotCombat.Interfaces;
using AmeisenBotCore;
using AmeisenBotUtilities;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotCombat.SpellStrategies
{
    public class PriestShadow : ICombatClass
    {
        public PriestShadow(List<Spell> spells)
        {
            Spells = spells;

            IsPowerwordFortitudeKnown = Spells.Where(spell => spell.Name == "Power Word: Fortitude").ToList().Count > 0;
            IsShadowformKnown = Spells.Where(spell => spell.Name == "Shadowform").ToList().Count > 0;
            IsVampiricEmbraceKnown = Spells.Where(spell => spell.Name == "Vampiric Embrace").ToList().Count > 0;
            IsVampiricTouchKnown = Spells.Where(spell => spell.Name == "Vampiric Touch").ToList().Count > 0;
            IsDevouringPlagueKnown = Spells.Where(spell => spell.Name == "Devouring Plague").ToList().Count > 0;
            IsShadowWordPainKnown = Spells.Where(spell => spell.Name == "Shadow Word: Pain").ToList().Count > 0;
            IsMindBlastKnown = Spells.Where(spell => spell.Name == "Mind Blast").ToList().Count > 0;
            IsMindFlayKnown = Spells.Where(spell => spell.Name == "Mind Flay").ToList().Count > 0;
            IsFlashHealKnown = Spells.Where(spell => spell.Name == "Flash Heal").ToList().Count > 0;
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

            // our Buff
            if (IsPowerwordFortitudeKnown && !myAuras.Contains("power word: fortitude"))
            {
                spellToUse = TryUseSpell("Power Word: Fortitude", me);
                if (spellToUse != null) { return spellToUse; }
            }

            // Shadowform
            if (IsShadowformKnown && !myAuras.Contains("shadowform"))
            {
                spellToUse = TryUseSpell("Shadowform", me);
                if (spellToUse != null) { return spellToUse; }
            }

            // our Vampiric Embrace
            if (IsVampiricEmbraceKnown && !myAuras.Contains("vampiric embrace"))
            {
                spellToUse = TryUseSpell("Vampiric Embrace", me);
                if (spellToUse != null) { CombatUtils.CastSpellByName(me, target, "Vampiric Embrace", true); } // only cast on me
            }

            // Heal oruself using Flash Heal
            if (IsFlashHealKnown && me.HealthPercentage < 50)
            {
                spellToUse = TryUseSpell("Vampiric Embrace", me);
                if (spellToUse != null) { CombatUtils.CastSpellByName(me, target, "Vampiric Embrace", true); } // only cast on me
            }

            // main spell rotation
            if (targetDistance < 28)
            {
                // Vampiric Touch
                if (IsVampiricTouchKnown && !targetAuras.Contains("vampiric touch"))
                {
                    spellToUse = TryUseSpell("Vampiric Touch", me);
                    if (spellToUse != null) { return spellToUse; }
                }

                // Devouring Plague
                if (IsDevouringPlagueKnown && !targetAuras.Contains("devouring plague"))
                {
                    spellToUse = TryUseSpell("Scorch", me);
                    if (spellToUse != null) { return spellToUse; }
                }

                // Shadow Word: Pain
                if (IsShadowWordPainKnown && !targetAuras.Contains("shadow word: pain"))
                {
                    spellToUse = TryUseSpell("Shadow Word: Pain", me);
                    if (spellToUse != null) { return spellToUse; }
                }

                // Mind Blast
                if (IsMindBlastKnown && !targetAuras.Contains("mind trauma"))
                {
                    spellToUse = TryUseSpell("Mind Blast", me);
                    if (spellToUse != null) { return spellToUse; }
                }

                // Spam Mind Flay
                if (IsMindFlayKnown)
                {
                    spellToUse = TryUseSpell("Mind Flay", me);
                    if (spellToUse != null) { return spellToUse; }
                }
            }

            return null;
        }

        public void Startup(Me me, Unit target, Unit pet)
        {
        }

        private bool IsDevouringPlagueKnown { get; set; }
        private bool IsFlashHealKnown { get; set; }
        private bool IsMindBlastKnown { get; set; }
        private bool IsMindFlayKnown { get; set; }
        private bool IsPowerwordFortitudeKnown { get; set; }
        private bool IsShadowformKnown { get; set; }
        private bool IsShadowWordPainKnown { get; set; }
        private bool IsVampiricEmbraceKnown { get; set; }
        private bool IsVampiricTouchKnown { get; set; }
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