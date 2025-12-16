using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoFactory.Components
{
    public class InventoryComponent
    {
        // Stores items like: "Stone" -> 5, "Wood" -> 10
        private Dictionary<string, int> _items = new Dictionary<string, int>();
        private int _maxCapacity;

        public InventoryComponent(int capacity = 10)
        {
            _maxCapacity = capacity;
        }

        public void AddItem(string itemName, int amount)
        {
            if (!_items.ContainsKey(itemName))
            {
                _items[itemName] = 0;
            }
            _items[itemName] += amount;

            // Debug output so you can see it working in the Output window
            Debug.WriteLine($"[Inventory] {itemName}: {_items[itemName]}");
        }

        public bool HasItem(string itemName, int amount)
        {
            return _items.ContainsKey(itemName) && _items[itemName] >= amount;
        }

        public void RemoveItem(string itemName, int amount)
        {
            if (HasItem(itemName, amount))
            {
                _items[itemName] -= amount;
                Debug.WriteLine($"[Inventory] Removed {amount} {itemName}. Remaining: {_items[itemName]}");
            }
        }
    }
}
