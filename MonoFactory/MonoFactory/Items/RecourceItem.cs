using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoFactory.Items
{
    public class RecourceItem: IItem
    {
        public string Name { get; private set; }
        public int StackSize { get; private set; } = 99;

        public RecourceItem(string name)
        {
            Name = name;
        }
    }
}
