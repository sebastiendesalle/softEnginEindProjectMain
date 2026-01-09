using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoFactory.Entities.Machines.States
{
    public class WaitingForInputState: IMachineState
    {
        public void Enter(Machine machine)
        {
            Debug.WriteLine("Machine ready for input");
            machine.SourceRect = new Rectangle(0, 412, 64, 100);
        }

        public void Interact(Hero hero, Machine machine)
        {
            foreach (var entry in hero.Inventory.Items)
            {
                var item = entry.Value.Item;
                if (machine.IsIngredient(item.GetId()))
                {
                    hero.Inventory.RemoveItem(item, 1);
                    machine.AddToBuffer(item);
                    Debug.WriteLine($"Added {item.Name} to machine");

                    if (machine.TryCraft())
                    {
                        return;
                    }
                    return;
                }
            }
            Debug.WriteLine("No valid ingredients found in inventory");
        }

        public void Update(GameTime gameTime, Machine machine) { }

        public void Draw(SpriteBatch spriteBatch, Machine machine) { }
    }
}
