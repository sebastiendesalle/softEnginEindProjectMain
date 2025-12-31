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

        private Texture2D _pixel;
        private bool _itemSpawned = false;
        public void Enter(Machine machine)
        {
            Debug.WriteLine("Machine finished processing");
            machine.SourceRect = new Rectangle(0, 412, 64, 100);

            if (!_itemSpawned)
            {
                Vector2 spawnPos = machine.Position + new Vector2(machine.Rectangle.Width + 20, 0);
                DroppedItem droppedItem = new DroppedItem(
                    machine.OutputItem,
                    spawnPos,
                    machine.ItemTexture,
                    machine.OutputItemSourceRect
                );

                machine.World.AddEntity(droppedItem);
                Debug.WriteLine($"Spawned {machine.OutputItem.Name} at {spawnPos}");
                _itemSpawned = true;

                machine.SetState(new EmptyState());
            }
        }

        public void Interact(Hero hero, Machine machine)
        {
            Debug.WriteLine("Machine empty");
        }

        public void Update(GameTime gameTime, Machine machine) 
        { 
        
        }

        public void Draw(SpriteBatch spriteBatch, Machine machine)
        {
            if (_pixel == null)
            {
                _pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                _pixel.SetData(new[] { Color.White });
            }

            int indSize = 10;
            Vector2 indPos = machine.Position + new Vector2((machine.Rectangle.Width - indSize) / 2, -15);
            spriteBatch.Draw(_pixel, new Rectangle((int)indPos.X, (int)indPos.Y, indSize, indSize), Color.Green);
        }
    }
}
