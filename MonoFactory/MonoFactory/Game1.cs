using MonoFactory.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoFactory;
using MonoFactory.Input;
using System.Collections.Generic;

namespace MonoFactory
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Texture2D heroTexture;
        private Hero hero;
        private WorldManager world;

        // set target window size
        private const int targetWidth = 1920;
        private const int targetHeight = 1080;

        private Matrix _globalTransformation;


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            // set windows size to windowedfullscreen compatible for all
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.HardwareModeSwitch = false;
            graphics.IsFullScreen = true;

            IsMouseVisible = true;

            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();

            // scaling
            float scaleX = (float)GraphicsDevice.Viewport.Width / targetWidth;
            float scaleY = (float)GraphicsDevice.Viewport.Height / targetHeight;

            _globalTransformation = Matrix.CreateScale(scaleX, scaleX, 1.0f);

            // load texture
            Texture2D grassTexture = Content.Load<Texture2D>("tile_grass");

            // init world
            world = new WorldManager(20, 20); // 20x20 grid
            world.GenerateWorld(grassTexture);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            heroTexture = Content.Load<Texture2D>("GoblinKingSpriteSheet");

            // init input
            var inputReader = new KeyboardReader();

            // init hero
            hero = new Hero(heroTexture, inputReader, new Vector2(300, 300), scale: 3f);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            hero.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(transformMatrix: _globalTransformation, samplerState: SamplerState.PointClamp);

            hero.Draw(spriteBatch);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}