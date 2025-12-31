using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoFactory.Entities.Interfaces;
using MonoFactory.Items;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoFactory.Entities
{
    public class DroppedItem: IGameObject, IInteractable
    {
        public Vector2 Position { get; private set; }
        public IItem Item { get; private set; }
        private Texture2D _texture;
        private Rectangle _sourceRect;

        private float _bobTimer;
        private float _baseY;

        public Rectangle Rectangle => new Rectangle((int)Position.X, (int)Position.Y, _sourceRect.Width, _sourceRect.Height);

        public DroppedItem(IItem item, Vector2 position, Texture2D texture, Rectangle sourceRect)
        {
            Item = item;
            Position = position;
            _baseY = position.Y;
            _texture = texture;
            _sourceRect = sourceRect;
        }

        public void Update(GameTime gameTime)
        {
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _bobTimer += delta * 5f;

            float offset = (float)System.Math.Sin(_bobTimer) * 3f;
            Position = new Vector2(Position.X, _baseY + offset);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Position, _sourceRect, Color.White);
        }

        public void Interact(Hero hero)
        {
            if (hero.Inventory.AddItem(Item, 1))
            {
                Debug.WriteLine($"Picked up: {Item.Name}");


                // TODO: implement new way to remove items
                Position = new Vector2(-9000, -9000);
            }
            else
            {
                Debug.WriteLine("Inventory full");
            }
        }

    }
}
