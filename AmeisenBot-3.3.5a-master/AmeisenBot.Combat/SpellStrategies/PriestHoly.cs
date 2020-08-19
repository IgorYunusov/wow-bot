using AmeisenBot.Character.Objects;
using AmeisenBotCombat.Interfaces;
using AmeisenBotCore;
using AmeisenBotUtilities;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotCombat.SpellStrategies
{
    public class PriestHoly : ICombatClass
    {
        public PriestHoly(List<Spell> spells)
        {
            Spells = spells;

            IsPrayerOfMendingKnown = Spells.Where(spell => spell.Name == "Prayer of Mending").ToList().Count > 0;
            IsPowerwordFortitudeKnown = Spells.Where(spell => spell.Name == "Power Word: Fortitude").ToList().Count > 0;
            IsInnerFireKnown = Spells.Where(spell => spell.Name == "Prayer of Mending").ToList().Count > 0;
            IsShadowfiendKnown = Spells.Where(spell => spell.Name == "Shadowfiend").ToList().Count > 0;
            IsRenewKnown = Spells.Where(spell => spell.Name == "Renew").ToList().Count > 0;
            IsBindingHealKnown = Spells.Where(spell => spell.Name == "Binding Heal").ToList().Count > 0;
            IsGreaterHealKnown = Spells.Where(spell => spell.Name == "Greater Heal").ToList().Count > 0;
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
            if (IsPowerwordFortitudeKnown && !targetAuras.Contains("power word: fortitude"))
            {
                spellToUse = TryUseSpell("Power Word: Fortitude", me);
                if (spellToUse != null) { return spellToUse; }
            }

            // our defensive buff
            if (IsInnerFireKnown && !myAuras.Contains("inner fire"))
            {
                spellToUse = TryUseSpell("Inner Fire", me);
                if (spellToUse != null) { CombatUtils.CastSpellByName(me, target, "Inner Fire", true); } // only cast on me
            }

            // Shadowfiend mana regen
            if (IsShadowfiendKnown && me.ManaPercentage < 20)
            {
                spellToUse = TryUseSpell("Shadowfiend", me);
                if (spellToUse != null)
                {
                    CombatUtils.TargetNearestEnemy(); // use shadowfiend on something around us
                    CombatUtils.CastSpellByName(me, target, "Shadowfiend", false);
                }
            }

            // main spell rotation
            if (targetDistance < 38)
            {
                // Renew HOT
                if (IsRenewKnown && target.HealthPercentage < 80 && !targetAuras.Contains("renew"))
                {
                    spellToUse = TryUseSpell("Renew", me);
                    if (spellToUse != null) { return spellToUse; }
                }

                // Binding Heal if me is low too, otherwise Flash Heal until Serendipity is stacked, then use Greater Heal
                if (target.HealthPercentage < 70)
                {
                    if (IsBindingHealKnown && me.HealthPercentage < 80)
                    {
                        spellToUse = TryUseSpell("Binding Heal", me);
                        if (spellToUse != null) { return spellToUse; }
                    }
                    else
                    {
                        if (IsGreaterHealKnown && myAuras.Contains("serendipity"))
                        {
                            spellToUse = TryUseSpell("Greater Heal", me);
                            if (spellToUse != null) { return spellToUse; }
                        }
                        else if (IsFlashHealKnown)
                        {
                            spellToUse = TryUseSpell("Flash Heal", me);
                            if (spellToUse != null) { return spellToUse; }
                        }
                    }
                }
            }

            return null;
        }

        public void Startup(Me me, Unit target, Unit pet)
        {
        }

        private bool IsBindingHealKnown { get; set; }
        private bool IsFlashHealKnown { get; set; }
        private bool IsGreaterHealKnown { get; set; }
        private bool IsInnerFireKnown { get; set; }
        private bool IsPowerwordFortitudeKnown { get; set; }
        private bool IsPrayerOfMendingKnown { get; set; }
        private bool IsRenewKnown { get; set; }
        private bool IsShadowfiendKnown { get; set; }
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