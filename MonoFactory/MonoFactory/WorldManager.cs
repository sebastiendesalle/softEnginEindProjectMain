using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace MonoFactory
{
    public class WorldManager
    {

        // store machines etc..
        private Dictionary<Point, Tile> grid = new Dictionary<Point, Tile>();


        // store grass texture
        private Texture2D grassTexture;

        private List<IInteractable> _machines = new List<IInteractable>();
        
        public WorldManager(Texture2D grassTexture)
        {
            this.grassTexture = grassTexture;
        }

        public void AddBuilding(Point coordinate, Tile tile)
        {
            if (!grid.ContainsKey(coordinate))
            {
                grid.Add(coordinate, tile);
            }
        }

        public void AddMachine(IInteractable machine)
        {
            _machines.Add(machine);
        }

        public IInteractable GetNearestMachine(Vector2 targetPos, float maxDistance)
        {
            foreach (var machine in _machines)
            {
                if (Vector2.Distance(targetPos, machine.Position) < maxDistance )
                {
                    return machine;
                }
            }
            return null;
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera, GraphicsDevice graphics)
        {
            // calc visible world area
            Matrix inverseView = Matrix.Invert(camera.Transform);

            Vector2 topLeft = Vector2.Transform(Vector2.Zero, inverseView);
            Vector2 bottomRight = Vector2.Transform(new Vector2(graphics.Viewport.Width, graphics.Viewport.Height), inverseView);

            // convert screen pixels to grid coords

            // todo: fix convertion error
            Point minGrid = GridHelper.WorldToGrid(topLeft);
            Point maxGrid = GridHelper.WorldToGrid(bottomRight);

            // tile buffer (fix flickering)
            minGrid.X -= 1; minGrid.Y -= 15;
            maxGrid.X += 1; maxGrid.Y += 15;

            for (int x = minGrid.X; x <= maxGrid.X; x++)
            {
                for (int y = minGrid.Y; y <= maxGrid.Y; y++)
                {
                    Vector2 position = GridHelper.GridToWorld(x, y);
                    Point coordinate = new Point(x, y);

                    DrawGrass(spriteBatch, position);

                    // check for machine, draw ontop of grid
                    if (grid.ContainsKey(coordinate))
                    {
                        grid[coordinate].Draw(spriteBatch, position);
                    }
                }
            }

            foreach (var item in _machines)
            {
                if (item is IGameObject drawable)
                {
                    drawable.Draw(spriteBatch);
                }
            }
        }

        private void DrawGrass(SpriteBatch spriteBatch, Vector2 position)
        {
            int size = GridHelper.TileSize;

            // destination
            Rectangle destRect = new Rectangle((int)position.X, (int)position.Y, size, size);

            // source
            Rectangle sourceRect = new Rectangle(0, 0, size, size);

            spriteBatch.Draw(grassTexture, destRect, sourceRect, Color.White);
        }
    }
}
