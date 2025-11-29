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
        private Texture2D heroTexture;

        // to store multiple animations
        private Dictionary<string, Animation> animations;
        private Animation currentAnimation;

        private IInputReader inputReader;
        private PhysicsComponent physics;
        private float moveSpeed = 200f;

        private float scale;
        private Vector2 spriteSize;
        private Vector2 drawOffset;
        private SpriteEffects flipEffect = SpriteEffects.None; // to face left/right

        public Hero(Texture2D texture, IInputReader inputReader, List<Rectangle> obstacles, Rectangle bounds, Vector2 startPos, float scale = 5f)
        {
            this.heroTexture = texture;
            this.inputReader = inputReader;
            this.scale = scale;

            animations = new Dictionary<string, Animation>();

            // idle animation
            var idleAnim = new Animation();
            idleAnim.AddFrame(new AnimationFrame(new Rectangle(0, 0, 64, 64)));
            idleAnim.AddFrame(new AnimationFrame(new Rectangle(64, 0, 64, 64)));
            idleAnim.AddFrame(new AnimationFrame(new Rectangle(128, 0, 64, 64)));
            idleAnim.AddFrame(new AnimationFrame(new Rectangle(192, 0, 64, 64)));
            animations.Add("Idle", idleAnim);

            // walk animation
            var walkAnim = new Animation();
            walkAnim.AddFrame(new AnimationFrame(new Rectangle(0, 64, 64, 64)));
            walkAnim.AddFrame(new AnimationFrame(new Rectangle(64, 64, 64, 64)));
            walkAnim.AddFrame(new AnimationFrame(new Rectangle(128, 64, 64, 64)));
            walkAnim.AddFrame(new AnimationFrame(new Rectangle(192, 64, 64, 64)));
            walkAnim.AddFrame(new AnimationFrame(new Rectangle(256, 64, 64, 64)));
            walkAnim.AddFrame(new AnimationFrame(new Rectangle(320, 64, 64, 64)));
            animations.Add("Walk", walkAnim);

            // jump animation
            var jumpAnim = new Animation();
            jumpAnim.IsLooping = false; // implement loop prevention

            // frames from row 10
            jumpAnim.AddFrame(new AnimationFrame(new Rectangle(0, 576, 64, 64)));
            jumpAnim.AddFrame(new AnimationFrame(new Rectangle(64, 576, 64, 64)));

            // frames from row 11
            jumpAnim.AddFrame(new AnimationFrame(new Rectangle(64, 640, 64, 64)));
            animations.Add("Jump", jumpAnim);

            // set default state
            currentAnimation = animations["Idle"];

            // calc hitbox size
            float hitBoxWidth = 30 * scale;
            float hitBoxHeight = 50 * scale;
            spriteSize = new Vector2(hitBoxWidth, hitBoxHeight); // physics size

            // calc offset
            var src = currentAnimation.CurrentFrame.SourceRectangle;
            drawOffset = new Vector2(
                (src.Width * scale - hitBoxWidth) / 2f, // center X
                (src.Height * scale) - hitBoxHeight // center Y
            );

            physics = new PhysicsComponent(startPos, obstacles, bounds);
        }

        public void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // get input
            Vector2 input = inputReader.ReadInput();

            // send input to physics
            physics.ApplyMovement(input, moveSpeed, delta);

            // update physics
            physics.Update(delta, (int)spriteSize.X, (int)spriteSize.Y);

            // state machine
            UpdateAnimationState(input);

            // update animation framecounter
            currentAnimation.Update(gameTime);
        }

        private void UpdateAnimationState(Vector2 input)
        {
            if (!physics.IsGrounded)
            {
                currentAnimation = animations["Jump"]; // set jump animation
            }
            else if (input.X != 0)
            {
                currentAnimation = animations["Walk"]; // set walk animation
            }
            else
            {
                currentAnimation = animations["Idle"]; // set idle animation
            }

            if (input.X > 0)
            {
                flipEffect = SpriteEffects.None; // face right
            }
            else if (input.X < 0)
            {
                flipEffect = SpriteEffects.FlipHorizontally; // face left
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 drawPosition = physics.Position - drawOffset;

            spriteBatch.Draw(
                heroTexture,
                drawPosition,
                currentAnimation.CurrentFrame.SourceRectangle,
                Color.White,
                0f,
                Vector2.Zero,
                scale,
                flipEffect,
                0f
            );
        }
    }
}