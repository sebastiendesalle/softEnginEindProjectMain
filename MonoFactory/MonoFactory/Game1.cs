using MonoFactory.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoFactory;
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
        private Camera camera;
        private KeyboardState _prevKeyState;
        private Texture2D pixelTexture;

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

            // load texture
            Texture2D grassTexture = Content.Load<Texture2D>("tile_grass");

            // init world
            world = new WorldManager(grassTexture);

            camera = new Camera();

            Texture2D chestTex = Content.Load<Texture2D>("GoblinKingSpriteSheet");
            Vector2 chestPos = GridHelper.GridToWorld(8, 8);
            world.AddMachine(new Chest(chestTex, chestPos));
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            heroTexture = Content.Load<Texture2D>("GoblinKingSpriteSheet");

            pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            pixelTexture.SetData(new Color[] { Color.White });

            // init input
            var inputReader = new KeyboardReader();

            // init hero
            hero = new Hero(heroTexture, inputReader, new Vector2(900, 500), scale: 2f);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            hero.Update(gameTime);

            // camera follows player
            camera.Follow(hero.Position, targetWidth, targetHeight);

            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.E) && !_prevKeyState.IsKeyDown(Keys.E))
            {
                IInteractable machine = world.GetNearestMachine(hero.Position, 100f);

                if (machine != null)
                {
                    machine.Interact(hero);
                }
            }

            _prevKeyState = state;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // matrix to squish Y axis
            Matrix tiltMatrix = Matrix.CreateScale(1.0f, 0.6f, 1.0f);

            // combine camera and tilt
            Matrix groundTransform = camera.Transform * tiltMatrix;

            spriteBatch.Begin(transformMatrix: groundTransform, samplerState: SamplerState.PointClamp);

            world.Draw(spriteBatch, camera, GraphicsDevice);

            world.DrawMachines(spriteBatch);

            hero.Draw(spriteBatch);

            IInteractable nearby = world.GetNearestMachine(hero.Position, 100f);

            if (nearby != null)
            {
                Vector2 promptPos = nearby.Position - new Vector2(0, 50);
                spriteBatch.Draw(pixelTexture, new Rectangle((int)promptPos.X, (int)promptPos.Y, 20, 20), Color.Yellow);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}