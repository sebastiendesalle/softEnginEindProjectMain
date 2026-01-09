using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoFactory.Items
{
    public class BaseItem : IItem
    {
        public string Name { get; protected set; }

        public ItemType Type { get; protected set; }

        public int Level { get; protected set; }

        public int StackSize { get; protected set; } = 99;

        protected BaseItem(string name, ItemType type, int level)
        {
            Name = name;
            Type = type;
            Level = level;
        }

        public string GetId()
        {
            return $"{Name}_{Level}";
        }

        public bool Equals(IItem other)
        {
            if (other == null)
            {
                return false;
            }
            return GetId() == other.GetId();
        }

        public override bool Equals(object obj)
        {
            if (obj is IItem item)
            {
                return Equals(item);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return GetId().GetHashCode();
        }
    }
}
