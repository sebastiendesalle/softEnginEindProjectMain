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

            // left / right
            if (state.IsKeyDown(Keys.Q) || state.IsKeyDown(Keys.Left))
            {
                direction.X -= 1;
            }
            if (state.IsKeyDown(Keys.D) || state.IsKeyDown(Keys.Right))
            {
                direction.X += 1;
            }

            // up / down
            if (state.IsKeyDown(Keys.Z) || state.IsKeyDown(Keys.Up))
            {
                direction.Y -= 1;
            }
            if (state.IsKeyDown(Keys.S) || state.IsKeyDown(Keys.Down))
            {
                direction.Y += 1;
            }
            return direction;
        }

        public bool ReadJump()
        {
            KeyboardState state = Keyboard.GetState();
            return state.IsKeyDown(Keys.Space);
        }
    }
}
