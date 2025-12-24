using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoFactory.Core;
using MonoFactory.Entities;
using MonoFactory.Entities.Interfaces;
using MonoFactory.Factories;
using MonoFactory.Inputs;
using MonoFactory.Items;
using MonoFactory.Managers;
using MonoFactory.Strategies;
using System.Collections.Generic;

namespace MonoFactory
{
    public enum GameState
    {
        Menu,
        Playing,
        GameOver,
        Victory
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private SpriteFont _gameFont;

        private GameState _currentState;

        private Texture2D _heroTexture;
        private Texture2D _pixelTexture;

        private Hero hero;
        private WorldManager world;
        private Camera camera;
        private KeyboardState _prevKeyState;

        private const float InteractionRadius = 200f;

        // set target window size
        private const int targetWidth = 1920;
        private const int targetHeight = 1080;

        private EntityFactory _entityFactory;

        private int _currentLevelIndex = 1;

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
            _currentState = GameState.Menu;
            base.Initialize();

            camera = new Camera();
        }

        protected override void LoadContent()
        {
            // init spritebatch
            spriteBatch = new SpriteBatch(GraphicsDevice);

            _gameFont = Content.Load<SpriteFont>("GameFont");
            _pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            _pixelTexture.SetData(new Color[] { Color.White });


            // load grass
            Texture2D grassTexture = Content.Load<Texture2D>("tile_grass");
            //hero texture
            _heroTexture = Content.Load<Texture2D>("GoblinKingSpriteSheet");
            // load textures for world
            Texture2D enemyTexture = Content.Load<Texture2D>("Skeleton enemy");
            Texture2D chestTexture = Content.Load<Texture2D>("chest");

            // init world
            world = new WorldManager(grassTexture);

            // setup factory
            _entityFactory = new EntityFactory();

            _entityFactory.RegisterTexture("Chest", chestTexture);
            _entityFactory.RegisterTexture("Goblin_Chaser", enemyTexture);
            _entityFactory.RegisterTexture("Goblin_Patrol", enemyTexture);
            _entityFactory.RegisterTexture("Goblin_Turret", enemyTexture);

            _entityFactory.RegisterCreator("Chest", (pos, tex) =>
                new Chest(tex, pos));

            _entityFactory.RegisterCreator("Goblin_Chaser", (pos, tex) =>
                new Enemy(tex, pos, new ChaseStrategy(), world));

            _entityFactory.RegisterCreator("Goblin_Patrol", (pos, tex) =>
            {
                Vector2 endPos = pos + new Vector2(200, 0);
                return new Enemy(tex, pos, new PatrolStrategy(pos, endPos), world);
            });

            _entityFactory.RegisterCreator("Goblin_Turret", (pos, tex) =>
                new Enemy(tex, pos, new StationaryStrategy(), world));
        }

        private void LoadLevel(int levelIndex)
        {

            _currentLevelIndex = levelIndex;
            // reset the world for a new game
            world = new WorldManager(Content.Load<Texture2D>("tile_grass"));
            camera = new Camera();

            var inputReader = new KeyboardReader();

            hero = new Hero(_heroTexture, inputReader, GridHelper.GridToWorld(5, 5), world, scale: 2f);
            world.AddEntity(hero);

            if (levelIndex == 1)
            {
                world.AddEntity(_entityFactory.CreateEntity("Chest", GridHelper.GridToWorld(8, 8)));

                IGameObject patroller = _entityFactory.CreateEntity("Goblin_Patrol", GridHelper.GridToWorld(10, 5));
                if (patroller is Enemy e) e.SetTarget(hero);
                world.AddEntity(patroller);
            }
            else if (levelIndex == 2)
            {
                IGameObject chaser1 = _entityFactory.CreateEntity("Goblin_Chaser", GridHelper.GridToWorld(15, 2));
                IGameObject chaser2 = _entityFactory.CreateEntity("Goblin_Chaser", GridHelper.GridToWorld(20, 10));
                IGameObject turret = _entityFactory.CreateEntity("Goblin_Turret", GridHelper.GridToWorld(12, 12));

                if (chaser1 is Enemy e1) e1.SetTarget(hero);
                if (chaser2 is Enemy e2) e2.SetTarget(hero);
                if (turret is Enemy e3) e3.SetTarget(hero);

                world.AddEntity(chaser1);
                world.AddEntity(chaser2);
                world.AddEntity(turret);

                world.AddEntity(_entityFactory.CreateEntity("Chest", GridHelper.GridToWorld(22, 10)));
            }
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var kState = Keyboard.GetState();

            switch(_currentState)
            {
                case GameState.Menu:
                    if (kState.IsKeyDown(Keys.Enter))
                    {
                        LoadLevel(1);
                        _currentState = GameState.Playing;
                    }
                    break;
                case GameState.Playing:
                    UpdateGamePlay(gameTime);
                    CheckGameOverCondition();
                    break;
                case GameState.GameOver:
                case GameState.Victory:
                    if (kState.IsKeyDown(Keys.Enter))
                    {
                        _currentState = GameState.Menu;
                    }
                    break;
            }

            base.Update(gameTime);
        }

