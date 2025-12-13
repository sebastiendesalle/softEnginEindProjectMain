using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonoFactory
{
    public class WorldManager // mediator pattern
    {

        // store machines etc..
        private Dictionary<Point, Tile> grid = new Dictionary<Point, Tile>();

        // store grass texture
        private Texture2D grassTexture;

        private List<IGameObject> _entities = new List<IGameObject>();
        
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

        public void AddEntity(IGameObject entity)
        {
            _entities.Add(entity);
        }

        public bool IsCollision(Rectangle targetRect, IGameObject owner)
        {
            Point minGrid = GridHelper.WorldToGrid(new Vector2(targetRect.Left, targetRect.Top));
            Point maxGrid = GridHelper.WorldToGrid(new Vector2(targetRect.Right, targetRect.Bottom));

            for (int x = minGrid.X - 1; x <= maxGrid.X + 1; x++)
            {
                for (int y = minGrid.Y - 1; y <= maxGrid.Y + 1; y++)
                {
                    Point pt = new Point(x, y);
                    if (grid.ContainsKey(pt))
                    {
                        Tile tile = grid[pt];

                        Rectangle tileRect = new Rectangle(
                            (int)(x * GridHelper.TileSize),
                            (int)(y * GridHelper.TileSize),
                            GridHelper.TileSize,
                            GridHelper.TileSize);

                        if (tile.IsSolid && tileRect.Intersects(targetRect))
                        {
                            return true;
                        }
                    }
                }
            }
            foreach (var entity in _entities)
            {
                if (entity == owner)
                {
                    continue;
                }
                if (entity.Rectangle.Intersects(targetRect))
                {
                    return true;
                }
            }
            return false;
        }


        // help find Interactables from list
        public IInteractable GetNearestInteractable(Vector2 targetPos, float maxDistance)
        {
            IInteractable nearest = null;
            float minDistance = maxDistance;

            foreach (var entity in _entities)
            {
                if (entity is IInteractable interactable)
                {
                    float distance = Vector2.Distance(targetPos, interactable.Position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearest = interactable;
                    }
                }
            }
            return nearest;
        }

        public void Update(GameTime gameTime)
        {
            for (int i = _entities.Count - 1; i >= 0; i--)
            {
                _entities[i].Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera, GraphicsDevice graphics)
        {
            // calc visible world area
            Matrix inverseView = Matrix.Invert(camera.Transform);

            Vector2 topLeft = Vector2.Transform(Vector2.Zero, inverseView);
            Vector2 bottomRight = Vector2.Transform(new Vector2(graphics.Viewport.Width, graphics.Viewport.Height), inverseView);

            // convert screen pixels to grid coords

            Point minGrid = GridHelper.WorldToGrid(topLeft);
            Point maxGrid = GridHelper.WorldToGrid(bottomRight);

            // tile buffer (fix flickering)
            minGrid.X -= 1; minGrid.Y -= 20;
            maxGrid.X += 1; maxGrid.Y += 20;

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

            foreach (var entity in _entities.OrderBy(e => e.Rectangle.Bottom))
            {
                entity.Draw(spriteBatch);
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
