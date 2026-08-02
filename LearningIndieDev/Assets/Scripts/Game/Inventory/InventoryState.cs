using System.Collections.Generic;

namespace SaltyGame
{
    public sealed class InventoryState
    {
        readonly Dictionary<string, int> amounts = new Dictionary<string, int>();

        public void Add(string resourceId, int amount)
        {
            if (string.IsNullOrEmpty(resourceId) || amount <= 0)
                return;

            amounts.TryGetValue(resourceId, out var current);
            amounts[resourceId] = current + amount;
        }

        public int Get(string resourceId)
        {
            return amounts.TryGetValue(resourceId, out var amount) ? amount : 0;
        }

        public bool TryRemove(string resourceId, int amount)
        {
            if (string.IsNullOrEmpty(resourceId) || amount <= 0 || Get(resourceId) < amount)
                return false;

            amounts[resourceId] -= amount;
            return true;
        }
    }
}
