using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoFactory
{
    public class WorldManager
    {
        private Tile[,] grid;
        private int width, height;

        public WorldManager(int width, int height)
        {
            this.width = width;
            this.height = height;
            grid = new Tile[width, height];
        }

        // generate map, factory pattern
        public void GenerateWorld(Texture2D grassTexture)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // Create a simple grass tile everywhere
                    grid[x, y] = new Tile(grassTexture, false);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {

            // loop to draw all tiles
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector2 worldPos = GridHelper.GridToWorld(x, y);
                    grid[x, y].Draw(spriteBatch, worldPos);
                }
            }
        }
    }
}
