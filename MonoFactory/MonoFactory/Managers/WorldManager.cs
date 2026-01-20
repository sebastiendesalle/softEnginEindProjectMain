using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoFactory.Core;
using MonoFactory.Entities;
using MonoFactory.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MonoFactory.Managers
{
    public class WorldManager
    {

        // store machines etc..
        private Dictionary<Point, Tile> grid = new Dictionary<Point, Tile>();

        // store grass texture
        private Texture2D grassTexture;

        private List<IGameObject> _entities = new List<IGameObject>();

        private Portal _activePortal;
        private Action _onAllEnemiesDefeated;

        private SoundManager _soundManager;

        private bool _wereEnemiesAlive = false;

        public WorldManager(Texture2D grassTexture, SoundManager soundManager)
        {
            this.grassTexture = grassTexture;
            _soundManager = soundManager;
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
                            x * GridHelper.TileSize,
                            y * GridHelper.TileSize,
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
                if (entity is DroppedItem)
                {
                    continue;
                }
                if (owner is Enemy && entity is Enemy)
                {
                    continue;
                }
                if (entity is HeartPowerup)
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

        public void DamageEntitiesInArea(Vector2 center, float radius, int damage, IGameObject attacker)
        {
            var targets = _entities.ToList();
            bool hitAnyone = false;

            foreach (var entity in targets)
            {
                if (entity == attacker)
                {
                    continue;
                }

                if (entity is IDamageable damageable)
                {
                    float dist = Vector2.Distance(center, ((IGameObject)entity).Rectangle.Center.ToVector2());

                    if (dist <= radius)
                    {
                        Debug.WriteLine($"hit for {damage} damage");
                        damageable.TakeDamage(damage);
                        hitAnyone = true;
                    }
                }
            }
            if (hitAnyone)
            {
                _soundManager.PlaySound("Hit");
            }
        }

        public void Update(GameTime gameTime)
        {

            int enemiesStartCount = GetEnemyCount();

            if (_wereEnemiesAlive && enemiesStartCount == 0)
            {
                Debug.WriteLine("All enemies defeated");
                _onAllEnemiesDefeated?.Invoke();
            }

            for (int i = _entities.Count - 1; i >= 0; i--)
            {
                _entities[i].Update(gameTime);

                if (_entities[i] is Projectile projectile && projectile.IsActive)
                {
                    foreach (var entity in _entities)
                    {
                        if (projectile.CheckHit(entity))
                        {
                            break;
                        }
                    }
                }
            }

            int enemiesEndCount = GetEnemyCount();
            if (enemiesStartCount > 0 && enemiesEndCount == 0)
            {
                _onAllEnemiesDefeated?.Invoke();
            }
            _wereEnemiesAlive = enemiesEndCount > 0;
        }

        public void SetEnemyDefeatedCallback(Action callback)
        {
            _onAllEnemiesDefeated = callback;
        }

        public void SpawnPortal(Vector2 position, Texture2D texture)
        {
            if (_activePortal != null)
            {
                _entities.Remove(_activePortal);
            }

            _activePortal = new Portal(position, texture);
            _entities.Add(_activePortal);
            Debug.WriteLine($"portal spawned at {position}");
        }

        public bool IsPlayerInPortal(Hero hero)
        {
            if (_activePortal != null)
            {
                return _activePortal.CheckPlayerCollision(hero);
            }
            return false;
        }

        public int GetEnemyCount()
        {
            int count = 0;
            foreach (var entity in _entities)
            {
                if (entity is Enemy enemy && !enemy.IsDead)
                {
                        count++;
                }
            }
            return count;
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera, GraphicsDevice graphics, Texture2D debugPixel = null)
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

                if (debugPixel != null)
                {
                    DrawBorder(spriteBatch, debugPixel, entity.Rectangle, Color.Red, 2);
                }
            }
        }
        private void DrawBorder(SpriteBatch sb, Texture2D pixel, Rectangle rect, Color color, int thickness)
        {
            sb.Draw(pixel, new Rectangle(rect.X, rect.Y, rect.Width, thickness), color);
            sb.Draw(pixel, new Rectangle(rect.X, rect.Bottom, rect.Width, thickness), color);
            sb.Draw(pixel, new Rectangle(rect.X, rect.Y, thickness, rect.Height), color);
            sb.Draw(pixel, new Rectangle(rect.Right, rect.Y, thickness, rect.Height), color);
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
