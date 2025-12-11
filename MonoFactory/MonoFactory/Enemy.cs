using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoFactory.EnemyStrategies;

namespace MonoFactory
{
    public class Enemy: IGameObject, IInteractable
    {
        private Texture2D _texture;
        public Vector2 Position { get; private set; }

        private IMovementStrategy _movementStrategy;
        private Hero _targetHero;
        private Rectangle _sourceRect = new Rectangle(0, 0, 64, 64);

        public Enemy(Texture2D texture, Vector2 startPosition, IMovementStrategy strategy)
        {
            _texture = texture;
            Position = startPosition;
            _movementStrategy = strategy;
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
