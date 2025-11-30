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

        private float friction = 0.85f; // make movement feel good
        private float speed = 50f; // acceleration speed

        public PhysicsComponent(Vector2 startPosition)
        {
            Position = startPosition;
        }

        public void ApplyMovement(Vector2 inputDirection, float deltaTime)
        {
            // add to velocity for smoothness
            if (inputDirection != Vector2.Zero)
            {
                inputDirection.Normalize(); // make diagonal same speed
                Velocity += inputDirection * speed;
            }
        }

        public void Update(float deltaTime)
        {
            // set velocity
            Position += Velocity * deltaTime;

            // set friction to slow down
            Velocity *= friction;

            if (Velocity.Length() < 0.1f)
            {
                Velocity = Vector2.Zero;
            }
        }
    }
}
