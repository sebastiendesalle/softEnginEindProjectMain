using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MonoFactory.Input
{
    class KeyboardReader : IInputReader
    {
        public Vector2 ReadInput()
        {
            KeyboardState state = Keyboard.GetState();
            Vector2 direction = Vector2.Zero;

            // horizontal movement
            if (state.IsKeyDown(Keys.Q) || state.IsKeyDown(Keys.Left))
            {
                direction.X -= 1;
            }
            if (state.IsKeyDown(Keys.D) || state.IsKeyDown(Keys.Right))
            {
                direction.X += 1;
            }

            // jump movement
            if (state.IsKeyDown(Keys.Space) || state.IsKeyDown(Keys.Up) || state.IsKeyDown(Keys.Z))
            {
                direction.Y = 1;
            }
            return direction;
        }
    }
}
