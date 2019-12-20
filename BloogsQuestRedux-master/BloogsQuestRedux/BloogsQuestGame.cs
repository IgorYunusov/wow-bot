using System;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using System.Collections.Generic;
using BloogsQuestRedux.Models;
using BloogsQuestRedux.Services;
using BloogsQuestRedux.Input;

namespace BloogsQuestRedux
{
    public class BloogsQuestGame : Game
    {
        private GameService gameService { get; set; }
        private Scene currentScene { get; set; }
        private InputHandler inputHandler { get; set; }

        protected Song song;

        // XNA Dependencies
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        List<TilePrototype> tilePrototypes;

        public BloogsQuestGame()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = Global.WindowWidth;
            graphics.PreferredBackBufferHeight = Global.WindowHeight;
            this.Content.RootDirectory = "Content";
            this.IsFixedTimeStep = false;
        }

        private void LoadScene(Scene scene)
        {
            currentScene = scene;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            gameService = new GameService();
            inputHandler = new InputHandler();

            tilePrototypes = gameService.GetTilePrototypes();

            var gameMap = new Map(100, 50, 50, tilePrototypes);
            var player = new Player(new Vector2(Global.WindowWidth / 2, Global.WindowHeight / 2));
            var camera = new Camera();            

            LoadScene(new Scene(gameMap, player, camera));

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(this.GraphicsDevice);

            foreach (var tilePrototype in tilePrototypes)
            {
                tilePrototype.Sprite.LoadContent(this.Content, tilePrototype.TextureFilename);
            }
            currentScene.Player.Sprite.LoadContent(this.Content, currentScene.Player.TextureFilename);

            //song = Content.Load<Song>("Canon");
            //MediaPlayer.Play(song);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            inputHandler.HandleInput(currentScene, Keyboard.GetState(), gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            this.GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, currentScene.Camera.GetTransform());
            currentScene.Map.Draw(spriteBatch, currentScene.Camera.Position);
            spriteBatch.Draw(currentScene.Player.Sprite.Texture, new Vector2(currentScene.Player.Position.X, currentScene.Player.Position.Y), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
