using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoFactory.EnemyStrategies;
using System.Collections.Generic;

namespace MonoFactory
{
    public class Enemy: IGameObject, IInteractable
    {
        public Vector2 Position { get; private set; }
        private Texture2D _texture;



        private Dictionary<string, Animation> _animations;
        private Animation _currentAnimation;
        private Vector2 _previousPosition;
        //private Vector2 drawOffset; remove perchance
        private SpriteEffects _flipEffect = SpriteEffects.None;

        

        private IMovementStrategy _movementStrategy;
        private Hero _targetHero;
        //private Rectangle _sourceRect = new Rectangle(0, 0, 64, 64); remove perchance

        public Enemy(Texture2D texture, Vector2 startPosition, IMovementStrategy strategy)
        {
            _texture = texture;
            Position = startPosition;
            _previousPosition = startPosition;
            _movementStrategy = strategy;

            LoadAnimations();
        }

        public void LoadAnimations()
        {
            _animations = new Dictionary<string, Animation>();

            var idleAnim = new Animation();
            idleAnim.AddFrame(new AnimationFrame(new Rectangle(0, 256, 62, 62)));
            idleAnim.AddFrame(new AnimationFrame(new Rectangle(62, 256, 62, 62)));
            idleAnim.AddFrame(new AnimationFrame(new Rectangle(124, 256, 62, 62)));
            idleAnim.AddFrame(new AnimationFrame(new Rectangle(186, 256, 62, 62)));
            _animations.Add("Idle", idleAnim);

            var runAnim = new Animation();
            runAnim.AddFrame(new AnimationFrame(new Rectangle(0, 192, 63, 63)));
            runAnim.AddFrame(new AnimationFrame(new Rectangle(63, 192, 63, 63)));
            runAnim.AddFrame(new AnimationFrame(new Rectangle(126, 192, 63, 63)));
            runAnim.AddFrame(new AnimationFrame(new Rectangle(189, 192, 63, 63)));
            runAnim.AddFrame(new AnimationFrame(new Rectangle(252, 192, 63, 63)));
            runAnim.AddFrame(new AnimationFrame(new Rectangle(315, 192, 63, 63)));
            runAnim.AddFrame(new AnimationFrame(new Rectangle(378, 192, 63, 63)));
            runAnim.AddFrame(new AnimationFrame(new Rectangle(441, 192, 63, 63)));
            runAnim.AddFrame(new AnimationFrame(new Rectangle(504, 192, 63, 63)));
            runAnim.AddFrame(new AnimationFrame(new Rectangle(567, 192, 63, 63)));
            runAnim.AddFrame(new AnimationFrame(new Rectangle(630, 192, 63, 63)));
            runAnim.AddFrame(new AnimationFrame(new Rectangle(693, 192, 63, 63)));
            _animations.Add("Run", runAnim);

            _currentAnimation = _animations["Idle"];
        }

        public void SetTarget(Hero hero)
        {
            _targetHero = hero;
        }

        public Rectangle Rectangle => new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);

        public void Update(GameTime gameTime)
        {
            Vector2 newPos = Position;

            // move via strategy
            if (_movementStrategy != null)
            {
                Vector2 targetPos = _targetHero != null ? _targetHero.Position : Position;
                Position = _movementStrategy.Move(gameTime, Position, targetPos);
            }

            Vector2 movement = newPos - Position;
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
                else
                {
                    _currentAnimation = _animations["Idle"];
                }
            }

            // update pos & animation
            Position = newPos;
            _currentAnimation.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, 
                Position,
                _currentAnimation.CurrentFrame.SourceRectangle,
                Color.White,
                0f,
                Vector2.Zero,
                1.5f,
                _flipEffect,
                0f);
        }

        public void Interact(Hero hero)
        {
            // TODO: implement logic for taking damage from enemy
        }
    }
}
