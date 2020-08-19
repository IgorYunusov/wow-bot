using AmeisenBot.Character.Objects;
using AmeisenBotCombat.Interfaces;
using AmeisenBotCore;
using AmeisenBotUtilities;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotCombat.SpellStrategies
{
    public class MageFire : ICombatClass
    {
        public MageFire(List<Spell> spells)
        {
            Spells = spells;

            IsArcaneIntellectKnown = Spells.Where(spell => spell.Name == "Arcane Intellect").ToList().Count > 0;
            IsMageArmorKnown = Spells.Where(spell => spell.Name == "Mage Armor").ToList().Count > 0;
            IsManaShieldKnown = Spells.Where(spell => spell.Name == "Mana Shield").ToList().Count > 0;
            IsEvocationKnown = Spells.Where(spell => spell.Name == "Evocation").ToList().Count > 0;
            IsMirrorImageKnown = Spells.Where(spell => spell.Name == "Mirror Image").ToList().Count > 0;
            IsScorchKnown = Spells.Where(spell => spell.Name == "Scorch").ToList().Count > 0;
            IsPyroblastKnown = Spells.Where(spell => spell.Name == "Pyroblast").ToList().Count > 0;
            IsForstfireBoltKnown = Spells.Where(spell => spell.Name == "Frostfire Bolt").ToList().Count > 0;
            IsFireballKnown = Spells.Where(spell => spell.Name == "Fireball").ToList().Count > 0;
            IsLivingBombKnown = Spells.Where(spell => spell.Name == "Living Bomb").ToList().Count > 0;
            IsIceBlockKnown = Spells.Where(spell => spell.Name == "Ice Block").ToList().Count > 0;
            IsMoltenArmorKnown = Spells.Where(spell => spell.Name == "Molten Armor").ToList().Count > 0;
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
            if (IsArcaneIntellectKnown && !myAuras.Contains("arcane intellect"))
            {
                spellToUse = TryUseSpell("Arcane Intellect", me);
                if (spellToUse != null) { return spellToUse; }
            }

            // our Mage Armor
            if (IsMageArmorKnown && me.ManaPercentage > 10 && !myAuras.Contains("mage armor"))
            {
                spellToUse = TryUseSpell("Mage Armor", me);
                if (spellToUse != null) { CombatUtils.CastSpellByName(me, target, "Mage Armor", true); } // only cast on me
            }

            // our Molten Armor
            if (IsMoltenArmorKnown && me.ManaPercentage < 10 && !myAuras.Contains("molten armor"))
            {
                spellToUse = TryUseSpell("Molten Armor", me);
                if (spellToUse != null) { CombatUtils.CastSpellByName(me, target, "Molten Armor", true); } // only cast on me
            }

            // Evocation mana regen
            if (IsEvocationKnown && me.ManaPercentage < 20)
            {
                spellToUse = TryUseSpell("Evocation", me);
                if (spellToUse != null) { return spellToUse; }
            }

            // Mirror Image CD
            if (IsMirrorImageKnown)
            {
                spellToUse = TryUseSpell("Mirror Image", me);
                if (spellToUse != null) { return spellToUse; }
            }

            // Mana Shield
            if (IsManaShieldKnown && me.HealthPercentage < 70)
            {
                spellToUse = TryUseSpell("Mana Shield", me);
                if (spellToUse != null) { return spellToUse; }
            }

            // Ice Block if we are about to die
            if (IsIceBlockKnown && me.HealthPercentage < 20)
            {
                spellToUse = TryUseSpell("Ice Block", me);
                if (spellToUse != null) { return spellToUse; }
            }

            // main spell rotation
            if (targetDistance < 28)
            {
                // Hot Streak proc
                if (IsPyroblastKnown && myAuras.Contains("hot streak"))
                {
                    spellToUse = TryUseSpell("Pyroblast", me);
                    if (spellToUse != null) { return spellToUse; }
                }

                // Scorch crit debuff
                if (IsScorchKnown && !targetAuras.Contains("improved scorch"))
                {
                    spellToUse = TryUseSpell("Scorch", me);
                    if (spellToUse != null) { return spellToUse; }
                }

                // Living Bomb
                if (IsLivingBombKnown && !targetAuras.Contains("living bomb"))
                {
                    spellToUse = TryUseSpell("Living Bomb", me);
                    if (spellToUse != null) { return spellToUse; }
                }

                // Spam Forstfire Bolt
                if (IsForstfireBoltKnown)
                {
                    spellToUse = TryUseSpell("Frostfire Bolt", me);
                    if (spellToUse != null) { return spellToUse; }
                }

                // Spam Fireball
                if (IsFireballKnown)
                {
                    spellToUse = TryUseSpell("Fireball", me);
                    if (spellToUse != null) { return spellToUse; }
                }
            }

            return null;
        }

        public void Startup(Me me, Unit target, Unit pet)
        {
        }

        private bool IsArcaneIntellectKnown { get; set; }
        private bool IsEvocationKnown { get; set; }
        private bool IsFireballKnown { get; set; }
        private bool IsForstfireBoltKnown { get; set; }
        private bool IsIceBlockKnown { get; set; }
        private bool IsLivingBombKnown { get; set; }
        private bool IsMageArmorKnown { get; set; }
        private bool IsManaShieldKnown { get; set; }
        private bool IsMirrorImageKnown { get; set; }
        private bool IsMoltenArmorKnown { get; set; }
        private bool IsPyroblastKnown { get; set; }
        private bool IsScorchKnown { get; set; }
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