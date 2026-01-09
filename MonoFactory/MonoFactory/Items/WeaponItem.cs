using MonoFactory.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoFactory.Items
{
    public class WeaponItem : BaseItem
    {
        public int Damage { get; private set; }

        public WeaponItem(string name, int level, int damage)
            : base(name, ItemType.Weapon, level)
        {
            Damage = damage;
            StackSize = 1;
        }
    }
}
