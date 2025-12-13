using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using System.Collections.Generic;

namespace MonoFactory
{
    public class Chest: IGameObject, IInteractable
    {
        public Vector2 Position { get; private set; }
        private Texture2D _texture;

        private Animation _openAnimation;
        private Animation _currentAnimation;
        private bool _isOpen = false;

        public Rectangle _closedFrame = new Rectangle(0,0, 16, 16);


        public Chest(Texture2D texture, Vector2 position)
        {
            _texture = texture;
            Position = position;

            //open animation
            _openAnimation = new Animation();
            _openAnimation.IsLooping = false;

            _openAnimation.AddFrame(new AnimationFrame(new Rectangle(0, 0, 16, 48)));
            _openAnimation.AddFrame(new AnimationFrame(new Rectangle(0, 48, 16, 16)));
            _openAnimation.AddFrame(new AnimationFrame(new Rectangle(0, 96, 16, 48)));
            _openAnimation.AddFrame(new AnimationFrame(new Rectangle(0, 144, 16, 48)));
            _openAnimation.AddFrame(new AnimationFrame(new Rectangle(0, 192, 16, 48)));
        }

        public Rectangle Rectangle => new Rectangle((int)Position.X, (int)Position.Y, 32, 32);
        public void Interact(Hero hero)
        {
            _isOpen = !_isOpen;
            _currentAnimation = _openAnimation;
        }

        public void Update(GameTime gameTime) 
        {
            if (_isOpen && _currentAnimation != null)
            {
                _currentAnimation.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle drawRect;
            if (_isOpen && _currentAnimation != null)
            {
                drawRect = _currentAnimation.CurrentFrame.SourceRectangle;
            }
            else
            {
                drawRect = _closedFrame;
            }
                Color c = _isOpen ? Color.Gray : Color.White;

            Rectangle sourceRect = new Rectangle(0, 0, 64, 64);

            spriteBatch.Draw(_texture, Position, drawRect, Color.White, 0f, Vector2.Zero, 2.0f, SpriteEffects.None, 0f);
        }
    }
}
