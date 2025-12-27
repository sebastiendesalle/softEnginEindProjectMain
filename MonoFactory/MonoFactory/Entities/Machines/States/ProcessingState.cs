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
    public class ProcessingState: IMachineState
    {
        public float _timer;

        public void Enter(Machine machine)
        {
            _timer = 0f;
        }

        public void Interact(Hero hero, Machine machine)
        {
            Debug.WriteLine("machine processing");
        }

        public void Update(GameTime gameTime, Machine machine)
        {
            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_timer >= machine.ProcessTime)
            {
                machine.SetState(new FinishedState());
            }
        }

        public void Draw(SpriteBatch spriteBatch, Machine machine)
        {
            // red to show it's working
            spriteBatch.Draw(machine.Texture, machine.Rectangle, Color.Red);
        }
    }
}
