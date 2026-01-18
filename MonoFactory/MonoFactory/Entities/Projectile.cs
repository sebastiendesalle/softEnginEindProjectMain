using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoFactory.Entities.Interfaces;
using System;

namespace MonoFactory.Entities
{
    public class Projectile : IGameObject
    {
        public Vector2 Position { get; private set; }
        public Vector2 _velocity;
        private Texture2D _texture;
        private int _damage;
        private IGameObject _owner;
        private float _lifetime;
        private const float MaxLifetime = 3f;
        private bool _isActive = true;

        private const int ProjectileSize = 8;
        private Color _color;

        public Rectangle Rectangle => _isActive ? new Rectangle((int)Position.X - ProjectileSize / 2, (int)Position.Y - ProjectileSize / 2, ProjectileSize, ProjectileSize) : Rectangle.Empty;
        
        public Projectile(Vector2 startPosition, Vector2 targetPosition, int damage, IGameObject owner, Texture2D pixelTexture, Color color)
        {
            Position = startPosition;
            _damage = damage;
            _owner = owner;
            _texture = pixelTexture;
            _color = color;

            Vector2 direction = targetPosition - startPosition;
            if (direction != Vector2.Zero)
            {
                direction.Normalize();
            }

            float speed = 300f;
            _velocity = direction * speed;
        }

        public void Update(GameTime gameTime)
        {
            if (!_isActive)
            {
                return;
            }

            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _lifetime += delta;

            Position += _velocity * delta;

            if (_lifetime > MaxLifetime)
            {
                _isActive = false;
                Position = new Vector2(-9000, -9000);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!_isActive)
            {
                return;
            }
            spriteBatch.Draw(_texture, Rectangle, _color);
        }

        public bool CheckHit(IGameObject target)
        {
            if (!_isActive)
            {
                return false;
            }
            if (target == _owner)
            {
                return false;
            }

            if (Rectangle.Intersects(target.Rectangle))
            {
                if (target is IDamageable damageable)
                {
                    damageable.TakeDamage(_damage);
                    _isActive = false;
                    Position = new Vector2(-9000, -9000);
                    return true;
                }
            }
            return false;
        }
        public bool IsActive => _isActive;
    }
}
