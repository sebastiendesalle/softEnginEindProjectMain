using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using MonoFactory.Strategies;
using MonoFactory.Managers;
using MonoFactory.Entities.Interfaces;
using MonoFactory.Entities;

namespace MonoFactory.Factories
{
    public class EntityFactory // factory pattern
    {
        // store textures
        private Dictionary<string, Texture2D> _textureLibrary;
        private Dictionary<string, Func<Vector2, Texture2D, IGameObject>> _creators;
        private WorldManager _world;

        public EntityFactory(WorldManager world)
        {
            _world = world;
            _textureLibrary = new Dictionary<string, Texture2D>();
            _creators = new Dictionary<string, Func<Vector2, Texture2D, IGameObject>>();

            InitializeDefaultCreators();
        }

        public void RegisterTexture(string key, Texture2D texture)
        {
            if (!_textureLibrary.ContainsKey(key))
            {
                _textureLibrary.Add(key, texture);
            }
        }

        private void InitializeDefaultCreators() // OCP
        {
            _creators["Chest"] = (pos, tex) => new Chest(tex, pos);

            _creators["Goblin_Chaser"] = (pos, tex) => new Enemy(tex, pos, new ChaseStrategy(), _world);

            _creators["Goblin_Patrol"] = (pos, tex) =>
            {
                Vector2 endPos = pos + new Vector2(200, 0);
                return new Enemy(tex, pos, new PatrolStrategy(pos, endPos), _world);
            };

            _creators["Goblin_Turret"] = (pos, tex) => new Enemy(tex, pos, new StationaryStrategy(), _world);
        }

        public IGameObject CreateEntity(string type, Vector2 position)
        {
            if (!_textureLibrary.ContainsKey(type))
            {
                throw new Exception($"Texture for {type} not found.");
            }

            if (!_creators.ContainsKey(type))
            {
                return new Enemy(_textureLibrary[type], position, new StationaryStrategy(), _world);
            }
            Texture2D texture = _textureLibrary[type];
            return _creators[type](position, texture);
        }
    }
}
