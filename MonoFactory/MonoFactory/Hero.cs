using MonoFactory.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace MonoFactory
{
    public class Hero : IGameObject
    {
        private Texture2D texture;

        // Animation System
        private Dictionary<string, Animation> animations;
        private Animation currentAnimation;

        private IInputReader inputReader;
        private PhysicsComponent physics;
        private float scale;

        // Visual Offsets (For the 2.5D look)
        private Vector2 drawOffset;
        private SpriteEffects flipEffect = SpriteEffects.None;

        // Public Properties
        public InventoryComponent Inventory { get; private set; }
        public Vector2 Position => physics.Position; // Expose position for Camera

        private int _hitBoxWidth;
        private int _hitBoxHeight;

        public Rectangle Rectangle => new Rectangle((int)(Position.X - _hitBoxWidth / 2), (int)(Position.Y - _hitBoxHeight), _hitBoxWidth, _hitBoxHeight);

        public Hero(Texture2D texture, IInputReader inputReader, Vector2 startPos, float scale = 5f)
        {
            this.texture = texture;
            this.inputReader = inputReader;
            this.scale = scale;

            // --- 1. SETUP ANIMATIONS ---
            animations = new Dictionary<string, Animation>();

            // Idle (Row 1)
            var idleAnim = new Animation();
            idleAnim.AddFrame(new AnimationFrame(new Rectangle(0, 0, 64, 64)));
            idleAnim.AddFrame(new AnimationFrame(new Rectangle(64, 0, 64, 64)));
            idleAnim.AddFrame(new AnimationFrame(new Rectangle(128, 0, 64, 64)));
            idleAnim.AddFrame(new AnimationFrame(new Rectangle(192, 0, 64, 64)));
            animations.Add("Idle", idleAnim);

            // Walk (Row 2)
            var walkAnim = new Animation();
            walkAnim.AddFrame(new AnimationFrame(new Rectangle(0, 64, 64, 64)));
            walkAnim.AddFrame(new AnimationFrame(new Rectangle(64, 64, 64, 64)));
            walkAnim.AddFrame(new AnimationFrame(new Rectangle(128, 64, 64, 64)));
            walkAnim.AddFrame(new AnimationFrame(new Rectangle(192, 64, 64, 64)));
            walkAnim.AddFrame(new AnimationFrame(new Rectangle(256, 64, 64, 64)));
            walkAnim.AddFrame(new AnimationFrame(new Rectangle(320, 64, 64, 64)));
            animations.Add("Walk", walkAnim);

            // Jump (Rows 10 & 11 - Optional if you add jumping back later)
            var jumpAnim = new Animation();
            jumpAnim.IsLooping = false;
            jumpAnim.AddFrame(new AnimationFrame(new Rectangle(0, 576, 64, 64)));
            jumpAnim.AddFrame(new AnimationFrame(new Rectangle(64, 576, 64, 64)));
            jumpAnim.AddFrame(new AnimationFrame(new Rectangle(0, 640, 64, 64)));
            jumpAnim.AddFrame(new AnimationFrame(new Rectangle(64, 640, 64, 64)));
            animations.Add("Jump", jumpAnim);

            // Set Default
            currentAnimation = animations["Idle"];

            // Define a smaller physics box (so he fits on tiles)
            float hitBoxWidth = 30 * scale;
            float hitBoxHeight = 50 * scale;

            // Get the source rectangle size
            var src = currentAnimation.CurrentFrame.SourceRectangle;

            // Calculate the offset to align his feet to the physics position
            drawOffset = new Vector2(
                (src.Width * scale - hitBoxWidth) / 2f,  // Center X
                (src.Height * scale) - hitBoxHeight      // Align Bottom Y (Feet)
            );

            // initial components
            physics = new PhysicsComponent(startPos);
            Inventory = new InventoryComponent(20);
        }

        public void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 input = inputReader.ReadInput();

            if (inputReader.ReadJump())
            {
                physics.Jump();
            }
            // Update Physics
            physics.ApplyMovement(input, delta);
            physics.Update(delta);

            // Update Animation State
            UpdateAnimationState(input);
            currentAnimation.Update(gameTime);
        }

        private void UpdateAnimationState(Vector2 input)
        {
            // Simple State Logic
            if (input != Vector2.Zero)
            {
                currentAnimation = animations["Walk"];
            }
            else
            {
                currentAnimation = animations["Idle"];
            }

            // Flip Logic
            if (input.X > 0) flipEffect = SpriteEffects.None;
            else if (input.X < 0) flipEffect = SpriteEffects.FlipHorizontally;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Apply Offset
            Vector2 drawPosition = new Vector2(physics.Position.X, physics.Position.Y - physics.Height) - drawOffset;

            // make world unsquish
            Vector2 drawScale = new Vector2(scale, scale / 0.6f);

            spriteBatch.Draw(
                texture,
                drawPosition,
                currentAnimation.CurrentFrame.SourceRectangle,
                Color.White,
                0f,
                Vector2.Zero,
                drawScale, // Use the corrected scale
                flipEffect,
                0f
            );
        }
    }
}