using DaymapInventory.Interfaces;
using DaymapInventory.Models;

namespace DaymapInventory.Repositories
{
    public class InMemoryItemRepository : IItemRepository
    {
        private readonly List<Item> _items = new();
        private int _nextId = 1;

        public Task<Item?> GetById(int id)
        {
            return Task.FromResult(_items.FirstOrDefault(i => i.Id == id));
        }

        public Task<IEnumerable<Item>> GetAll()
        {
            return Task.FromResult<IEnumerable<Item>>(_items);
        }

        public Task Add(Item item)
        {
            item.Id = _nextId++;
            item.CreatedAt = DateTime.UtcNow;
            item.UpdatedAt = DateTime.UtcNow;
            _items.Add(item);
            return Task.CompletedTask;
        }

        public Task Update(Item item)
        {
            var existing = _items.FirstOrDefault(i => i.Id == item.Id);
            if (existing == null) return Task.CompletedTask;

            existing.Name = item.Name;
            existing.Description = item.Description;
            existing.StockCount = item.StockCount;
            existing.Status = item.Status;
            existing.LowStockThreshold = item.LowStockThreshold;
            existing.UpdatedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        public Task Delete(int id)
        {
            _items.RemoveAll(i => i.Id == id);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Item>> GetByStatus(string status)
        {
            return Task.FromResult<IEnumerable<Item>>(_items.Where(i => i.Status == status));
        }

        public Task<IEnumerable<Item>> GetLowStock()
        {
            return Task.FromResult<IEnumerable<Item>>(_items.Where(i => i.StockCount <= i.LowStockThreshold));
        }
    }
}