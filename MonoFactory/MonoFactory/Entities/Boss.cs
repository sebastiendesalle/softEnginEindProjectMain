using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoFactory.Components.Animation;
using MonoFactory.Core;
using MonoFactory.Managers;
using MonoFactory.Strategies;
using System.Collections.Generic;

namespace MonoFactory.Entities
{
    public class Boss: Enemy
    {
        public int MaxHp { get; private set; }
        private BossStrategy _bossStrategy;
        private float _bossDamageCooldown = 0f;

        public Boss(Texture2D texture, Vector2 position, BossStrategy strategy, WorldManager world, SoundManager sound, int hp): base(texture, position, strategy, world, sound, hp)
        {
            MaxHp = hp;
            _bossStrategy = strategy;

            _useDefaultAttackLogic = false;

            Scale = 3.0f;
        }

        public override void LoadAnimations()
        {
            _animations = new Dictionary<string, Components.Animation.Animation>();

            var idleAnim = new Animation();
            for (int i = 0; i < 4; i++)
            {
                idleAnim.AddFrame(new AnimationFrame(new Rectangle(i * 64, 0, 64, 64)));
            }
            _animations.Add("Idle", idleAnim);

            var runAnim = new Animation();
            for (int i = 0; i < 6; i++)
            {
                runAnim.AddFrame(new AnimationFrame(new Rectangle(i * 64, 64, 64, 64)));
            }
            _animations.Add("Run", runAnim);

            var attackAnim = new Animation();
            attackAnim.IsLooping = false;
            attackAnim.FPS = 15;
            for (int i = 0; i < 6; i++)
            {
                attackAnim.AddFrame(new AnimationFrame(new Rectangle(i * 64, 256, 64, 64)));
            }
            _animations.Add("Attack1", attackAnim);
            _animations.Add("Attack", attackAnim);

            _animations.Add("Death", idleAnim);

            _currentAnimation = _animations["Idle"];

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_bossStrategy.ShouldShoot)
            {
                _soundManager.PlaySound("Hit");
                _bossStrategy.ResetShootFlag();

                _currentAnimation = _animations["Attack"];
                _currentAnimation.Reset();
            }

            if (_currentAnimation == _animations["Attack"])
            {
                if (!_currentAnimation.IsFinished)
                {
                    _currentAnimation = _animations["Attack"];
                }
            }

            if (_bossDamageCooldown > 0)
            {
                _bossDamageCooldown -= dt;
            }

            if (_targetHero != null)
            {
                float dist = Vector2.Distance(Position, _targetHero.Position);

                if (dist < 80f && _bossDamageCooldown <= 0)
                {
                    int damageAmount = 1;

                    if (_currentAnimation == _animations["Attack"])
                    {
                        damageAmount = 2;
                    }

                    _targetHero.TakeDamage(damageAmount);

                    _bossDamageCooldown = 1.0f;
                }
            }
        }
    }
}
