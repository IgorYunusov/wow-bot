using AmeisenBotCombat.Interfaces;
using AmeisenBotUtilities;

namespace AmeisenBotCombat.MovementStrategies
{
    public class MovementClose : IMovementStrategy
    {
        public MovementClose(double distance = 3.0)
        {
            Distance = distance;
        }

        public Vector3 CalculatePosition(Me me, Unit target)
        {
            if (target != null && Utils.GetDistance(me.pos, target.pos) > Distance)
            {
                return target.pos;
            }

            return me.pos;
        }

        private double Distance { get; set; }
    }
}