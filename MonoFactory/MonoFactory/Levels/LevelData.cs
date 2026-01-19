using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MonoFactory.Levels
{

    public class EnemyWave
    {
        public string EnemyType { get; set; }
        public int Count { get; set; }
        public int Hp { get; set; }
        public int Damage { get; set; }
        public float Speed { get; set; }
        public bool CanShoot { get; set; }
    }
    public class LevelData
    {
        public int Index { get; set; }
        public string MusicTrack { get; set; }
        public bool IsSafeZone { get; set; }

        public List<EnemyWave> Waves { get; set; } = new List<EnemyWave>();

        public List<(string Type, Vector2 Pos)> StaticEntities { get; set; } = new List<(string Type, Vector2 Pos)>();

        public int ResourceCount { get; set; } = 0;


    }
}
