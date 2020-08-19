using AmeisenBot.Character.Objects;
using AmeisenBotCombat.Interfaces;
using AmeisenBotCore;
using AmeisenBotUtilities;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotCombat.SpellStrategies
{
    public class PaladinRetribution : ICombatClass
    {
        public PaladinRetribution(List<Spell> spells)
        {
            Spells = spells;

            IsSealOfVengeanceKnown = Spells.Where(spell => spell.Name == "Seal of Vengeance").ToList().Count > 0;
            IsBlessingOfMightKnown = Spells.Where(spell => spell.Name == "Blessing of Might").ToList().Count > 0;
            IsLayOnHandsKnown = Spells.Where(spell => spell.Name == "Lay on Hands").ToList().Count > 0;
            IsHammerOfJusticeKnown = Spells.Where(spell => spell.Name == "Hammer of Justice").ToList().Count > 0;
            IsJudgementOfLightKnown = Spells.Where(spell => spell.Name == "Judgement of Light").ToList().Count > 0;
            IsJudgementOfWisdomKnown = Spells.Where(spell => spell.Name == "Judgement of Wisdom").ToList().Count > 0;
            IsHammerOfWrathKnown = Spells.Where(spell => spell.Name == "Hammer of Wrath").ToList().Count > 0;
            IsCrusaderStrikeKnown = Spells.Where(spell => spell.Name == "Crusader Strike").ToList().Count > 0;
            IsDivineStormKnown = Spells.Where(spell => spell.Name == "Divine Storm").ToList().Count > 0;
            IsConsecrationKnown = Spells.Where(spell => spell.Name == "Consecration").ToList().Count > 0;
            IsExorcismKnown = Spells.Where(spell => spell.Name == "Exorcism").ToList().Count > 0;
            IsAvengingWrathKnown = Spells.Where(spell => spell.Name == "Avenging Wrath").ToList().Count > 0;
            IsFlashHealKnown = Spells.Where(spell => spell.Name == "Flash Heal").ToList().Count > 0;
            IsHolyLightKnown = Spells.Where(spell => spell.Name == "Holy Light").ToList().Count > 0;
            IsDivinePleaKnown = Spells.Where(spell => spell.Name == "Divine Plea").ToList().Count > 0;
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

            // our Seal
            if (IsSealOfVengeanceKnown)
            {
                if (!myAuras.Contains("seal of vengeance"))
                {
                    spellToUse = TryUseSpell("Seal of Vengeance", me);
                    if (spellToUse != null) { return spellToUse; }
                }
            }

            // our Buff
            if (IsBlessingOfMightKnown)
            {
                if (!myAuras.Contains("blessing of might"))
                {
                    spellToUse = TryUseSpell("Blessing of Might", me);
                    if (spellToUse != null) { return spellToUse; }
                }
            }

            // if we go under 70% Mana, use Divine Plea
            if (IsDivinePleaKnown && me.HealthPercentage < 70)
            {
                spellToUse = TryUseSpell("Divine Plea", me);
                if (spellToUse != null)
                {
                    CombatUtils.CastSpellByName(me, target, "Divine Plea", false, true); // doesn't trigger GCD
                }
            }

            // if we go under 10% health, use our insta-full-heal
            if (IsLayOnHandsKnown && me.HealthPercentage < 10 && !myAuras.Contains("forbearance"))
            {
                spellToUse = TryUseSpell("Lay on Hands", me);
                if (spellToUse != null) { return spellToUse; }
            }

            // if we go under 70% health, use Flash Heal
            if (IsFlashHealKnown && me.HealthPercentage < 70)
            {
                spellToUse = TryUseSpell("Flash Heal", me);
                if (spellToUse != null) { return spellToUse; }
            }

            // if we go under 40% health, use Holy Light
            if (IsHolyLightKnown && me.HealthPercentage < 40)
            {
                spellToUse = TryUseSpell("Holy Light", me);
                if (spellToUse != null) { return spellToUse; }
            }

            // Avenging Wrath cooldown
            if (IsAvengingWrathKnown)
            {
                spellToUse = TryUseSpell("Avenging Wrath", me);
                if (spellToUse != null)
                {
                    CombatUtils.CastSpellByName(me, target, "Avenging Wrath", false, true); // doesn't trigger GCD
                }
            }

            // stun
            if (targetDistance < 5)
            {
                // try to Fear it away lmao
                if (IsHammerOfJusticeKnown)
                {
                    spellToUse = TryUseSpell("Hammer of Justice", me);
                    if (spellToUse != null) { return spellToUse; }
                }
            }

            // main spell rotation
            if (targetDistance < 30)
            {
                // apply our Judgement
                if (IsJudgementOfWisdomKnown)
                {
                    spellToUse = TryUseSpell("Judgement of Wisdom", me);
                    if (spellToUse != null) { return spellToUse; }
                }
                else if (IsJudgementOfLightKnown)
                {
                    spellToUse = TryUseSpell("Judgement of Light", me);
                    if (spellToUse != null) { return spellToUse; }
                }

                // if target is over 20% use Hammer of Wrath
                if (target.HealthPercentage < 20)
                {
                    if (IsHammerOfWrathKnown)
                    {
                        spellToUse = TryUseSpell("Hammer of Wrath", me);
                        if (spellToUse != null) { return spellToUse; }
                    }
                }

                // use Crusader Strike
                if (IsCrusaderStrikeKnown)
                {
                    spellToUse = TryUseSpell("Crusader Strike", me);
                    if (spellToUse != null) { return spellToUse; }
                }

                // use Divine Storm
                if (IsDivineStormKnown)
                {
                    spellToUse = TryUseSpell("Divine Storm", me);
                    if (spellToUse != null) { return spellToUse; }
                }

                // use Consecration
                if (IsConsecrationKnown)
                {
                    spellToUse = TryUseSpell("Consecration", me);
                    if (spellToUse != null) { return spellToUse; }
                }

                // use Exorcism
                if (IsExorcismKnown)
                {
                    spellToUse = TryUseSpell("Exorcism", me);
                    if (spellToUse != null) { return spellToUse; }
                }
            }

            return null;
        }

        public void Startup(Me me, Unit target, Unit pet)
        {
        }

        private bool IsAvengingWrathKnown { get; set; }
        private bool IsBlessingOfMightKnown { get; set; }
        private bool IsConsecrationKnown { get; set; }
        private bool IsCrusaderStrikeKnown { get; set; }
        private bool IsDivinePleaKnown { get; set; }
        private bool IsDivineStormKnown { get; set; }
        private bool IsExorcismKnown { get; set; }
        private bool IsFlashHealKnown { get; set; }
        private bool IsHammerOfJusticeKnown { get; set; }
        private bool IsHammerOfWrathKnown { get; set; }
        private bool IsHolyLightKnown { get; set; }
        private bool IsJudgementOfLightKnown { get; set; }
        private bool IsJudgementOfWisdomKnown { get; set; }
        private bool IsLayOnHandsKnown { get; set; }
        private bool IsSealOfVengeanceKnown { get; set; }
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