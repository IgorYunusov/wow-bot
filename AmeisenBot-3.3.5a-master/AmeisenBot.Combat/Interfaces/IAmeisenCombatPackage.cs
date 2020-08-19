using AmeisenBot.Character.Objects;
using System.Collections.Generic;

namespace AmeisenBotCombat.Interfaces
{
    public interface IAmeisenCombatPackage
    {
        IMovementStrategy MovementStrategy { get; }
        List<Spell> Spells { get; }
        ICombatClass SpellStrategy { get; }
    }
}