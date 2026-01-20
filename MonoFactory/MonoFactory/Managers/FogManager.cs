using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace MonoFactory.Managers
{
    public class FogManager
    {
        private Texture2D _fogTexture;
        private GraphicsDevice _graphicsDevice;

        public float Radius { get; set; } = 1000f;
        public float Opacity { get; set; } = 1f;
        public Color FogColor { get; set; } = Color.Gray;

        public FogManager(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            _fogTexture = CreateFogTexture(512);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 centerPosition)
        {
            if (_fogTexture == null)
            {
                return;
            }

            float scale = (Radius * 2.8f) / _fogTexture.Width;
            Vector2 origin = new Vector2(_fogTexture.Width / 2f, _fogTexture.Height / 2f);

            spriteBatch.Draw(
                _fogTexture,
                centerPosition,
                null,
                FogColor * Opacity,
                0f,
                origin,
                scale,
                SpriteEffects.None,
                0f
                );
        }

        private Texture2D CreateFogTexture(int size)
        {
            Texture2D texture = new Texture2D(_graphicsDevice, size, size);
            Color[] data = new Color[size * size];
            Vector2 center = new Vector2(size / 2f);
            float maxDistance = size / 2f;

            for (int i = 0; i < data.Length; i++)
            {
                int x = i % size;
                int y = i / size;

                float dist = Vector2.Distance(new Vector2(x, y), center);
                float factor = dist / maxDistance;

                float alpha = (float)Math.Pow(factor, 3);

                if (alpha > 1f)
                {
                    alpha = 1f;
                }

                data[i] = Color.White * alpha;
            }

            texture.SetData(data);
            return texture;
        }
    }
}
