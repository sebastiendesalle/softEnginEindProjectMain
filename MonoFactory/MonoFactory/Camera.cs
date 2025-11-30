using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoFactory
{
    public class Camera
    {
        public Matrix Transform { get; private set; }

        public void Follow(Vector2 targetPosition, int viewportWidth, int viewportHeight)
        {

            // center player
            var position = Matrix.CreateTranslation(
                -targetPosition.X,
                -targetPosition.Y,
                0);


            // offset to center of screen
            var offset = Matrix.CreateTranslation(
                viewportWidth / 2f,
                viewportHeight / 2f,
                0);

            Transform = position * offset;
        }
    }
}
