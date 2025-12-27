using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoFactory.Entities.Interfaces;
using MonoFactory.Entities.Machines.States;
using MonoFactory.Items;
using System;

namespace MonoFactory.Entities.Machines
{
    public class Machine : IGameObject, IInteractable
    {
        public Vector2 Position { get; private set; }
        public Texture2D Texture { get; private set; }

        // state pattern
        public IMachineState CurrentState { get; private set; }

        public string InputItemName { get; } = "stone";
        public IItem OutputItem { get; } = new ResourceItem("Iron Bar");
        public float ProcessTime { get; } = 3.0f;

        private int _width = 64;
        private int _height = 64;
        public Rectangle Rectangle => new Rectangle((int)Position.X, (int)Position.Y, _width, _height);

        public Machine(Texture2D texture, Vector2 position)
        {
            Texture = texture;
            Position = position;

            SetState(new EmptyState());
        }

        public void SetState(IMachineState newState)
        {
            CurrentState = newState;
            CurrentState.Enter(this);
        }

        public void Update(GameTime gameTime)
        {
            CurrentState.Update(gameTime, this);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            CurrentState.Draw(spriteBatch, this);
        }

        public void Interact(Hero hero)
        {
            CurrentState.Interact(hero, this);
        }
    }
}
