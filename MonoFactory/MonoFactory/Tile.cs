using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoFactory
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
            spriteBatch.Draw(Texture, position, Color.White);
        }
    }
}
