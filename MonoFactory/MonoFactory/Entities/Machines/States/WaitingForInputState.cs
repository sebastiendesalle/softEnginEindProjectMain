using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoFactory.Items;

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
            if (hero.Inventory.Items.Count == 0)
            {
                Debug.WriteLine("Hero inventory is empty.");
                return;
            }

            var buffer = machine.GetBuffer();
            List<string> bufferIds = buffer.Select(i => i.GetId()).ToList();

            foreach (var entry in hero.Inventory.Items)
            {
                var item = entry.Value.Item;

                if (item is WeaponItem weapon && entry.Value.Count >= 2)
                {

                    if (weapon.Level >= 4)
                    {
                        Debug.WriteLine("Cannot upgrade, weapon max level");
                        continue;
                    }
                    string itemId = item.GetId();

                    if (machine.IsIngredient(itemId))
                    {
                        List<string> testBuffer = new List<string>(bufferIds);
                        testBuffer.Add(itemId);

                        if (machine.CouldMatchRecipe(testBuffer))
                        {
                            hero.Inventory.RemoveItem(item, 1);
                            machine.AddToBuffer(item);
                            Debug.WriteLine($"Added {item.Name} to machine buffer");
                            Debug.WriteLine($"[Machine] Buffer now contains: {string.Join(", ", machine.GetBuffer().Select(i => i.GetId()))}");

                            if (machine.TryCraft())
                            {
                                Debug.WriteLine("Crafting started.");
                                return;
                            }
                            return;
                        }

                    }
                }
            }

            foreach (var entry in hero.Inventory.Items)
            {
                var item = entry.Value.Item;
                string itemId = item.GetId();

                Debug.WriteLine($"Checking inventory item {itemId}");

                if (machine.IsIngredient(itemId))
                {
                    List<string> testBuffer = new List<string>(bufferIds);
                    testBuffer.Add(itemId);

                    if (machine.CouldMatchRecipe(testBuffer))
                    {
                        hero.Inventory.RemoveItem(item, 1);
                        machine.AddToBuffer(item);
                        Debug.WriteLine($"Added {item.Name} to machine buffer");
                        Debug.WriteLine($"[Machine] Buffer now contains: {string.Join(", ", machine.GetBuffer().Select(i => i.GetId()))}");

                        if (machine.TryCraft())
                        {
                            Debug.WriteLine("Crafting started.");
                            return;
                        }
                        return;
                    }
                }
            }
        }

        public void Update(GameTime gameTime, Machine machine) { }

        public void Draw(SpriteBatch spriteBatch, Machine machine) { }
    }
}
