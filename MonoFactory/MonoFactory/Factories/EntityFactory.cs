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
        // store textures / flyweight pattern
        private Dictionary<string, Texture2D> _textureLibrary;
        private Dictionary<string, Func<Vector2, Texture2D, IGameObject>> _creators;
        //private WorldManager _world;

        public EntityFactory()
        {
            //_world = world;
            _textureLibrary = new Dictionary<string, Texture2D>();
            _creators = new Dictionary<string, Func<Vector2, Texture2D, IGameObject>>();

            //InitializeDefaultCreators();
        }

        public void RegisterCreator(string id, Func<Vector2, Texture2D, IGameObject> creator)
        {
            if (!_creators.ContainsKey(id))
            {
                _creators.Add(id, creator);
            }
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

            if (!_creators.ContainsKey(type))
            {
                throw new Exception($"Creator for {type} not found.");
            }
            return _creators[type](position, _textureLibrary[type]);
        }
    }
}
