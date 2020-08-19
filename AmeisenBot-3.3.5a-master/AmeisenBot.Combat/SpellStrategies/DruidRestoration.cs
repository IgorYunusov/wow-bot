using AmeisenBot.Character.Objects;
using AmeisenBotCombat.Interfaces;
using AmeisenBotCore;
using AmeisenBotUtilities;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotCombat.SpellStrategies
{
    public class DruidRestoration : ICombatClass
    {
        public DruidRestoration(List<Spell> spells)
        {
            Spells = spells;

            IsMarkOfTheWildKnown = Spells.Where(spell => spell.Name == "Mark of the Wild").ToList().Count > 0;
            IsRegrowthKnown = Spells.Where(spell => spell.Name == "Regrowth").ToList().Count > 0;
            IsLifebloomKnown = Spells.Where(spell => spell.Name == "Lifebloom").ToList().Count > 0;
            IsWildGrowthKnown = Spells.Where(spell => spell.Name == "Wild Growth").ToList().Count > 0;
            IsRejuvenationKnown = Spells.Where(spell => spell.Name == "Rejuvenation").ToList().Count > 0;
            IsSwiftmendKnown = Spells.Where(spell => spell.Name == "Swiftmend").ToList().Count > 0;
            IsTranquiullityKnown = Spells.Where(spell => spell.Name == "Tranquiullity").ToList().Count > 0;
            IsBarkskinKnown = Spells.Where(spell => spell.Name == "Barkskin").ToList().Count > 0;
            IsNaturesSwiftnessKnown = Spells.Where(spell => spell.Name == "Nature's Swiftness").ToList().Count > 0;
            IsInnervateKnown = Spells.Where(spell => spell.Name == "Innervate").ToList().Count > 0;
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
            if (IsMarkOfTheWildKnown && !targetAuras.Contains("mark of the wild"))
            {
                spellToUse = TryUseSpell("Mark of the Wild", me);
                if (spellToUse != null) { return spellToUse; }
            }

            // our Barkskin
            if (IsMarkOfTheWildKnown && !myAuras.Contains("barkskin"))
            {
                spellToUse = TryUseSpell("Barkskin", me);
                if (spellToUse != null) { CombatUtils.CastSpellByName(me, target, "Barkskin", true); } // only cast on me
            }

            // Innervate mana regen
            if (IsInnervateKnown && me.ManaPercentage < 20)
            {
                spellToUse = TryUseSpell("Innervate", me);
                if (spellToUse != null) { CombatUtils.CastSpellByName(me, target, "Innervate", true); } // only cast on me
            }

            // main spell rotation
            if (targetDistance < 38)
            {
                // Swiftmend burst heal
                if (IsSwiftmendKnown && target.HealthPercentage < 40 && targetAuras.Contains("rejuvenation"))
                {
                    spellToUse = TryUseSpell("Swiftmend", me);
                    if (spellToUse != null) { return spellToUse; }
                }

                // Regrowth HOT
                if (IsRegrowthKnown && target.HealthPercentage < 80 && !targetAuras.Contains("regrowth"))
                {
                    spellToUse = TryUseSpell("Regrowth", me);
                    if (spellToUse != null) { return spellToUse; }
                }

                // Lifebloom HOT
                if (IsLifebloomKnown && target.HealthPercentage < 90 && !targetAuras.Contains("lifebloom"))
                {
                    spellToUse = TryUseSpell("Lifebloom", me);
                    if (spellToUse != null) { return spellToUse; }
                }

                // Wild Growth HOT
                if (IsWildGrowthKnown && target.HealthPercentage < 70 && !targetAuras.Contains("wild growth"))
                {
                    spellToUse = TryUseSpell("Wild Growth", me);
                    if (spellToUse != null) { return spellToUse; }
                }

                // Rejuvenation HOT
                if (IsRejuvenationKnown && target.HealthPercentage < 70 && !targetAuras.Contains("rejuvenation"))
                {
                    spellToUse = TryUseSpell("Rejuvenation", me);
                    if (spellToUse != null) { return spellToUse; }
                }
            }

            return null;
        }

        public void Startup(Me me, Unit target, Unit pet)
        {
        }

        private bool IsBarkskinKnown { get; set; }
        private bool IsInnervateKnown { get; set; }
        private bool IsLifebloomKnown { get; set; }
        private bool IsMarkOfTheWildKnown { get; set; }
        private bool IsNaturesSwiftnessKnown { get; set; }
        private bool IsRegrowthKnown { get; set; }
        private bool IsRejuvenationKnown { get; set; }
        private bool IsSwiftmendKnown { get; set; }
        private bool IsTranquiullityKnown { get; set; }
        private bool IsWildGrowthKnown { get; set; }
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