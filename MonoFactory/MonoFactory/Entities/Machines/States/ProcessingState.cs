using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoFactory.Entities.Machines.States.Furnace;
using System.Diagnostics;
using MonoFactory.Items;


namespace MonoFactory.Entities.Machines.States
{
    public class ProcessingState: IMachineState
    {
        private float _timer;
        private float _totalTime;
        private Texture2D _pixel;

        private float _animTimer;
        private int _currentFrameIndex;

        private readonly int[] _frameXOffsets = { 64, 128, 192 };

        public ProcessingState(float duration)
        {
            _totalTime = duration;
            _timer = duration;
        }

        public void Enter(Machine machine)
        {
            _currentFrameIndex = 0;
            UpdateAnimation(machine);
        }

        public void Interact(Hero hero, Machine machine)
        {
            Debug.WriteLine("machine processing");
        }

        public void Update(GameTime gameTime, Machine machine)
        {
            var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _timer -= delta;

            _animTimer += delta;
            if (_animTimer > 0.2f)
            {
                _animTimer = 0;
                _currentFrameIndex = (_currentFrameIndex + 1) % _frameXOffsets.Length;
                UpdateAnimation(machine);
            }

            if (_timer <= 0)
            {
                machine.SetState(new FinishedState());
            }
        }

        public void UpdateAnimation(Machine machine)
        {
            int x = _frameXOffsets[_currentFrameIndex];

            machine.SourceRect = new Rectangle(x, 412, 64, 100);
        }

        public void Draw(SpriteBatch spriteBatch, Machine machine)
        {
            if (_pixel == null)
            {
                _pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                _pixel.SetData(new[] { Color.White });
            }

            float progress = 1.0f - _timer / _totalTime;
            int barWidth = 50;
            int fillWidth = (int)(barWidth * progress);

            Vector2 barPos = machine.Position + new Vector2(machine.Rectangle.Width / 2 - barWidth / 2, -15);

            // black background
            spriteBatch.Draw(_pixel, new Rectangle((int)barPos.X, (int)barPos.Y, barWidth, 5), Color.Black);

            spriteBatch.Draw(_pixel, new Rectangle((int)barPos.X, (int)barPos.Y, fillWidth, 5), Color.Orange);
        }
    }
}
