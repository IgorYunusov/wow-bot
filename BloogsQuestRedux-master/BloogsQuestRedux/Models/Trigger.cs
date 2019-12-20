using Microsoft.Xna.Framework;

namespace BloogsQuestRedux.Models
{
    public class Trigger
    {
        public Rectangle BoundingBox { get; private set; }

        public Trigger(Rectangle boundingBox)
        {
            BoundingBox = boundingBox;
        }
    }
}