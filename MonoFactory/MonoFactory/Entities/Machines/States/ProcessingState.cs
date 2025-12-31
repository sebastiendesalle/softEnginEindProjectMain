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
        private float _timer;
        private float _totalTime;
        private Texture2D _pixel;

        public ProcessingState(float duration)
        {
            _totalTime = duration;
            _timer = duration;
        }

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
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _timer += delta;

            if (_timer <= 0)
            {
                machine.SetState(new FinishedState());
            }
        }

        public void Draw(SpriteBatch spriteBatch, Machine machine)
        {
            if (_pixel == null)
            {
                _pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                _pixel.SetData(new[] { Color.White });
            }

            float progress = 1.0f - (_timer / _totalTime);
            int barWidth = 50;
            int fillWidth = (int)(barWidth * progress);

            Vector2 barPos = machine.Position + new Vector2(machine.Rectangle.Width / 2 - barWidth / 2, -15);

            // black background
            spriteBatch.Draw(_pixel, new Rectangle((int)barPos.X, (int)barPos.Y, barWidth, 5), Color.Black);

            spriteBatch.Draw(_pixel, new Rectangle((int)barPos.X, (int)barPos.Y, fillWidth, 5), Color.Orange);
        }
    }
}
