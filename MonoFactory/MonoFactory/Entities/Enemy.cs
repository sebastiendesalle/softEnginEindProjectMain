using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoFactory.Components.Animation;
using MonoFactory.Strategies;
using MonoFactory.Entities.Interfaces;
using MonoFactory.Managers;
using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace MonoFactory.Entities
{
    public class Enemy: IGameObject, IDamageable
    {
        public Vector2 Position { get; private set; }
        private Texture2D _texture;

        private WorldManager _world;

        private Dictionary<string, Animation> _animations;
        private Animation _currentAnimation;

        private float _shootCooldown = 0f;
        private const float ShootDelay = 2.0f;
        private const float ShootRange = 500f;
        private bool _canShoot;
        private Texture2D _projectileTexture;

        private int _hitBoxWidth;
        private int _hitBoxHeight;

        private int _offsetX = 0;
        private int _offsetY = 0;

        private SpriteEffects _flipEffect = SpriteEffects.None;

        private IMovementStrategy _movementStrategy;
        private Hero _targetHero;

        private Color _tintColor = Color.White;
        private float _damageFlashTimer = 0f;

        public int Health { get; private set; } = 9;

        private const float Scale = 3.0f;

        private float _damageCooldown = 0f;
        private const float DamageDelay = 1.0f;

        private const float AttackRange = 100.0f;

        private Random _random;

        private bool _isAttacking = false;
        private bool _isDead = false;
        private bool _isVisible = true;

        public Enemy(Texture2D texture, Vector2 startPosition, IMovementStrategy strategy, WorldManager world, int health = 9, bool canShoot = false)
        {
            _texture = texture;
            Position = startPosition;
            _movementStrategy = strategy;
            _world = world;
            Health = health;
            _canShoot = canShoot;

            _random = new Random();

            LoadAnimations();

            _hitBoxWidth = (int)(20 * Scale);
            _hitBoxHeight = (int)(40 * Scale);

            _offsetY = -(int)(10 * Scale);
        }

        public void LoadAnimations()
        {
            _animations = new Dictionary<string, Animation>();

            var idleAnim = new Animation();
            for (int i = 0; i < 4; i++)
            {
                idleAnim.AddFrame(new AnimationFrame(new Rectangle(i * 64, 192, 64, 64)));
            }
            _animations.Add("Idle", idleAnim);

            var runAnim = new Animation();
            for (int i = 0; i < 12; i++)
            {
                runAnim.AddFrame(new AnimationFrame(new Rectangle(i * 64, 128, 64, 64)));
            }
            _animations.Add("Run", runAnim);

            var attack1 = new Animation();
            attack1.IsLooping = false;
            attack1.FPS = 12;
            for (int i = 0; i < 7; i++)
            {
                attack1.AddFrame(new AnimationFrame(new Rectangle(i * 64, 0, 64, 64)));
            }
            _animations.Add("Attack1", attack1);

            var attack2 = new Animation();
            attack2.IsLooping = false;
            attack2.FPS = 12;
            for (int i = 7; i < 13; i++)
            {
                attack2.AddFrame(new AnimationFrame(new Rectangle(i * 64, 0, 64, 64)));
            }
            _animations.Add("Attack2", attack2);

            var deathAnim = new Animation();
            deathAnim.IsLooping = false;
            deathAnim.FPS = 10;
            for (int i = 0; i < 13; i++)
            {
                deathAnim.AddFrame(new AnimationFrame(new Rectangle(i * 64, 64, 64, 64)));
            }
            _animations.Add("Death", deathAnim);

            _currentAnimation = _animations["Idle"];
        }

        public void SetTarget(Hero hero)
        {
            _targetHero = hero;
        }

        public Rectangle Rectangle
        {
            get
            {
                if (!_isVisible)
                {
                    return Rectangle.Empty;
                }
                return new Rectangle((int)(Position.X - _hitBoxWidth / 2) + _offsetX, (int)(Position.Y - _hitBoxHeight) + _offsetY, _hitBoxWidth, _hitBoxHeight);
            }
        }

        public void Update(GameTime gameTime)
        {

            if (!_isVisible)
            {
                return;
            }
            Vector2 previousPos = Position;

            Vector2 desiredPosition = Position;

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_isDead)
            {
                _currentAnimation.Update(gameTime);
                if (_currentAnimation.IsFinished)
                {
                    _isVisible = false;

                    Position = new Vector2(-9000, -9000);
                }
                return;
            }

            if (_isAttacking)
            {
                _currentAnimation.Update(gameTime);
                if (_currentAnimation.IsFinished)
                {
                    _isAttacking = false;
                    _currentAnimation = _animations["Idle"];
                }
                return;
            }

            if (_damageCooldown > 0)
            {
                _damageCooldown -= deltaTime;
            }

            if (_damageFlashTimer > 0)
            {
                _damageFlashTimer -= deltaTime;
                _tintColor = Color.Red;
            }
            else
            {
                _tintColor = Color.White;
            }

            // move via strategy
            if (_movementStrategy != null)
            {
                Vector2 targetPos = _targetHero != null ? _targetHero.Position : Position;
                desiredPosition = _movementStrategy.Move(gameTime, Position, targetPos);
            }

            Vector2 delta = desiredPosition - Position;

            Rectangle futureRectX = new Rectangle((int)(Position.X + delta.X - _hitBoxWidth / 2) + _offsetX, (int)(Position.Y - _hitBoxHeight) + _offsetY, _hitBoxWidth, _hitBoxHeight);

            if (!_world.IsCollision(futureRectX, this))
            {
                Position = new Vector2(Position.X + delta.X, Position.Y);
            }

            Rectangle futureRectY = new Rectangle((int)(Position.X - _hitBoxWidth / 2) + _offsetX, (int)(Position.Y + delta.Y - _hitBoxHeight) + _offsetY, _hitBoxWidth, _hitBoxHeight);

            if (!_world.IsCollision(futureRectY, this))
            {
                Position = new Vector2(Position.X, Position.Y + delta.Y);
            }

            Vector2 movement = Position - previousPos;
            if (movement.Length() > 0.1f)
            {
                _currentAnimation = _animations["Run"];

                if (movement.X > 0)
                {
                    _flipEffect = SpriteEffects.None;
                }
                else if (movement.X < 0)
                {
                    _flipEffect = SpriteEffects.FlipHorizontally;
                }
            }
            else
            {
                _currentAnimation = _animations["Idle"];
            }

            // update animation
            _currentAnimation.Update(gameTime);

            if (_targetHero != null)
            {
                float dist = Vector2.Distance(Position, _targetHero.Position);
                if (dist < AttackRange && _damageCooldown <= 0)
                {
                    _isAttacking = true;

                    if (_targetHero.Position.X < Position.X)
                    {
                        _flipEffect = SpriteEffects.FlipHorizontally;
                    }
                    else
                    {
                        _flipEffect = SpriteEffects.None;
                    }
                    if (_random.Next(0, 2) == 0)
                    {
                        _currentAnimation = _animations["Attack1"];
                    }
                    else
                    {
                        _currentAnimation = _animations["Attack2"];
                    }

                    _currentAnimation.Reset();

                    _targetHero.TakeDamage(1);
                    _damageCooldown = DamageDelay;
                }
            }

            if (_canShoot && _targetHero != null && _projectileTexture != null)
            {
                if (_shootCooldown > 0)
                {
                    _shootCooldown -= deltaTime;
                }

                float dist = Vector2.Distance(Position, _targetHero.Position);
                if (dist < ShootRange && _shootCooldown <= 0)
                {
                    if (_targetHero.Position.X < Position.X)
                    {
                        _flipEffect = SpriteEffects.FlipHorizontally;
                    }
                    else
                    {
                        _flipEffect = SpriteEffects.None;
                    }

                    Vector2 projectStart = Position;
                    Vector2 targetPosition = _targetHero.Position;

                    Projectile Projectile = new Projectile(
                        projectStart,
                        targetPosition,
                        1,
                        this,
                        _projectileTexture,
                        Color.Red
                        );

                    _world.AddEntity(Projectile);
                    _shootCooldown = ShootDelay;

                    Debug.WriteLine($"Enemy turret fired projectile at {targetPosition}");
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {

            if (!_isVisible)
            {
                return;
            }

            Rectangle src = _currentAnimation.CurrentFrame.SourceRectangle;

            Vector2 origin = new Vector2(src.Width / 2f, src.Height);
            spriteBatch.Draw(_texture, 
                Position,
                src,
                _tintColor,
                0f,
                origin,
                Scale,
                _flipEffect,
                0f);
        }

        public void TakeDamage(int amount)
        {

            if (_isDead)
            {
                return;
            }

            Health -= amount;

            _damageFlashTimer = 0.2f;
            if (Health <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            _isDead = true;
            _currentAnimation = _animations["Death"];
            _currentAnimation.Reset();
        }

        public void SetProjectileTexture(Texture2D texture)
        {
            _projectileTexture = texture;
        }
    }
}
