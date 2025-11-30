using MonoFactory;
using MonoFactory.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace MonoFactory
{
    internal class Hero : IGameObject
    {
        private Texture2D texture;
        private Animation animation;
        private IInputReader inputReader;
        private PhysicsComponent physics;
        private float scale;

        public Hero(Texture2D texture, IInputReader inputReader, Vector2 startPos, float scale = 5f)
        {
            this.texture = texture;
            this.inputReader = inputReader;
            this.scale = scale;

            // Setup Animation (Using Walk Row for now)
            animation = new Animation();
            animation.AddFrame(new AnimationFrame(new Rectangle(0, 64, 64, 64)));
            animation.AddFrame(new AnimationFrame(new Rectangle(64, 64, 64, 64)));
            animation.AddFrame(new AnimationFrame(new Rectangle(128, 64, 64, 64)));
            animation.AddFrame(new AnimationFrame(new Rectangle(192, 64, 64, 64)));
            animation.AddFrame(new AnimationFrame(new Rectangle(256, 64, 64, 64)));

            physics = new PhysicsComponent(startPos);
        }

        public void Update(GameTime gameTime)
        {
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var input = this.inputReader.ReadInput();

            physics.ApplyMovement(input, delta);
            physics.Update(delta);

            if (input != Vector2.Zero)
            {
                animation.Update(gameTime);
            }
            else
            {
                
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // We draw at the physics position
            spriteBatch.Draw(
                texture,
                physics.Position,
                animation.CurrentFrame.SourceRectangle,
                Color.White,
                0f,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                0f
            );
        }
    }
}