using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoFactory.Entities.Interfaces;
using System;

namespace MonoFactory.Entities
{
    public class HeartPowerup: IGameObject
    {
        public Vector2 Position { get; private set; }
        private Texture2D _texture;
        private Hero _targetHero;
        private bool _isCollected = false;

        private float _scale = 0.22f;
        private Vector2 _origin;

        private float _bobTimer;
        private float _baseY;

        public Rectangle Rectangle
        {
            get
            {
                if (_isCollected)
                {
                    return Rectangle.Empty;
                }
                int size = (int)(_texture.Width * _scale);
                return new Rectangle((int)Position.X - size / 2, (int)Position.Y - size / 2, size, size);
            }
        }

        public HeartPowerup(Texture2D texture, Vector2 position)
        {
            _texture = texture;
            Position = position;
            _baseY = position.Y;
            _origin = new Vector2(_texture.Width / 2f, _texture.Height / 2f);
        }

        public void SetHero(Hero hero)
        {
            _targetHero = hero;
        }

        public void Update(GameTime gameTime)
        {
            if (_isCollected)
            {
                return;
            }

            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _bobTimer += delta * 3f;
            Position = new Vector2(Position.X, _baseY + (float)Math.Sin(_bobTimer) * 5f);

            if (_targetHero != null && Rectangle.Intersects(_targetHero.Rectangle))
            {
                _targetHero.Heal(5);
                Collect();
            }
        }

        private void Collect()
        {
            _isCollected = true;
            Position = new Vector2(-9000, -9000);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_isCollected)
            {
                return;
            }

            spriteBatch.Draw(
                _texture,
                Position,
                null,
                Color.White,
                0f,
                _origin,
                _scale,
                SpriteEffects.None,
                0f
                );
        }
    }
}
