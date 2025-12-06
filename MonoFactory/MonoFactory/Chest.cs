using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace MonoFactory
{
    public class Chest: IGameObject, IInteractable
    {
        public Vector2 Position { get; private set; }
        private Texture2D _texture;
        private bool _isOpen = false;

        public Chest(Texture2D texture, Vector2 position)
        {
            _texture = texture;
            Position = position;
        }

        public void Interact(Hero hero)
        {
            _isOpen = !_isOpen;
            Debug.WriteLine($"Chest is {(_isOpen ? "open" : "closed")}");
        }

        public void Update(GameTime gameTime) { }

        public void Draw(SpriteBatch spriteBatch)
        {
            Color c = _isOpen ? Color.Gray : Color.White;

            Rectangle sourceRect = new Rectangle(0, 0, 64, 64);

            spriteBatch.Draw(_texture, Position, sourceRect, c);
        }
    }
}
