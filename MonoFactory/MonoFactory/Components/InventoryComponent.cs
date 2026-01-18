using MonoFactory.Items;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoFactory.Components
{
    public class InventoryEntry
    {
        public IItem Item { get; set; }
        public int Count { get; set; }
    }
    public class InventoryComponent
    {
        private Dictionary<string, InventoryEntry> _slots = new Dictionary<string, InventoryEntry>();
        private int _maxCapacity;

        public IReadOnlyDictionary<string, InventoryEntry> Items => _slots;

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
            string id = item.GetId();

            if (!_slots.ContainsKey(id) && _slots.Count >= _maxCapacity)
            {
                Debug.WriteLine("Inventory full");
                return false;
            }

            if (!_slots.ContainsKey(id))
            {
                _slots[id] = new InventoryEntry { Item = item, Count = 0 };
            }

            _slots[id].Count += amount;
            Debug.WriteLine($"Added {amount} {item.Name}. (lvl {item.Level}). Total: {_slots[id].Count}");
            return true;
        }

        public bool HasItem(string itemId, int amount)
        {
            return _slots.ContainsKey(itemId) && _slots[itemId].Count >= amount;
        }

        public bool HasItem(IItem item, int amount)
        {
            if (item == null)
            {
                return false;
            }
            return HasItem(item.Name, amount);
        }

        public void RemoveItem(string itemId, int amount)
        {
            if (HasItem(itemId, amount))
            {
                _slots[itemId].Count -= amount;
                int remaining = _slots[itemId].Count;
                if (_slots[itemId].Count <= 0)
                {
                    _slots.Remove(itemId);
                }
                Debug.WriteLine($"[Inventory] Removed {amount} {itemId}. Remaining: {remaining}");
            }
        }

        public void RemoveItem(IItem item, int amount)
        {
            if (item != null)
            {
                RemoveItem(item.GetId(), amount);
            }
        }

        public int GetItemCount(string itemId)
        {
            if (_slots.ContainsKey(itemId))
            {
                return _slots[itemId].Count;
            }
            return 0;
        }

        public WeaponItem GetBestWeapon()
        {
            WeaponItem best = null;
            foreach (var entry in _slots.Values)
            {
                if (entry.Item is WeaponItem weapon)
                {
                    if (best == null || weapon.Damage > best.Damage)
                    {
                        best = weapon;
                    }
                }
            }
            return best;
        }
    }
}
