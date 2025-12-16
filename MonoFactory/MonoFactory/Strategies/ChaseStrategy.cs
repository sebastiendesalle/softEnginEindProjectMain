using Microsoft.Xna.Framework;


namespace MonoFactory.Strategies
{
    public class ChaseStrategy : IMovementStrategy
    {
        private float _speed = 80f;

        public Vector2 Move(GameTime gameTime, Vector2 currentPosition, Vector2 targetPosition)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 direction = targetPosition - currentPosition;

            if (direction != Vector2.Zero)
            {
                direction.Normalize();

                return currentPosition + (direction * _speed * dt);
            }

            return currentPosition;
        }
    }
}
