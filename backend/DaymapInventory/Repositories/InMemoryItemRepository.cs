using DaymapInventory.Interfaces;
using DaymapInventory.Models;

namespace DaymapInventory.Repositories
{
    // Used for testing - no database required
    public class InMemoryItemRepository : IItemRepository
    {
        private readonly List<Item> _items = new();
        private int _nextId = 1;

        public Task<IEnumerable<Item>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<Item>>(_items);
        }

        public Task<Item?> GetByIdAsync(int id)
        {
            var item = _items.FirstOrDefault(i => i.Id == id);
            return Task.FromResult(item);
        }

        public Task<Item> CreateAsync(Item item)
        {
            item.Id = _nextId++;
            item.CreatedAt = DateTime.UtcNow;
            item.UpdatedAt = DateTime.UtcNow;
            _items.Add(item);
            return Task.FromResult(item);
        }

        public Task<Item?> UpdateAsync(int id, Item updated)
        {
            var existing = _items.FirstOrDefault(i => i.Id == id);
            if (existing == null) return Task.FromResult<Item?>(null);

            existing.Name = updated.Name;
            existing.Description = updated.Description;
            existing.StockCount = updated.StockCount;
            existing.LowStockThreshold = updated.LowStockThreshold;
            existing.Status = updated.Status;
            existing.UpdatedAt = DateTime.UtcNow;

            return Task.FromResult<Item?>(existing);
        }

        public Task<bool> DeleteAsync(int id)
        {
            var item = _items.FirstOrDefault(i => i.Id == id);
            if (item == null) return Task.FromResult(false);

            _items.Remove(item);
            return Task.FromResult(true);
        }
    }
}
