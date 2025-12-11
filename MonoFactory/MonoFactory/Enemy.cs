using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoFactory.EnemyStrategies;
using System.Collections.Generic;

namespace MonoFactory
{
    public class Enemy: IGameObject, IInteractable
    {
        private Texture2D _texture;

        private Dictionary<string, Animation> animations;
        private Animation currentAnimation;

        private Vector2 drawOffset;
        private SpriteEffects flipEffect = SpriteEffects.None;

        public Vector2 Position { get; private set; }

        private IMovementStrategy _movementStrategy;
        private Hero _targetHero;
        private Rectangle _sourceRect = new Rectangle(0, 0, 64, 64);

        public Enemy(Texture2D texture, Vector2 startPosition, IMovementStrategy strategy, float scale = 4f)
        {
            _texture = texture;
            Position = startPosition;
            _movementStrategy = strategy;

            animations = new Dictionary<string, Animation>();

            var walkAnim = new Animation();
            walkAnim.AddFrame(new AnimationFrame(new Rectangle(0, 192, 64, 64)));
            walkAnim.AddFrame(new AnimationFrame(new Rectangle(64, 192, 64, 64)));
            walkAnim.AddFrame(new AnimationFrame(new Rectangle(128, 192, 64, 64)));
            walkAnim.AddFrame(new AnimationFrame(new Rectangle(192, 192, 64, 64)));
            walkAnim.AddFrame(new AnimationFrame(new Rectangle(256, 192, 64, 64)));
            walkAnim.AddFrame(new AnimationFrame(new Rectangle(320, 192, 64, 64)));
            animations.Add("Walk", walkAnim);

            currentAnimation = animations["Idle"]; // TODO: implement idle

            float hitBoxWidth = 30 * scale;
            float hitBoxHeight = 50 * scale;

            var src = currentAnimation.CurrentFrame.SourceRectangle;

            drawOffset = new Vector2((src.Width * scale - hitBoxWidth) / 2f, (src.Height * scale) - hitBoxHeight);
        }

        public void SetTarget(Hero hero)
        {
            _targetHero = hero;
        }

        public Rectangle Rectangle => new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);

        public void Update(GameTime gameTime)
        {
            if (_movementStrategy != null)
            {
                Vector2 targetPos = _targetHero != null ? _targetHero.Position : Position;
                Position = _movementStrategy.Move(gameTime, Position, targetPos);
            }

            currentAnimation.Update(gameTime);
        }

        private void UpdateAnimatioState(Vector2 input)
        {
            if (input != Vector2.Zero)
            {
                currentAnimation = animations["Walk"];
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Position,_sourceRect, Color.White);
        }

        public void Interact(Hero hero)
        {
            // TODO: implement logic for taking damage from enemy
        }
    }
}
