using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoFactory.Core;
using MonoFactory.Entities;
using MonoFactory.Entities.Interfaces;
using MonoFactory.Entities.Machines;
using MonoFactory.Factories;
using MonoFactory.Inputs;
using MonoFactory.Items;
using MonoFactory.Managers;
using MonoFactory.Strategies;
using System.Diagnostics;
using MonoFactory.UI;
using System;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Linq;

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

        private Texture2D _factorySheet;

        private Hero hero;
        private WorldManager world;
        private Camera camera;
        private KeyboardState _prevKeyState;
        private MouseState _prevMouseState;

        private const float InteractionRadius = 200f;
        private ICommand _interactCommand;
        private ICommand _attackCommand;

        private float _attackCooldown = 0f;
        private const float AttackDelay = 0.4f;

        // set target window size
        private const int targetWidth = 1920;
        private const int targetHeight = 1080;

        private EntityFactory _entityFactory;

        private int _currentLevelIndex = 1;

        private bool _showDebugHitboxes = false;

        private Hud _hud;

        private Random _random;

        private Texture2D _portalTexture;
        private bool _portalSpawned = false;

        private SoundManager _soundManager;
        private LevelManager _levelManager;

        private FogManager _fogManager;

        private BossHealthBar _bossHealthBar;

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

            _random = new Random();
        }

        protected override void Initialize()
        {
            _currentState = GameState.Menu;
            _soundManager = new SoundManager();
            _levelManager = new LevelManager();
            base.Initialize();

            camera = new Camera();
        }

        protected override void LoadContent()
        {
            // init spritebatch
            spriteBatch = new SpriteBatch(GraphicsDevice);

            _gameFont = Content.Load<SpriteFont>("GameFont");

            _hud = new Hud(_gameFont, GraphicsDevice);

            _pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            _pixelTexture.SetData(new Color[] { Color.White });


            // load tiles
            Texture2D groundTexture = Content.Load<Texture2D>("tile");
            //hero texture
            _heroTexture = Content.Load<Texture2D>("GoblinKingSpriteSheet");
            // load textures for world
            Texture2D enemyTexture = Content.Load<Texture2D>("Skeleton enemy");
            Texture2D crafterTexture = Content.Load<Texture2D>("crafter");

            _factorySheet = Content.Load<Texture2D>("ore");
            Texture2D stickTexture = Content.Load<Texture2D>("stick");
            Texture2D swordTexture = Content.Load<Texture2D>("swords");
            Texture2D ironBarTexture = Content.Load<Texture2D>("ore");
            Texture2D heartTexture = Content.Load<Texture2D>("heart");


            // init world
            world = new WorldManager(groundTexture, _soundManager);

            _fogManager = new FogManager(GraphicsDevice);
            _fogManager.Radius = 1000f;

            _bossHealthBar = new BossHealthBar(GraphicsDevice);

            // setup factory
            _entityFactory = new EntityFactory();
            _entityFactory.RegisterTexture("Crafter", crafterTexture);
            _entityFactory.RegisterTexture("Goblin_Chaser", enemyTexture);
            _entityFactory.RegisterTexture("Goblin_Patrol", enemyTexture);
            _entityFactory.RegisterTexture("Goblin_Turret", enemyTexture);
            _entityFactory.RegisterTexture("Furnace", _factorySheet);
            _entityFactory.RegisterTexture("Heart", heartTexture);

            // test swords
            _entityFactory.RegisterTexture("Sword_Lvl1", swordTexture);
            _entityFactory.RegisterTexture("Sword_Lvl2", swordTexture);
            _entityFactory.RegisterTexture("Sword_Lvl3", swordTexture);
            _entityFactory.RegisterTexture("Sword_Lvl4", swordTexture);

            _entityFactory.RegisterCreator("Goblin_Chaser", (pos, tex) =>
                new Enemy(tex, pos, new ChaseStrategy(), world, _soundManager, 9));

            _entityFactory.RegisterCreator("Goblin_Patrol", (pos, tex) =>
            {
                Vector2 endPos = pos + new Vector2(200, 0);
                return new Enemy(tex, pos, new PatrolStrategy(pos, endPos), world, _soundManager, 12);
            });

            _entityFactory.RegisterCreator("Goblin_Turret", (pos, tex) =>
                { 
                    var enemy = new Enemy(tex, pos, new StationaryStrategy(), world, _soundManager, 15, canShoot: true);
                    enemy.SetProjectileTexture(_pixelTexture);
                    return enemy;
                });

            _entityFactory.RegisterCreator("Furnace", (pos, tex) =>
                new Machine(tex, pos, world, _factorySheet, "Furnace", ironBarTexture));

            _entityFactory.RegisterCreator("Crafter", (pos, tex) =>
                new Machine(tex, pos, world, _factorySheet, "Crafter", swordTexture));

            _entityFactory.RegisterCreator("Heart", (pos, tex) => new HeartPowerup(tex, pos));

            _entityFactory.RegisterCreator("Boss_Self", (pos, tex) =>
            {
                var strategy = new BossStrategy(hero);
                return new Boss(tex, pos, strategy, world, _soundManager, 50);
            });

            // swords for testing

            int spriteSize = 128;
            int rowY = 4 * 128;

            float visualScale = 1f;

            _entityFactory.RegisterCreator("Sword_Lvl1", (pos, tex) =>
                new DroppedItem(
                    new WeaponItem("Wooden Sword", 1, 2),
                    pos,
                    swordTexture,
                    new Rectangle(0 * spriteSize, rowY, 128, 128),
                    visualScale));

            _entityFactory.RegisterCreator("Sword_Lvl2", (pos, tex) =>
                new DroppedItem(
                    new WeaponItem("Stone Sword", 2, 4),
                    pos,
                    swordTexture,
                    new Rectangle(1 * spriteSize, rowY, 128, 128),
                    visualScale));

            _entityFactory.RegisterCreator("Sword_Lvl3", (pos, tex) =>
                new DroppedItem(
                    new WeaponItem("Iron Sword", 3, 6),
                    pos,
                    swordTexture,
                    new Rectangle(2 * spriteSize, rowY, 128, 128),
                    visualScale));

            _entityFactory.RegisterCreator("Sword_Lvl4", (pos, tex) =>
                new DroppedItem(
                    new WeaponItem("Diamond Sword", 4, 10),
                    pos,
                    swordTexture,
                    new Rectangle(3 * spriteSize, rowY, 128, 128),
                    visualScale));


            //portal
            _pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            _pixelTexture.SetData(new Color[] { Color.White });

            _portalTexture = new Texture2D(GraphicsDevice, 32, 32);
            Color[] portalData = new Color[32 * 32];
            for (int i = 0; i < portalData.Length; i++)
            {
                int x = i % 32;
                int y = i / 32;
                float distFromCenter = Vector2.Distance(new Vector2(x, y), new Vector2(16, 16));
                if (distFromCenter < 14)
                {
                    portalData[i] = Color.White;
                }
                else
                {
                    portalData[i] = Color.Transparent;
                }
            }
            _portalTexture.SetData(portalData);

            //sfx

            Song craftSong = Content.Load<Song>("CrafterTheme");
            Song battleSong = Content.Load<Song>("BattleTheme");

            SoundEffect hitSound = Content.Load<SoundEffect>("HitSound");
            SoundEffect hurtSound = Content.Load<SoundEffect>("HurtSound");

            _soundManager.RegisterSong("Crafting", craftSong);
            _soundManager.RegisterSong("Battle", battleSong);
            _soundManager.RegisterSoundEffect("Hit", hitSound);
            _soundManager.RegisterSoundEffect("Hurt", hurtSound);
        }

        private void LoadLevel(int levelIndex)
        {

            _currentLevelIndex = levelIndex;

            var savedInventory = hero?.Inventory;
            int savedHealth = hero?.Health ?? 10;

            // reset the world for a new game
            Texture2D _tilesetTexture = Content.Load<Texture2D>("tile");
            world = new WorldManager(_tilesetTexture, _soundManager);
            camera = new Camera();
            _interactCommand = new InteractCommand(world);

            _attackCommand = new AttackCommand(world, range: 150f);

            var inputReader = new KeyboardReader();

            Vector2 startPos = GridHelper.GridToWorld(5, 5);
            hero = new Hero(_heroTexture, inputReader, GridHelper.GridToWorld(5, 5), world , _soundManager, scale: 2f);

            if (savedInventory != null && levelIndex > 1)
            {
                foreach (var entry in savedInventory.Items)
                {
                    hero.Inventory.AddItem(entry.Value.Item, entry.Value.Count);
                }

                hero.SetHealth(savedHealth);
            }

            world.AddEntity(hero);
            world.SetEnemyDefeatedCallback(() => OnAllEnemiesDefeated());
            _portalSpawned = false;

            var levelData = _levelManager.GetLevelData(levelIndex);

            if (levelData != null)
            {
                _soundManager.PlayMusic(levelData.MusicTrack);

                if (levelData.ResourceCount > 0)
                {
                    SpawnRandomItems(levelData.ResourceCount);
                }

                Texture2D enemyTex = Content.Load<Texture2D>("Skeleton enemy");
                _levelManager.LoadLevel(levelIndex, world, _entityFactory, enemyTex, _soundManager, _pixelTexture, hero);
            }

            if (levelIndex == 1)
            {
                world.SpawnPortal(GridHelper.GridToWorld(35, 10), _portalTexture);
            }
            if (levelIndex == 7)
            {
                Vector2 bossPos = hero.Position + new Vector2(400, 0);

                var bossStrategy = new BossStrategy(hero);
                var boss = new Boss(_heroTexture, bossPos, bossStrategy, world, _soundManager, 100);

                boss.SetTarget(hero);
                world.AddEntity(boss);
            }
        }

        private void OnAllEnemiesDefeated()
        {
            if (!_portalSpawned && _currentLevelIndex > 1 && _currentLevelIndex < 7)
            {
                Vector2 portalPos = hero.Position + new Vector2(400, 0);
                world.SpawnPortal(portalPos, _portalTexture);
                _portalSpawned = true;
            }
            else if (_currentLevelIndex == 7)
            {
                _currentState = GameState.Victory;
            }
        }

        private void SpawnRandomItems(int count)
        {
            float safeRadius = 500f;
            Texture2D stickTexture = Content.Load<Texture2D>("stick");

            for (int i = 0; i < count; i++)
            {
                Vector2 spawnPos;
                bool valid = false;
                int attempts = 0;

                do
                {
                    float angle = (float)(_random.NextDouble() * Math.PI * 2);
                    float distance = (float)(_random.NextDouble() * 1000) + safeRadius;

                    Vector2 offset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * distance;
                    spawnPos = hero.Position + offset;

                    float dist = Vector2.Distance(spawnPos, hero.Position);
                    if (dist > safeRadius)
                    {
                        valid = true;
                    }
                    attempts++;
                }
                while (!valid && attempts < 10);

                if (valid)
                {
                    bool isIron = _random.Next(2) == 0;
                    string name = isIron ? "Iron Ore" : "Stick";

                    Texture2D itemTex;
                    Rectangle sourceRect;
                    float scale;
                    if (isIron)
                    {
                        itemTex = _factorySheet;
                        sourceRect = new Rectangle(2 * 32, 0 * 32, 32, 32);
                        scale = 1f;
                    }
                    else
                    {
                        itemTex = stickTexture;
                        sourceRect = new Rectangle(0,0,350,350);
                        scale = 64 / 350f;
                    }

                    world.AddEntity(new DroppedItem(new ResourceItem(name), spawnPos, itemTex, sourceRect, scale));
                }
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
            var mState = Mouse.GetState();
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            world.Update(gameTime);

            // camera follows player
            camera.Follow(hero.Position, targetWidth, targetHeight);

            CheckPortalTransition();

            if (kState.IsKeyDown(Keys.R) && !_prevKeyState.IsKeyDown(Keys.R))
            {
                IInteractable nearby = world.GetNearestInteractable(hero.Position, InteractionRadius);

                if (nearby is Machine machine)
                {
                    machine.RemoveLastItem(hero);
                }
            }

            // implementing command pattern
            if (kState.IsKeyDown(Keys.E) && !_prevKeyState.IsKeyDown(Keys.E))
            {
                _interactCommand.Execute(hero);
            }

            if (_attackCooldown > 0)
            {
                _attackCooldown -= delta;
            }

            if (mState.LeftButton == ButtonState.Pressed && _prevMouseState.LeftButton == ButtonState.Released)
            {
                if (_attackCooldown <= 0)
                {
                    if (_attackCommand != null)
                    {
                        hero.TriggerAttack();

                        _attackCommand.Execute(hero);
                        _attackCooldown = AttackDelay;
                    }
                    else
                    {
                        Debug.WriteLine("Attack command is null");
                    }
                }
            }

            if (kState.IsKeyDown(Keys.F3) && !_prevKeyState.IsKeyDown(Keys.F3))
            {
                _showDebugHitboxes = !_showDebugHitboxes;
            }

            _prevKeyState = kState;
            _prevMouseState = mState;
        }

        private void CheckPortalTransition()
        {
            if (world.IsPlayerInPortal(hero))
            {
                if (_currentLevelIndex == 1)
                {
                    LoadLevel(2);
                }
                else if (_currentLevelIndex >= 2 && _currentLevelIndex < 7)
                {
                    LoadLevel(_currentLevelIndex + 1);
                }
                else if (_currentLevelIndex == 7)
                {
                    _currentState = GameState.Victory;
                }
            }
        }

        private void CheckGameOverCondition()
        {
            if (hero.IsDead)
            {
                _currentState = GameState.GameOver;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(0,0,0));

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

                    world.Draw(spriteBatch, camera, GraphicsDevice, _showDebugHitboxes ? _pixelTexture : null);

                    _fogManager.Draw(spriteBatch, hero.Position);

                    if (_showDebugHitboxes)
                    {
                        DrawDebugHitbox(spriteBatch, hero.Rectangle, Color.Red);
                    }


                    IInteractable nearby = world.GetNearestInteractable(hero.Position, InteractionRadius);

                    if (nearby != null)
                    {
                        Vector2 promptPos = nearby.Position - new Vector2(0, 50);
                        spriteBatch.Draw(_pixelTexture, new Rectangle((int)promptPos.X, (int)promptPos.Y, 20, 20), Color.Yellow);
                    }
                    spriteBatch.End();

                    spriteBatch.Begin();

                    if (_hud != null)
                    {
                        _hud.Draw(spriteBatch, hero);
                    }
                    if (_currentLevelIndex == 7)
                    {
                        var boss = world.GetEntities().OfType<Boss>().FirstOrDefault();
                        if (boss != null)
                        {
                            _bossHealthBar.Draw(spriteBatch, boss);
                        }
                    }
                    spriteBatch.End();
                    spriteBatch.Begin();

                    if (_hud != null)
                    {
                        _hud.Draw(spriteBatch, hero);
                    }
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

        public void DrawDebugHitbox(SpriteBatch batch, Rectangle rect, Color color)
        {
            int lineWidth = 2;

            // top
            batch.Draw(_pixelTexture, new Rectangle(rect.X, rect.Y, rect.Width, lineWidth), color);
            //bottom
            batch.Draw(_pixelTexture, new Rectangle(rect.X, rect.Bottom, rect.Width, lineWidth), color);
            //left
            batch.Draw(_pixelTexture, new Rectangle(rect.X, rect.Y, lineWidth, rect.Height), color);
            // right
            batch.Draw(_pixelTexture, new Rectangle(rect.Right, rect.Y, lineWidth, rect.Height), color);
        }
    }
}