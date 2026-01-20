using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoFactory.Components.Animation
{
    public class AnimationFrame
    {
        public Rectangle SourceRectangle { get; set; }

        public AnimationFrame(Rectangle sourceRectangle)
        {
            SourceRectangle = sourceRectangle;
        }
    }
}
