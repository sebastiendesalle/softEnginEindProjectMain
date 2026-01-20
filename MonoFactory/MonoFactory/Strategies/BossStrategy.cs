using Microsoft.Xna.Framework;
using MonoFactory.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoFactory.Strategies
{
    public enum BossState
    {
        Chasing,
        Charging,
        Resting
    }
    public class BossStrategy: IMovementStrategy
    {
        private Hero _target;
        private BossState _currentState;

        private float _stateTimer;
        private float _chargeSpeedMultiplier = 3.5f;

        public bool ShouldShoot { get; private set; }

        public BossStrategy(Hero target)
        {
            _target = target;
            _currentState = BossState.Chasing;
        }

        public Vector2 Move(GameTime gameTime, Vector2 currentPosition, Vector2 targetPosition)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _stateTimer += dt;

            float baseSpeed = 100f;

            switch (_currentState)
            {
                case BossState.Chasing:
                    if (_stateTimer > 4.0f)
                    {
                        ChangeState(BossState.Charging);
                    }
                    return MoveTowards(currentPosition, targetPosition, baseSpeed * dt);
                case BossState.Charging:
                    if (_stateTimer > 1.5f)
                    {
                        ChangeState(BossState.Resting);
                    }
                    return MoveTowards(currentPosition, targetPosition, (baseSpeed * 3.5f) * dt);
                case BossState.Resting:
                    if (_stateTimer > 2.0f)
                    {
                        ChangeState(BossState.Chasing);
                    }
                    return currentPosition;
                default:
                    return currentPosition;
            }
        }

        private void ChangeState(BossState newState)
        {
            _currentState = newState;
            _stateTimer = 0f;

            if (newState == BossState.Resting)
            {
                ShouldShoot = true;
            }
        }

        private Vector2 MoveTowards(Vector2 current, Vector2 target, float speed)
        {
            Vector2 direction = target - current;
            if (direction != Vector2.Zero)
            {
                direction.Normalize();
            }
            return current + direction * speed;
        }
        public void ResetShootFlag() => ShouldShoot = false;

    }
}
