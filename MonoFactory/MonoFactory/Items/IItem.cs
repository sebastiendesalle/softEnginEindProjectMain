using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoFactory.Items
{
    public enum ItemType
    {
        Resource,
        Weapon
    }
    public interface IItem : IEquatable<IItem>
    {
        string Name { get; }
        ItemType Type { get; }
        int Level { get; }
        int StackSize { get; }
        string GetId();

    }
}
