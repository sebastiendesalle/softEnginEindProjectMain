using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

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
                case "Goblin":
                    return new Enemy(texture, position);
                case "Chest":
                    return new Chest(texture, position);
                // TODO: add machine
                default:
                    throw new Exception($"Entity type {type} is not in factory switch");
            }
        }
    }
}
