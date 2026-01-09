using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoFactory.Items
{
    public class ResourceItem: BaseItem
    {
        public ResourceItem(string name)
            : base(name, ItemType.Resource, 1)
        {
        }
    }
}
