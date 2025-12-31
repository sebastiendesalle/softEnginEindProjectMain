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
        public void Enter(Machine machine)
        {
            Debug.WriteLine("Machine finished processing");
            machine.SourceRect = new Rectangle(0, 412, 64, 100);
        }

        public void Interact(Hero hero, Machine machine)
        {
            if (hero.Inventory.AddItem(machine.OutputItem, 1))
            {
                Debug.WriteLine($"Collected: {machine.OutputItem.Name}");

                machine.SetState(new EmptyState());
            }
            else
            {
                Debug.WriteLine("Inventory full, can't collect");
            }
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
