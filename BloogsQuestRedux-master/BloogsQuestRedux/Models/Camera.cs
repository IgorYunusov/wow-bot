using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BloogsQuestRedux.Models
{
    public class Camera
    {
        public Vector2 Position { get; set; }

        public Camera()
        {
            Position = new Vector2(0, 0);
        }

        public Matrix GetTransform()
        {
            return Matrix.CreateTranslation(new Vector3(-Position, 0));
        }

        public void TryMove(KeyboardState keyboardState, Vector2 currentPlayerPosition, Vector2 moveVector, bool isMovingHorizontally, bool isMovingVertically)
        {
            if (ShouldMove(keyboardState, currentPlayerPosition, isMovingHorizontally, isMovingVertically))
                Position = Position + moveVector;
        }

        private bool ShouldMove(KeyboardState keyboardState, Vector2 currentPlayerPosition, bool isMovingHorizontally, bool isMovingVertically)
        {
            if (isMovingHorizontally)
            {
                if ((currentPlayerPosition.X > Global.WindowWidth / 2) &&
                    (Position.X + Global.WindowWidth + Global.PlayerSpeed > Global.MapWidth) &&
                    (currentPlayerPosition.X < Global.MapWidth - (Global.WindowWidth / 2)) &&
                    (Position.X - Global.PlayerSpeed < 0))
                        return true;
            }

            if (isMovingVertically)
            {
                if ((currentPlayerPosition.Y > Global.WindowHeight / 2) &&
                    (Position.Y + Global.WindowHeight + Global.PlayerSpeed > Global.MapHeight) &&
                    (currentPlayerPosition.Y < Global.MapHeight - (Global.WindowHeight / 2)) &&
                    (Position.Y - Global.PlayerSpeed < 0))
                        return true;
            }

            return false;
        }
    }
}