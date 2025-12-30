using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoFactory.Entities;
using System.Collections.Generic;

namespace MonoFactory.UI
{
    public class Hud
    {
        private SpriteFont _font;
        private Texture2D _pixel;

        public Hud(SpriteFont font, GraphicsDevice graphics)
        {
            _font = font;
            _pixel = new Texture2D(graphics, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }

        public void Draw(SpriteBatch spriteBatch, Hero hero)
        {
            if (_font == null)
            {
                return;
            }

            int barWidth = 200;
            int barHeight = 25;
            Vector2 barPos = new Vector2(20, 20);

            spriteBatch.Draw(_pixel, new Rectangle((int)barPos.X, (int)barPos.Y, barWidth, barHeight), Color.Gray);

            // calc hp percent max 10 hp rn

            float healthpct = (float)hero.Health / 10f;
            if (healthpct < 0)
            {
                healthpct = 0;
            }

            spriteBatch.Draw(_pixel, new Rectangle((int)barPos.X, (int)barPos.Y, (int)(barWidth * healthpct), barHeight), Color.Red);

            spriteBatch.DrawString(_font, $"HP: {hero.Health}/10", barPos + new Vector2(5, 2), Color.White);

            Vector2 itemPos = new Vector2(20, 60);
            spriteBatch.DrawString(_font, "Inventory: ", itemPos, Color.Gold);

            itemPos.Y += 25;

            if (hero.Inventory != null)
            {
                foreach (var item in hero.Inventory.Items)
                {
                    spriteBatch.DrawString(_font, $"- {item.Key}: {item.Value}", itemPos, Color.White);
                    itemPos.Y += 20;
                }
            }
        }
    }
}
