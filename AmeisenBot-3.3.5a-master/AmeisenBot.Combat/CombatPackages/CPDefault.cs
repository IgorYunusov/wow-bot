using AmeisenBot.Character.Objects;
using AmeisenBotCombat.Interfaces;
using System.Collections.Generic;

namespace AmeisenBotCombat.CombatPackages
{
    public class CPDefault : IAmeisenCombatPackage
    {
        public CPDefault(List<Spell> spells, ICombatClass spellStrategy, IMovementStrategy movementStrategy)
        {
            Spells = spells;
            SpellStrategy = spellStrategy;
            MovementStrategy = movementStrategy;
        }

        public IMovementStrategy MovementStrategy { get; private set; }
        public List<Spell> Spells { get; private set; }
        public ICombatClass SpellStrategy { get; private set; }
    }
}