using MonoFactory.Items;
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
        private Dictionary<string, int> _slots = new Dictionary<string, int>();
        private int _maxCapacity;

        public InventoryComponent(int capacity = 20)
        {
            _maxCapacity = capacity;
        }

        public bool AddItem(IItem item, int amount)
        {

            if (item == null)
            {
                return false;
            }

            if (!_slots.ContainsKey(item.Name) && _slots.Count >= _maxCapacity)
            {
                Debug.WriteLine("Inventory full");
                return false;
            }

            if (!_slots.ContainsKey(item.Name))
            {
                _slots[item.Name] = 0;
            }

            _slots[item.Name] += amount;
            Debug.WriteLine($"Added {amount} {item.Name}. Total: {_slots[item.Name]}");
            return true;
        }

        public bool HasItem(string itemName, int amount)
        {
            return _slots.ContainsKey(itemName) && _slots[itemName] >= amount;
        }

        public void RemoveItem(string itemName, int amount)
        {
            if (HasItem(itemName, amount))
            {
                _slots[itemName] -= amount;
                if (_slots[itemName] <= 0)
                {
                    _slots.Remove(itemName);
                }
                Debug.WriteLine($"[Inventory] Removed {amount} {itemName}. Remaining: {_slots[itemName]}");
            }
        }

        public int GetItemCount(string itemName)
        {
            if (_slots.ContainsKey(itemName))
            {
                return _slots[itemName];
            }
            return 0;
        }
    }
}
