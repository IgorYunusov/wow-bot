using AmeisenBotUtilities;

namespace AmeisenBotCombat.Interfaces
{
    public interface IMovementStrategy
    {
        Vector3 CalculatePosition(Me me, Unit target);
    }
}