using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoFactory.Components.Animation;
using MonoFactory.Strategies;
using MonoFactory.Entities.Interfaces;
using MonoFactory.Managers;
using System.Collections.Generic;
using System.Diagnostics;

namespace MonoFactory.Entities
{
    public class Enemy: IGameObject, IDamageable
    {
        public Vector2 Position { get; private set; }
        private Texture2D _texture;

        private WorldManager _world;

        private Dictionary<string, Animation> _animations;
        private Animation _currentAnimation;

        private int _hitBoxWidth;
        private int _hitBoxHeight;

        private int _offsetX = 0;
        private int _offsetY = 0;

        private SpriteEffects _flipEffect = SpriteEffects.None;

        private IMovementStrategy _movementStrategy;
        private Hero _targetHero;

        public int Health { get; private set; } = 3;

        private const float Scale = 3.0f;

        private float _damageCooldown = 0f;
        private const float DamageDelay = 1.0f;

        private const float AttackRange = 75.0f;

        public Enemy(Texture2D texture, Vector2 startPosition, IMovementStrategy strategy, WorldManager world)
        {
            _texture = texture;
            Position = startPosition;
            _movementStrategy = strategy;
            _world = world;

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

            _currentAnimation = _animations["Idle"];
        }

        public void SetTarget(Hero hero)
        {
            _targetHero = hero;
        }

        public Rectangle Rectangle => new Rectangle((int)(Position.X - _hitBoxWidth / 2) + _offsetX, (int)(Position.Y - _hitBoxHeight) + _offsetY, _hitBoxWidth, _hitBoxHeight);

        public void Update(GameTime gameTime)
        {
            Vector2 previousPos = Position;

            Vector2 desiredPosition = Position;

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

            if (_damageCooldown > 0)
            {
                _damageCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (_targetHero == null)
            {
                Debug.WriteLine("no target hero set");
            }
            else
            {
                float dist = Vector2.Distance(Position, _targetHero.Position);
                if (dist < AttackRange)
                {
                    if (_damageCooldown <= 0)
                    {
                        Debug.WriteLine($"Dealing damage to hero. Dist: {dist}");
                        _targetHero.TakeDamage(1);
                        _damageCooldown = DamageDelay;
                    }
                    else
                    {
                        Debug.WriteLine("In range, but cooldown active");
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {

            Rectangle src = _currentAnimation.CurrentFrame.SourceRectangle;

            Vector2 origin = new Vector2(src.Width / 2f, src.Height);
            spriteBatch.Draw(_texture, 
                Position,
                src,
                Color.White,
                0f,
                origin,
                Scale,
                _flipEffect,
                0f);
        }

        public void TakeDamage(int amount)
        {
            // TODO: implement logic for taking damage from enemy
            Health -= amount;
            if (Health <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            Position = new Vector2(-9000, -9000);
        }
    }
}
