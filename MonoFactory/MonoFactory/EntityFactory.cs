using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using MonoFactory.EnemyStrategies;

namespace MonoFactory
{
    public class EntityFactory // factory pattern
    {
        // store textures
        private Dictionary<string, Texture2D> _textureLibrary;

        public EntityFactory()
        {
            _textureLibrary = new Dictionary<string, Texture2D>();
        }

        public void RegisterTexture(string key, Texture2D texture)
        {
            if (!_textureLibrary.ContainsKey(key))
            {
                _textureLibrary.Add(key, texture);
            }
        }

        public IGameObject CreateEntity(string type, Vector2 position)
        {
            if (!_textureLibrary.ContainsKey(type))
            {
                throw new Exception($"Texture for {type} not found.");
            }

            Texture2D texture = _textureLibrary[type];

            switch (type)
            {
                case "Chest":
                    return new Chest(texture, position);
                case "Goblin_Chaser":
                    return new Enemy(texture, position, new ChaseStrategy());
                case "Goblin_Patrol":
                    Vector2 endPos = position + new Vector2(200, 0);
                    return new Enemy(texture, position, new PatrolStrategy(position, endPos));
                case "Goblin_Turret":
                    return new Enemy(texture, position, new StationaryStrategy());
                default:
                    return new Enemy(texture, position, new StationaryStrategy());
            }
        }
    }
}
