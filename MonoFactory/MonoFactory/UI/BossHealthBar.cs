using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoFactory.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoFactory.UI
{
    public class BossHealthBar
    {
        private Texture2D _pixel;
        private GraphicsDevice _graphics;

        public BossHealthBar(GraphicsDevice graphics)
        {
            _graphics = graphics;
            _pixel = new Texture2D(graphics, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }

        public void Draw(SpriteBatch spriteBatch, Boss boss)
        {
            if (boss == null || boss.IsDead)
            {
                return;
            }

            int screenWidth = _graphics.Viewport.Width;
            int barWidth = 800;
            int barHeight = 30;

            Vector2 position = new Vector2((screenWidth - barWidth) / 2, 50);

            spriteBatch.Draw(_pixel, new Rectangle((int)position.X - 4, (int)position.Y - 4, barWidth + 8, barHeight + 8), Color.Black);

            float hpPercent = (float)boss.Health / boss.MaxHp;
            int currentWidth = (int)(barWidth * hpPercent);

            spriteBatch.Draw(_pixel, new Rectangle((int)position.X, (int)position.Y, currentWidth, barHeight), Color.Red);
        }
    }
}
