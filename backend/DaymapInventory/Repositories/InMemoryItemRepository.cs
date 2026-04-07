using DaymapInventory.Interfaces;
using DaymapInventory.Models;

namespace DaymapInventory.Repositories
{
    public class InMemoryItemRepository : IItemRepository
    {
        private readonly List<Item> _items = new();
        private int _nextId = 1;

        public Item? GetById(int id)
        {
            return _items.FirstOrDefault(i => i.Id == id);
        }

        public IEnumerable<Item> GetAll()
        {
            return _items;
        }

        public void Add(Item item)
        {
            item.Id = _nextId++;
            item.CreatedAt = DateTime.UtcNow;
            item.UpdatedAt = DateTime.UtcNow;
            _items.Add(item);
        }

        public void Update(Item item)
        {
            var existing = GetById(item.Id);
            if (existing == null) return;

            existing.Name = item.Name;
            existing.Description = item.Description;
            existing.StockCount = item.StockCount;
            existing.Status = item.Status;
            existing.LowStockThreshold = item.LowStockThreshold;
            existing.UpdatedAt = DateTime.UtcNow;
        }

        public void Delete(int id)
        {
            _items.RemoveAll(i => i.Id == id);
        }

        public IEnumerable<Item> GetByStatus(string status)
        {
            return _items.Where(i => i.Status == status);
        }

        public IEnumerable<Item> GetLowStock()
        {
            return _items.Where(i => i.StockCount <= i.LowStockThreshold);
        }
    }
}
