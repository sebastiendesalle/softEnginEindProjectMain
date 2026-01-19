using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoFactory.Core;
using MonoFactory.Entities;
using MonoFactory.Factories;
using MonoFactory.Levels;
using MonoFactory.Strategies;
using System;
using System.Collections.Generic;

namespace MonoFactory.Managers
{
    public class LevelManager
    {
        private Dictionary<int, LevelData> _levels;
        private Random _random;

        public LevelManager()
        {
            _levels = new Dictionary<int, LevelData>();
            _random = new Random();
            InitialiseLevels();
        }

        private void InitialiseLevels()
        {
            var lvl1 = new LevelData
            {
                Index = 1,
                MusicTrack = "Crafting",
                IsSafeZone = true,
                ResourceCount = 50
            };

            lvl1.StaticEntities.Add(("Furnace", GridHelper.GridToWorld(8,5)));
            lvl1.StaticEntities.Add(("Crafter", GridHelper.GridToWorld(12, 5)));
            _levels.Add(1, lvl1);

            // LEVEL 2 START
            // intro level
            var lvl2 = new LevelData
            {
                Index = 2,
                MusicTrack = "Battle",
                IsSafeZone = false
            };
            lvl2.Waves.Add(new EnemyWave
            {
                EnemyType = "Goblin_Chaser",
                Count = 1,
                Hp = 9,
                Damage = 1,
                Speed = 80f
            });
            lvl2.Waves.Add(new EnemyWave
            {
                EnemyType = "Goblin_Patrol",
                Count = 1,
                Hp = 12,
                Damage = 1,
                Speed = 80f
            });
            lvl2.Waves.Add(new EnemyWave
            {
                EnemyType = "Goblin_Turret",
                Count = 1,
                Hp = 15,
                Damage = 1,
                Speed = 0f,
                CanShoot = true
            });
            _levels.Add(2, lvl2);

            //LEVEL 2 END
            //LEVEL 3 START
            // swarm level
            var lvl3 = new LevelData
            {
                Index = 3,
                MusicTrack = "Battle",
                IsSafeZone = false
            };
            lvl3.Waves.Add(new EnemyWave
            {
                EnemyType = "Goblin_Chaser",
                Count = 8,
                Hp = 5,
                Damage = 1,
                Speed = 130f
            });
            _levels.Add(3, lvl3);

            //LEVEL 3 END
            //LEVEL 4 START
            // many turrets level
            var lvl4 = new LevelData
            {
                Index = 4,
                MusicTrack = "Battle",
                IsSafeZone = false
            };
            lvl4.Waves.Add(new EnemyWave
            {
                EnemyType = "Goblin_Turret",
                Count = 5,
                Hp = 15,
                Damage = 1,
                Speed = 0f,
                CanShoot = true
            });
            _levels.Add(4, lvl4);

            //LEVEL 4 END
            //LEVEL 5 START
            // tank level

            var lvl5 = new LevelData
            {
                Index = 5,
                MusicTrack = "Battle",
                IsSafeZone = false
            };
            lvl5.Waves.Add(new EnemyWave
            {
                EnemyType = "Goblin_Chaser",
                Count = 3,
                Hp = 25,
                Damage = 3,
                Speed = 50f
            });
            _levels.Add(5, lvl5);

            //LEVEL 5 END
            //LEVEL 6 START
            // mix everything

            var lvl6 = new LevelData
            {
                Index = 5,
                MusicTrack = "Battle",
                IsSafeZone = false
            };
            lvl6.Waves.Add(new EnemyWave
            {
                EnemyType = "Goblin_Chaser", Count = 3, Hp = 15, Damage = 2, Speed = 100f
            });
            lvl6.Waves.Add(new EnemyWave
            {
                EnemyType = "Goblin_Turret",
                Count = 2,
                Hp = 20,
                Damage = 2,
                Speed = 0,
                CanShoot = true
            });
            lvl6.Waves.Add(new EnemyWave
            {
                EnemyType = "Goblin_Patroller",
                Count = 2,
                Hp = 20,
                Damage = 2,
                Speed = 90f
            });
            _levels.Add(6, lvl6);

            //END LEVEL 6
            // START LEVEL 7
            // final boss level

            var lvl7 = new LevelData
            {
                Index = 7,
                MusicTrack = "Battle",
                IsSafeZone = false
            };
            _levels.Add(7, lvl7);

            //END LEVEL 7
        }
        public void LoadLevel(int index, WorldManager world, EntityFactory factory, Texture2D enemyTex, SoundManager soundManager, Texture2D projectileTex, Hero hero)
        {
            if (!_levels.ContainsKey(index)) return;

            LevelData data = _levels[index];

            foreach (var entity in data.StaticEntities)
            {
                world.AddEntity(factory.CreateEntity(entity.Type, entity.Pos));
            }

            foreach (var wave in data.Waves)
            {
                for (int i = 0; i < wave.Count; i++)
                {
                    Vector2 pos = GetRandomSpawnPos();


                    IMovementStrategy strategy;
                    if (wave.EnemyType == "Goblin_Patrol")
                    {
                        strategy = new PatrolStrategy(pos, pos + new Vector2(200, 0));
                    }
                    else if (wave.EnemyType == "Goblin_Turret")
                    {
                        strategy = new StationaryStrategy();
                    }
                    else
                    {
                        strategy = new ChaseStrategy(wave.Speed);
                    }

                    var enemy = new Enemy(enemyTex, pos, strategy, world, soundManager, wave.Hp, wave.Damage, wave.CanShoot);
                    enemy.SetTarget(hero);

                    if (wave.CanShoot)
                    {
                        enemy.SetProjectileTexture(projectileTex);
                    }


                    world.AddEntity(enemy);
                }
            }
        }

        public LevelData GetLevelData(int index)
        {
            if (_levels.ContainsKey(index)) return _levels[index];
            return null;
        }

        private Vector2 GetRandomSpawnPos()
        {
            int x = _random.Next(15, 40);
            int y = _random.Next(5, 20);
            return GridHelper.GridToWorld(x, y);
        }
    }
}
