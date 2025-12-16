using Microsoft.Xna.Framework;

namespace MonoFactory.Strategies
{
    public class StationaryStrategy : IMovementStrategy
    {
        public Vector2 Move(GameTime gameTime, Vector2 currentPosition, Vector2 targetPosition)
        {
            return currentPosition;
        }
    }
}
