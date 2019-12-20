using System.Collections.Generic;

using BloogsQuestRedux.Models;

using Microsoft.Xna.Framework;

namespace BloogsQuestRedux.Input
{
    public abstract class MoveCommand
    {
        public abstract Vector2 GetMovementVector(GameTime gameTime);

        public void Move(GameTime gameTime, Actor actor, IEnumerable<Tile> blockedTiles)
        {
            if (!actor.CollisionDetected(blockedTiles, GetMovementVector(gameTime)))
                actor.Position = actor.Position + GetMovementVector(gameTime);
        }
    }
}