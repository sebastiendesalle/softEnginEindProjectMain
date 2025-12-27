using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoFactory.Entities.Machines;

namespace MonoFactory.Entities.Machines.States
{
    public interface IMachineState
    {
        void Enter(Machine machine);
        void Interact(Hero hero, Machine machine);
        void Update(GameTime gameTime, Machine machine);
        void Draw(SpriteBatch spriteBatch, Machine machine);
    }
}
