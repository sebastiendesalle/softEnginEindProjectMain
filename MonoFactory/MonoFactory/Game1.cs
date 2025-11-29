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
        private Texture2D pixelTexture; // For debugging walls
        private Hero hero;

        // set target window size
        private const int targetWidth = 1920;
        private const int targetHeight = 1080;

        private Matrix _globalTransformation;
        private List<Rectangle> _obstacles;

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
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            heroTexture = Content.Load<Texture2D>("GoblinKingSpriteSheet");

            // create white pixel texture to draw walls
            pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            pixelTexture.SetData(new Color[] { Color.White });

            // make and place obstacles
            _obstacles = new List<Rectangle>();
            _obstacles.Add(new Rectangle(0, 400, 800, 50));   // Main Floor
            _obstacles.Add(new Rectangle(200, 300, 200, 30)); // Floating Platform
            _obstacles.Add(new Rectangle(500, 200, 50, 200)); // Tall Wall

            // input and bounds
            var inputReader = new KeyboardReader();
            var viewport = GraphicsDevice.Viewport;
            var playArea = new Rectangle(0, 0, targetWidth, targetHeight);

            // create hero
            hero = new Hero(
                heroTexture,
                inputReader,
                _obstacles,
                playArea,
                new Vector2(50, 50), // Start Position
                scale: 2f //hero scale
            );
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

            // draw the obstacles so they aren't inviz
            foreach (var rect in _obstacles)
            {
                spriteBatch.Draw(pixelTexture, rect, Color.Black);
            }

            hero.Draw(spriteBatch);

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}