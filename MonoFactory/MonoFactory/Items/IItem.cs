using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoFactory.Items
{
    public interface IItem
    {
        string Name { get; }
        int StackSize { get; }
    }
}
