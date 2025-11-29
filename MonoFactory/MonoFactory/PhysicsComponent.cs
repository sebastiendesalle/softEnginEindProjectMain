using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MonoFactory
{
    class PhysicsComponent // handles movement and gravity and collisions
    {
        public Vector2 Position;
        public Vector2 Velocity;

        private Rectangle _bounds; //screenbounds
        private List<Rectangle> _obstacles;

        private const float Gravity = 1000f;
        private const float JumpStrength = 600f;

        public bool IsGrounded { get; private set; }

        public PhysicsComponent(Vector2 startPosition, List<Rectangle> obstacles, Rectangle levelBounds)
        {
            Position = startPosition;
            _obstacles = obstacles;
            _bounds = levelBounds;
            IsGrounded = false;
        }

        public void ApplyMovement(Vector2 direction, float speed, float deltaTime)
        {
            // x axis imput
            Velocity.X = direction.X * speed;

            // jump input
            if (IsGrounded && direction.Y > 0)
            {
                Velocity.Y = -JumpStrength;
                IsGrounded = false;
            }

            // set gravity
            Velocity.Y += Gravity * deltaTime;
        }

        public void Update(float deltaTime, int width, int height)
        {
            // box to check collision based on spritesize, adaptive
            Position.X += Velocity.X * deltaTime;
            Rectangle collisionBox = new Rectangle((int)Position.X, (int)Position.Y, width, height);

            foreach (var obstacle in _obstacles)
            {
                if (collisionBox.Intersects(obstacle)) // collision works on obstacles
                {
                    if (Velocity.X > 0) Position.X = obstacle.Left - width; // move left
                    else if (Velocity.X < 0) Position.X = obstacle.Right; // move right
                    collisionBox.X = (int)Position.X; // sync the box
                }
            }

            // move setup for y axis
            Position.Y += Velocity.Y * deltaTime;
            collisionBox.Y = (int)Position.Y;
            IsGrounded = false; // assume falling first

            foreach (var obstacle in _obstacles)
            {
                if (collisionBox.Intersects(obstacle))
                {
                    if (Velocity.Y > 0) // Landing
                    {
                        Position.Y = obstacle.Top - height;
                        Velocity.Y = 0;
                        IsGrounded = true;
                    }
                    else if (Velocity.Y < 0) // Hitting Head
                    {
                        Position.Y = obstacle.Bottom;
                        Velocity.Y = 0;
                    }
                    collisionBox.Y = (int)Position.Y; // Sync box
                }
            }

            if (!IsGrounded && Velocity.Y >= 0)
            {
                Rectangle groundSensor = new Rectangle((int)Position.X, (int)Position.Y + 1, width, height);
                foreach (var obstacle in _obstacles) // check if we touch _obstacle
                {
                    if (groundSensor.Intersects(obstacle))
                    {
                        IsGrounded = true;
                        break;
                    }
                }
            }

            // bounds clamp
            ClampToBounds(width, height);
        }

        private void ClampToBounds(int width, int height) // set window width, height bounds
        {
            float maxX = _bounds.X + _bounds.Width - width;
            float maxY = _bounds.Y + _bounds.Height - height;

            Position.X = MathHelper.Clamp(Position.X, _bounds.X, maxX);
            Position.Y = MathHelper.Clamp(Position.Y, _bounds.Y, maxY);

            // if we hit screenfloor stop falling
            if (Position.Y >= maxY)
            {
                Velocity.Y = 0;
                IsGrounded = true;
            }
        }
    }
}
