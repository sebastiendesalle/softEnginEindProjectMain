using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoFactory.Entities.Machines.States
{
    public class FinishedState: IMachineState
    {
        public void Enter(Machine machine)
        {
            
        }

        public void Interact(Hero hero, Machine machine)
        {
            bool added = hero.Inventory.AddItem(machine.OutputItem, 1);

            if (added)
            {
                Debug.WriteLine("machine crafted item collected");

                machine.SetState(new EmptyState());
            }
            else
            {
                Debug.WriteLine("machine inventory full");
            }
        }

        public void Update(GameTime gameTime, Machine machine) { }

        public void Draw(SpriteBatch spriteBatch, Machine machine)
        {
            spriteBatch.Draw(machine.Texture, machine.Rectangle, Color.Green);
        }
    }
}
