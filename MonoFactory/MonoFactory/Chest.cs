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

        private const int FrameWidth = 54;
        private const int FrameHeight = 48;
        private const float Scale = 3.0f;

        public Rectangle _closedFrame;


        public Chest(Texture2D texture, Vector2 position)
        {
            _texture = texture;
            Position = position;

            //open animation
            _openAnimation = new Animation();
            _openAnimation.IsLooping = false;

            _openAnimation.AddFrame(new AnimationFrame(new Rectangle(0, 0, 54, 48)));
            _openAnimation.AddFrame(new AnimationFrame(new Rectangle(0, 48, 54, 16)));
            _openAnimation.AddFrame(new AnimationFrame(new Rectangle(0, 96, 54, 48)));
            _openAnimation.AddFrame(new AnimationFrame(new Rectangle(0, 144, 54, 48)));

            _closedFrame = new Rectangle(0, 0, FrameWidth, FrameHeight);
        }

        public Rectangle Rectangle => new Rectangle((int)Position.X, (int)Position.Y,(int)(FrameWidth * Scale), (int)(FrameHeight * Scale));

        public Rectangle InteractionRectangle
        {
            get
            {
                Rectangle rect = Rectangle;
                rect.Inflate(20, 20);
                return rect;
            }
        }
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

            spriteBatch.Draw(_texture,
                Position,
                drawRect,
                Color.White,
                0f,
                Vector2.Zero,
                Scale,
                SpriteEffects.None,
                0f);
        }
    }
}
