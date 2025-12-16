using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Input;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MonoFactory.Inputs
{
    public interface IInputReader
    {
        Vector2 ReadInput();
        bool ReadJump();
    }
}