        private void UpdateGamePlay(GameTime gameTime)
        {
            var kState = Keyboard.GetState();

            world.Update(gameTime);
            hero.Update(gameTime);

            // camera follows player
            camera.Follow(hero.Position, targetWidth, targetHeight);

            CheckLevelTransition();

            if (kState.IsKeyDown(Keys.E) && !_prevKeyState.IsKeyDown(Keys.E))
            {
                IInteractable machine = world.GetNearestInteractable(hero.Position, InteractionRadius);
                if (machine != null)
                {
                    machine.Interact(hero);
                }
            }

            _prevKeyState = kState;
        }

        private void CheckLevelTransition()
        {
            if (hero.Position.X > 1000)
            {
                if (_currentLevelIndex == 1)
                {
                    LoadLevel(2);
                }
                else
                {
                    _currentState = GameState.Victory;
                }
            }
        }

        private void CheckGameOverCondition()
        {
            // TODO: add actual thing later
            if (hero.Position.Y > 2000)
            {
                _currentState = GameState.GameOver;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            switch (_currentState)
            {
                case GameState.Menu:
                    DrawTextCentered("MONO FACTORY", -50);
                    DrawTextCentered("Press ENTER to start", 50);
                    break;
                case GameState.Playing:
                    spriteBatch.End();

                    // matrix to squish Y axis
                    Matrix tiltMatrix = Matrix.CreateScale(1.0f, 0.6f, 1.0f);

                    // combine camera and tilt
                    Matrix groundTransform = camera.Transform * tiltMatrix;

                    spriteBatch.Begin(transformMatrix: groundTransform, samplerState: SamplerState.PointClamp);

                    world.Draw(spriteBatch, camera, GraphicsDevice);

                    hero.Draw(spriteBatch);

                    IInteractable nearby = world.GetNearestInteractable(hero.Position, InteractionRadius);

                    if (nearby != null)
                    {
                        Vector2 promptPos = nearby.Position - new Vector2(0, 50);
                        spriteBatch.Draw(_pixelTexture, new Rectangle((int)promptPos.X, (int)promptPos.Y, 20, 20), Color.Yellow);
                    }
                    spriteBatch.End();
                    spriteBatch.Begin();
                    break;
                case GameState.GameOver:
                    GraphicsDevice.Clear(Color.Black);
                    DrawTextCentered("GAME OVER", 0, Color.Red);
                    DrawTextCentered("Press ENTER to Main Menu", 50);
                    break;
            }
            spriteBatch.End();
        }

        private void DrawTextCentered(string text, float offsetY, Color? color = null)
        {
            if (_gameFont == null)
            {
                return;
            }

            Vector2 size = _gameFont.MeasureString(text);
            Vector2 center = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            Vector2 pos = center - size / 2 + new Vector2(0, offsetY);

            spriteBatch.DrawString(_gameFont, text, pos, color ?? Color.White);
        }
    }
}