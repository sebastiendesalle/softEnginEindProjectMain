using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoFactory.Core;
using MonoFactory.Managers;
using MonoFactory.Strategies;

namespace MonoFactory.Entities
{
    public class Boss: Enemy
    {
        public int MaxHp { get; private set; }

        private BossStrategy _bossStrategy;

        public Boss(Texture2D texture, Vector2 position, BossStrategy strategy, WorldManager world, SoundManager sound, int hp): base(texture, position, strategy, world, sound, hp)
        {
            MaxHp = hp;
            _bossStrategy = strategy;

            Scale = 3.0f;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_bossStrategy.ShouldShoot)
            {
                ShootProjectile();
                _bossStrategy.ResetShootFlag();
            }
        }

        private void ShootProjectile()
        {
            _soundManager.PlaySound("Hit");
        }

    }
}
