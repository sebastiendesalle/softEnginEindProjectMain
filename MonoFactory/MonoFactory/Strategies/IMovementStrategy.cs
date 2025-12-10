using Microsoft.Xna.Framework;

namespace MonoFactory.Strategies
{
    public interface IMovementStrategy
    {
        Vector2 Move(GameTime gameTime, Vector2 currentPosition, Vector2 targetPosition);
    }
}
