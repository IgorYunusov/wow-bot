using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;

namespace BloogsQuestRedux.Models
{
    public abstract class Actor
    {
        public abstract Vector2 Position { get; set; }
        public abstract Rectangle BoundingBox { get; }

        public bool CollisionDetected(IEnumerable<Tile> blockedTiles, Vector2 movementVector)
        {
            var newPosition = Position + movementVector;

            return blockedTiles.Any(x => x.BoundingBox.Intersects(new Rectangle((int)newPosition.X, (int)newPosition.Y, 30, 30)));
        }
    }
}