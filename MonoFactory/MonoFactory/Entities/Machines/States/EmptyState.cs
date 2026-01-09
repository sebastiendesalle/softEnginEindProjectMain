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
    public class EmptyState: IMachineState
    {
        public void Enter(Machine machine) 
        {
            Debug.WriteLine("Machine ready for input");
            machine.SourceRect = new Rectangle(0, 412, 64, 100);
        }

        public void Interact(Hero hero, Machine machine)
        {
            if (hero.Inventory.HasItem(machine.InputItemName, 1))
            {
                hero.Inventory.RemoveItem(machine.InputItemName, 1);

                machine.SetState(new ProcessingState(machine.ProcessTime, machine.OutputItem));
            }
            else
            {
                Debug.WriteLine($"Machine needs {machine.InputItemName} to work yo");
            }
        }

        public void Update(GameTime gameTime, Machine machine) 
        {
            
        }

        public void Draw(SpriteBatch spriteBatch, Machine machine)
        {

        }
    }
}
