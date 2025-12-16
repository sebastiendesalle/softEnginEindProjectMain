using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoFactory.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoFactory.Entities
{
    public class Tile // flyweight pattern, makes 1 square on the map
    {
        public bool IsSolid { get; set; }

        public Texture2D Texture { get; set; }

        public Tile(Texture2D texture, bool isSolid)
        {
            Texture = texture;
            IsSolid = isSolid;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            // set image at 64x64 pixels
            int size = GridHelper.TileSize;

            Rectangle destRect = new Rectangle(
                (int)position.X,
                (int)position.Y,
                size,
                size
            );

            Rectangle sourceRect = new Rectangle(0, 0, size, size);

            spriteBatch.Draw(Texture, destRect,sourceRect, Color.White);
        }
    }
}
