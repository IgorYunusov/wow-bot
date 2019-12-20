using Microsoft.Xna.Framework;

namespace BloogsQuestRedux.Input
{
    public class MoveRightCommand : MoveCommand
    {
        public override Vector2 GetMovementVector(GameTime gameTime)
        {
            return new Vector2(Global.PlayerSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds, 0);
        }
    }
}