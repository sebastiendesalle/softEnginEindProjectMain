using MonoFactory.Inputs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using MonoFactory.Managers;
using MonoFactory.Entities.Interfaces;
using MonoFactory.Components.Animation;
using MonoFactory.Components;
using MonoFactory.Items;
using System.Diagnostics;

namespace MonoFactory.Entities
{
    public class Hero : IGameObject, IDamageable
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

        private Color _tintColor = Color.White;
        private float _damageFlashTimer = 0f;

        // Public Properties
        public InventoryComponent Inventory { get; private set; }
        public Vector2 Position => physics.Position;

        public int Health { get; private set; } = 10;
        public bool IsDead => Health <= 0;

        private int _hitBoxWidth;
        private int _hitBoxHeight;

        private bool _isAttacking = false;

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
            for (int i = 0; i < 4; i++)
            {
                idleAnim.AddFrame(new AnimationFrame(new Rectangle(i * 64, 0, 64, 64)));
            }
            animations.Add("Idle", idleAnim);

            // Walk (Row 2)
            var walkAnim = new Animation();
            for (int i = 0; i < 6; i++)
            {
                walkAnim.AddFrame(new AnimationFrame(new Rectangle(i * 64, 64, 64, 64)));
            }
            animations.Add("Walk", walkAnim);

            // Jump (Rows 10 & 11)
            var jumpAnim = new Animation();
            jumpAnim.IsLooping = false;
            jumpAnim.AddFrame(new AnimationFrame(new Rectangle(0, 576, 64, 64)));
            jumpAnim.AddFrame(new AnimationFrame(new Rectangle(64, 576, 64, 64)));
            jumpAnim.AddFrame(new AnimationFrame(new Rectangle(0, 640, 64, 64)));
            jumpAnim.AddFrame(new AnimationFrame(new Rectangle(64, 640, 64, 64)));
            animations.Add("Jump", jumpAnim);

            var attackAnim = new Animation();
            attackAnim.IsLooping = false;
            attackAnim.FPS = 15;
            for (int i = 0; i < 6; i++)
            {
                attackAnim.AddFrame(new AnimationFrame(new Rectangle(i * 64, 256, 64, 64)));
            }
            animations.Add("Attack", attackAnim);

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

            if (_damageFlashTimer > 0)
            {
                _damageFlashTimer -= delta;
                _tintColor = Color.Red;
            }
            else
            {
                _tintColor = Color.White;
            }
        }

        private void UpdateAnimationState(Vector2 input)
        {

            if (_isAttacking)
            {
                if (currentAnimation.IsFinished)
                {
                    _isAttacking = false;
                }
                else
                {
                    return;
                }
            }
            // Simple State Logic
            if (physics.Height > 0)
            {
                if (currentAnimation != animations["Jump"])
                {
                    currentAnimation = animations["Jump"];
                    currentAnimation.Reset();
                }
            }
            else if (input != Vector2.Zero)
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

        public void TriggerAttack()
        {
            if (!_isAttacking)
            {
                _isAttacking = true;
                currentAnimation = animations["Attack"];
                currentAnimation.Reset();
            }
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
                _tintColor,
                0f,
                origin,
                drawScale, // Use the corrected scale
                flipEffect,
                0f
            );
        }

        public void TakeDamage(int amount)
        {
            Health -= amount;
            _damageFlashTimer = 0.2f;

            Debug.WriteLine($"Took damage: {Health}");

        }
    }
}