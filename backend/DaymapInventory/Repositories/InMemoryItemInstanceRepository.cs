using DaymapInventory.Interfaces;
using DaymapInventory.Models;

namespace DaymapInventory.Repositories
{
    public class InMemoryItemInstanceRepository : IItemInstanceRepository
    {
        private readonly List<ItemInstance> _instances = new();
        private int _nextId = 1;

        public ItemInstance? GetById(int id)
        {
            return _instances.FirstOrDefault(i => i.Id == id);
        }

        public IEnumerable<ItemInstance> GetAll()
        {
            return _instances;
        }

        public void Add(ItemInstance instance)
        {
            instance.Id = _nextId++;
            instance.CreatedAt = DateTime.UtcNow;
            instance.UpdatedAt = DateTime.UtcNow;
            _instances.Add(instance);
        }

        public void Update(ItemInstance instance)
        {
            var existing = GetById(instance.Id);
            if (existing == null) return;

            existing.SerialNumber = instance.SerialNumber;
            existing.ExpiryDate = instance.ExpiryDate;
            existing.Status = instance.Status;
            existing.Metadata = instance.Metadata;
            existing.UpdatedAt = DateTime.UtcNow;
        }

        public void Delete(int id)
        {
            _instances.RemoveAll(i => i.Id == id);
        }

        public IEnumerable<ItemInstance> GetByItemId(int itemId)
        {
            return _instances.Where(i => i.ItemId == itemId);
        }

        public IEnumerable<ItemInstance> GetByStatus(string status)
        {
            return _instances.Where(i => i.Status == status);
        }

        public IEnumerable<ItemInstance> GetExpiringSoon(DateTime before)
        {
            return _instances.Where(i =>
                i.ExpiryDate.HasValue &&
                i.ExpiryDate.Value <= before);
        }
    }
}
