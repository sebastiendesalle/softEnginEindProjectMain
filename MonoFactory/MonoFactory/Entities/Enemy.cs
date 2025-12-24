using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoFactory.Components.Animation;
using MonoFactory.Strategies;
using MonoFactory.Entities.Interfaces;
using MonoFactory.Managers;
using System.Collections.Generic;

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

        private SpriteEffects _flipEffect = SpriteEffects.None;

        private IMovementStrategy _movementStrategy;
        private Hero _targetHero;

        public int Health { get; private set; } = 3;

        private const float Scale = 3.0f;

        public Enemy(Texture2D texture, Vector2 startPosition, IMovementStrategy strategy, WorldManager world)
        {
            _texture = texture;
            Position = startPosition;
            _movementStrategy = strategy;
            _world = world;

            LoadAnimations();

            _hitBoxWidth = (int)(25 * Scale);
            _hitBoxHeight = (int)(15 * Scale);
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

        public Rectangle Rectangle => new Rectangle((int)(Position.X - _hitBoxWidth / 2), (int)(Position.Y - _hitBoxHeight), _hitBoxWidth, _hitBoxHeight);

        public void Update(GameTime gameTime)
        {
            Vector2 previousPos = Position;

            Vector2 desiredPosition = Position;

            // move via strategy
            if (_movementStrategy != null)
            {
                Vector2 targetPos = _targetHero != null ? _targetHero.Position : Position;
                Position = _movementStrategy.Move(gameTime, Position, targetPos);
            }

            Rectangle futureRect = new Rectangle((int)(desiredPosition.X - _hitBoxWidth / 2), (int)(desiredPosition.Y - _hitBoxHeight),
                _hitBoxWidth, _hitBoxHeight);

            if (!_world.IsCollision(futureRect, this))
            {
                Position = desiredPosition;
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

        public void Interact(Hero hero)
        {
            // TODO: implement logic for taking damage from enemy
        }
    }
}
