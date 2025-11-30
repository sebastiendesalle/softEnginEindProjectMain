using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MonoFactory
{
    public static class GridHelper
    {
        // set tile size, 1 square = 64 px
        public const int TileSize = 64;


        // convert grid coords to screen pixels
        public static Vector2 GridToWorld(int x, int y)
        {
            return new Vector2(x * TileSize, y * TileSize);
        }

        public static Point WorldToGrid(Vector2 position)
        {
            return new Point((int)(position.X / TileSize),
                (int)(position.Y / TileSize));
        }
    }
}
