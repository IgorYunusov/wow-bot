using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using BloogsQuestRedux.Models;

namespace BloogsQuestRedux.Input
{
    public class InputHandler
    {
        private MoveCommand MoveRight { get; set; }
        private MoveCommand MoveLeft { get; set; }
        private MoveCommand MoveDown { get; set; }
        private MoveCommand MoveUp { get; set; }

        public InputHandler()
        {
            MoveRight = new MoveRightCommand();
            MoveLeft = new MoveLeftCommand();
            MoveDown = new MoveDownCommand();
            MoveUp = new MoveUpCommand();
        }

        public void HandleInput(Scene currentScene, KeyboardState keyboardState, GameTime gameTime)
        {
            MovePlayer(currentScene, keyboardState, gameTime);
        }

        private void MovePlayer(Scene currentScene, KeyboardState keyboardState, GameTime gameTime)
        {
            MoveCommand moveCommand;

            if (keyboardState.IsKeyDown(Keys.Right))
            {
                moveCommand = MoveRight;
            }

            else if (keyboardState.IsKeyDown(Keys.Up))
            {
                moveCommand = MoveUp;
            }

            else if (keyboardState.IsKeyDown(Keys.Left))
            {
                moveCommand = MoveLeft;
            }

            else if (keyboardState.IsKeyDown(Keys.Down))
            {
                moveCommand = MoveDown;
            }

            else
                return;

            moveCommand.Move(gameTime, currentScene.Player, currentScene.Map.GetBlockedTiles());
            currentScene.Camera.TryMove(keyboardState, currentScene.Player.Position, moveCommand.GetMovementVector(gameTime), IsMovingHorizontally(keyboardState), IsMovingVertically(keyboardState));                         
        }

        private bool IsMovingHorizontally(KeyboardState keyboardState)
        {
            return keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.Left);
        }

        private bool IsMovingVertically(KeyboardState keyboardState)
        {
            return keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.Down);
        }

        private bool CheckCollision(Scene currentScene, Vector2 movementVector)
        {
            var newPosition = currentScene.Player.Position + movementVector;

            return currentScene.Map.GetBlockedTiles().Any(x => x.BoundingBox.Intersects(new Rectangle((int)newPosition.X, (int)newPosition.Y, 30, 30)));
        }
    }
}