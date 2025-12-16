using Microsoft.Xna.Framework;

namespace MonoFactory.Strategies
{
    public class PatrolStrategy : IMovementStrategy
    {
        private float _speed = 60f;
        private Vector2 _startPoint;
        private Vector2 _endPoint;
        private bool _movingToEnd = true;

        public PatrolStrategy(Vector2 start, Vector2 end)
        {
            _startPoint = start;
            _endPoint = end;
        }
        public Vector2 Move(GameTime gameTime, Vector2 currentPosition, Vector2 targetPosition)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 target = _movingToEnd ? _endPoint : _startPoint;

            Vector2 direction = target - currentPosition;
            float distance = direction.Length();

            if (distance < 5f)
            {
                _movingToEnd = !_movingToEnd;
                return currentPosition;
            }

            direction.Normalize();
            return currentPosition + (direction * _speed * dt);
        }
    }
}
