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

        private Texture2D _mainSpriteSheet;
        private Texture2D _pixelTexture;

        private Hero hero;
        private WorldManager world;
        private Camera camera;
        private KeyboardState _prevKeyState;

        // set target window size
        private const int targetWidth = 1920;
        private const int targetHeight = 1080;

        private EntityFactory _entityFactory;


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

            camera = new Camera();
        }

        protected override void LoadContent()
        {
            // init spritebatch
            spriteBatch = new SpriteBatch(GraphicsDevice);
            _pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            _pixelTexture.SetData(new Color[] { Color.White });


            // load grass
            Texture2D grassTexture = Content.Load<Texture2D>("tile_grass");
            // load textures for world
            Texture2D enemyTexture = Content.Load<Texture2D>("GoblinKingSpriteSheet");
            Texture2D chestTexture = Content.Load<Texture2D>("GoblinKingSpriteSheet"); // TODO: change to chest png

            // init world
            world = new WorldManager(grassTexture);

            // setup factory
            _entityFactory = new EntityFactory();

            _entityFactory.RegisterTexture("Chest", chestTexture);
            _entityFactory.RegisterTexture("Goblin_Chaser", enemyTexture);
            _entityFactory.RegisterTexture("Goblin_Patrol", enemyTexture);
            _entityFactory.RegisterTexture("Goblin_Turret", enemyTexture);

            // chests
            world.AddEntity(_entityFactory.CreateEntity("Chest", GridHelper.GridToWorld(8, 8)));
            //world.AddEntity(_entityFactory.CreateEntity("Chest", new Vector2(200, 200)));

            // hero
            var inputReader = new KeyboardReader();
            hero = new Hero(_mainSpriteSheet, inputReader, new Vector2(900, 500), scale: 2f);

            // enemies
            IGameObject chaser = _entityFactory.CreateEntity("Goblin_Chaser", new Vector2(400, 400));
            IGameObject patroller = _entityFactory.CreateEntity("Goblin_Patrol", new Vector2(600, 200));
            IGameObject turret = _entityFactory.CreateEntity("Goblin_Turret", new Vector2(800, 600));

            // set targets
            if (chaser is Enemy e1)
            {
                e1.SetTarget(hero);
            }
            if (patroller is Enemy e2)
            {
                e2.SetTarget(hero);
            }

            world.AddEntity(chaser);
            world.AddEntity(patroller);
            world.AddEntity(turret);
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            world.Update(gameTime);
            hero.Update(gameTime);

            // camera follows player
            camera.Follow(hero.Position, targetWidth, targetHeight);

            if (state.IsKeyDown(Keys.E) && !_prevKeyState.IsKeyDown(Keys.E))
            {
                IInteractable machine = world.GetNearestInteractable(hero.Position, 100f);
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

            hero.Draw(spriteBatch);

            IInteractable nearby = world.GetNearestInteractable(hero.Position, 100f);

            if (nearby != null)
            {
                Vector2 promptPos = nearby.Position - new Vector2(0, 50);
                spriteBatch.Draw(_pixelTexture, new Rectangle((int)promptPos.X, (int)promptPos.Y, 20, 20), Color.Yellow);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}