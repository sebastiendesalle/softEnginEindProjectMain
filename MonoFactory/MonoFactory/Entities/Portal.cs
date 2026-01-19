using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoFactory.Entities.Interfaces;
using System;
using System.Security.Cryptography.X509Certificates;

namespace MonoFactory.Entities
{
    public class Portal: IGameObject
    {
        public Vector2 Position { get; private set; }
        private Texture2D _texture;
        private float _rotationAngle;
        private float _pulseTimer;
        private float _scale;
        private const float BaseScale = 2.0f;
        private Color _color;

        private const int PortalSize = 64;

        public Rectangle Rectangle => new Rectangle(
            (int)(Position.X - (PortalSize * _scale) / 2),
            (int)(Position.Y - (PortalSize * _scale) / 2),
            (int)(PortalSize * _scale),
            (int)(PortalSize * _scale)
        );

        
        public Portal(Vector2 position, Texture2D texture)
        {
            Position = position;
            _texture = texture;
            _scale = BaseScale;
            _color = new Color(100, 200, 255);
        }

        public void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _rotationAngle += delta * 2f;

            _pulseTimer += delta * 3f;
            _scale = BaseScale + (float)Math.Sin(_pulseTimer) * 0.2f;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 origin = new Vector2(_texture.Width / 2f, _texture.Height / 2f);

            // outer glow
            spriteBatch.Draw(
                _texture,
                Position,
                null,
                _color * 0.3f,
                _rotationAngle,
                origin,
                _scale * 1.5f,
                SpriteEffects.None,
                0f
                );

            //middle layer
            spriteBatch.Draw(
                _texture,
                Position,
                null,
                _color * 0.6f,
                -_rotationAngle * 0.7f,
                origin,
                _scale * 1.2f,
                SpriteEffects.None,
                0f
                );

            spriteBatch.Draw(
                _texture,
                Position,
                null,
                Color.White * 0.8f,
                _rotationAngle * 0.5f,
                origin,
                _scale,
                SpriteEffects.None,
                0f
                );
        }
        public bool CheckPlayerCollision(Hero hero)
        {
            return Rectangle.Intersects(hero.Rectangle);
        }
    }
}
