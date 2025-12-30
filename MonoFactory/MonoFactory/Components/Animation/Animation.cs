using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MonoFactory.Components.Animation
{
    class Animation
    {
        public AnimationFrame CurrentFrame { get; set; }
        private List<AnimationFrame> frames;
        private int counter;
        private double secondCounter = 0;

        public bool IsLooping { get; set; } = true;

        public int FPS { get; set; } = 10;
        public Animation()
        {
            frames = new List<AnimationFrame>();
        }

        public void AddFrame(AnimationFrame frame)
        {
            frames.Add(frame);
            CurrentFrame = frames[0];
        }

        public void Update(GameTime gameTime)
        {
            CurrentFrame = frames[counter];

            secondCounter += gameTime.ElapsedGameTime.TotalSeconds;
            int fps = 10;

            if (secondCounter >= 1d/fps)
            {
                counter++;
                secondCounter = 0;
            }

            if (counter >= frames.Count)
            {
                if (IsLooping)
                {
                    counter = 0;
                }
                else
                {
                    counter = frames.Count - 1;
                }
                
            }
        }

        public void Reset()
        {
            counter = 0;
            secondCounter = 0;
            if (frames.Count > 0)
            {
                CurrentFrame = frames[0];
            }
        }

        public bool IsFinished
        {
            get
            {
                return !IsLooping && counter >= frames.Count - 1;
            }
        }
    }
}
