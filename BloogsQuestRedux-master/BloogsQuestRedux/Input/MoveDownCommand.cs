﻿using Microsoft.Xna.Framework;

namespace BloogsQuestRedux.Input
{
    public class MoveDownCommand : MoveCommand
    {
        public override Vector2 GetMovementVector(GameTime gameTime)
        {
            return new Vector2(0, Global.PlayerSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
        }
    }
}