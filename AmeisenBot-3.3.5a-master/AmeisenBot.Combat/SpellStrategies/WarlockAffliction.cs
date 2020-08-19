using AmeisenBot.Character.Objects;
using AmeisenBotCombat.Interfaces;
using AmeisenBotCore;
using AmeisenBotUtilities;
using System.Collections.Generic;
using System.Linq;

namespace AmeisenBotCombat.SpellStrategies
{
    public class WarlockAffliction : ICombatClass
    {
        public WarlockAffliction(List<Spell> spells)
        {
            Spells = spells;

            IsFelArmorKnown = Spells.Where(spell => spell.Name == "Fel Armor").ToList().Count > 0;
            IsDemonArmorKnown = Spells.Where(spell => spell.Name == "Demon Armor").ToList().Count > 0;
            IsDemonSkinKnown = Spells.Where(spell => spell.Name == "Demon Skin").ToList().Count > 0;
            IsShadowBoltKnown = Spells.Where(spell => spell.Name == "Shadow Bolt").ToList().Count > 0;
            IsDrainSoulKnown = Spells.Where(spell => spell.Name == "Drain Soul").ToList().Count > 0;
            IsHauntKnown = Spells.Where(spell => spell.Name == "Haunt").ToList().Count > 0;
            IsUnstableAfflictionKnown = Spells.Where(spell => spell.Name == "Unstable Affliction").ToList().Count > 0;
            IsCorruptionKnown = Spells.Where(spell => spell.Name == "Corruption").ToList().Count > 0;
            IsLifeTapKnown = Spells.Where(spell => spell.Name == "Life Tap").ToList().Count > 0;
            IsFearKnown = Spells.Where(spell => spell.Name == "Fear").ToList().Count > 0;
            IsCurseOfAgonyKnown = Spells.Where(spell => spell.Name == "Curse of Agony").ToList().Count > 0;
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
            if (IsFelArmorKnown)
            {
                if (!myAuras.Contains("fel armor"))
                {
                    spellToUse = TryUseSpell("Fel Armor", me);
                    if (spellToUse != null) { return spellToUse; }
                }
            }
            else if (IsDemonArmorKnown)
            {
                if (!myAuras.Contains("demon armor"))
                {
                    spellToUse = TryUseSpell("Demon Armor", me);
                    if (spellToUse != null) { return spellToUse; }
                }
            }
            else if (IsDemonSkinKnown)
            {
                if (!myAuras.Contains("demon skin"))
                {
                    spellToUse = TryUseSpell("Demon Skin", me);
                    if (spellToUse != null) { return spellToUse; }
                }
            }

            // the Imp
            /*if (IsFelArmorKnown && me.PetGuid == 0)
            {
                spellToUse = TryUseSpell("Summon Imp", me);
                if (spellToUse != null) { return spellToUse; }
            }*/

            // if we go under 50% Mana and above 80% Health use Life Tap
            if (IsLifeTapKnown && me.HealthPercentage > 80 && me.ManaPercentage < 50)
            {
                spellToUse = TryUseSpell("Life Tap", me);
                if (spellToUse != null) { return spellToUse; }
            }

            // fear
            if (targetDistance < 5)
            {
                // try to Fear it away lmao
                if (IsFearKnown)
                {
                    spellToUse = TryUseSpell("Fear", me);
                    if (spellToUse != null) { return spellToUse; }
                }
            }

            // main spell rotation
            if (targetDistance < 30)
            {
                // apply Haunt DOT
                if (IsHauntKnown && !targetAuras.Contains("haunt"))
                {
                    spellToUse = TryUseSpell("Haunt", me);
                    return spellToUse;
                }

                // apply Unstable Affliction DOT
                if (IsUnstableAfflictionKnown && !targetAuras.Contains("unstable affliction"))
                {
                    spellToUse = TryUseSpell("Unstable Affliction", me);
                    return spellToUse;
                }

                // apply Corruption DOT
                if (IsCorruptionKnown && !targetAuras.Contains("corruption"))
                {
                    spellToUse = TryUseSpell("Corruption", me);
                    return spellToUse;
                }

                // apply Curse of Agony DOT
                if (IsCurseOfAgonyKnown && !targetAuras.Contains("curse of agony"))
                {
                    spellToUse = TryUseSpell("Curse of Agony", me);
                    return spellToUse;
                }

                // if target is over 25% use Shadow Bolt otherwise use Drain Soul
                if (target.HealthPercentage > 25)
                {
                    if (IsShadowBoltKnown)
                    {
                        spellToUse = TryUseSpell("Shadow Bolt", me);
                        if (spellToUse != null) { return spellToUse; }
                    }
                }
                else
                {
                    if (IsDrainSoulKnown)
                    {
                        spellToUse = TryUseSpell("Drain Soul", me);
                        if (spellToUse != null) { return spellToUse; }
                    }
                }
            }

            return null;
        }

        public void Startup(Me me, Unit target, Unit pet)
        {
        }

        private bool IsCorruptionKnown { get; set; }
        private bool IsCurseOfAgonyKnown { get; set; }
        private bool IsDemonArmorKnown { get; set; }
        private bool IsDemonSkinKnown { get; set; }
        private bool IsDrainSoulKnown { get; set; }
        private bool IsFearKnown { get; set; }
        private bool IsFelArmorKnown { get; set; }
        private bool IsHauntKnown { get; set; }
        private bool IsLifeTapKnown { get; set; }
        private bool IsShadowBoltKnown { get; set; }
        private bool IsUnstableAfflictionKnown { get; set; }
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