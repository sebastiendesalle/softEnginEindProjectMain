using MonoFactory.Inputs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using MonoFactory.Managers;
using MonoFactory.Entities.Interfaces;
using MonoFactory.Components.Animation;
using MonoFactory.Components;
using MonoFactory.Items;

namespace MonoFactory.Entities
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

        private WorldManager _world;

        private SpriteEffects flipEffect = SpriteEffects.None;

        // Public Properties
        public InventoryComponent Inventory { get; private set; }
        public Vector2 Position => physics.Position; // Expose position for Camera

        private int _hitBoxWidth;
        private int _hitBoxHeight;

        public Rectangle Rectangle => new Rectangle((int)(Position.X - _hitBoxWidth / 2), (int)(Position.Y - _hitBoxHeight), _hitBoxWidth, _hitBoxHeight);

        public Hero(Texture2D texture, IInputReader inputReader, Vector2 startPos, WorldManager world, float scale = 5f)
        {
            this.texture = texture;
            this.inputReader = inputReader;
            this.scale = scale;
            _world = world;

            // SETUP ANIMATIONS 
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
            _hitBoxWidth = (int)(30 * scale);
            _hitBoxHeight = (int)(50 * scale);

            // Get the source rectangle size
            var src = currentAnimation.CurrentFrame.SourceRectangle;

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

            Vector2 velocityStep = physics.Velocity * delta;

            Rectangle futureRectX = new Rectangle((int)(physics.Position.X + velocityStep.X - _hitBoxWidth / 2),
                (int)(physics.Position.Y - _hitBoxHeight), _hitBoxWidth, _hitBoxHeight);

            if (_world.IsCollision(futureRectX, this))
            {
                physics.Velocity.X = 0;
            }

            Rectangle futureRectY = new Rectangle((int)(physics.Position.X - _hitBoxWidth / 2),
                (int)(physics.Position.Y + velocityStep.Y - _hitBoxHeight),
                _hitBoxWidth, _hitBoxHeight);

            if (_world.IsCollision(futureRectY, this))
            {
                physics.Velocity.Y = 0;
            }
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
            Vector2 drawPosition = new Vector2(physics.Position.X, physics.Position.Y - physics.Height);

            Rectangle src = currentAnimation.CurrentFrame.SourceRectangle;

            Vector2 origin = new Vector2(src.Width / 2f, src.Height);

            // make world unsquish
            Vector2 drawScale = new Vector2(scale, scale / 0.6f);

            spriteBatch.Draw(
                texture,
                drawPosition,
                src,
                Color.White,
                0f,
                origin,
                drawScale, // Use the corrected scale
                flipEffect,
                0f
            );
        }
    }
}