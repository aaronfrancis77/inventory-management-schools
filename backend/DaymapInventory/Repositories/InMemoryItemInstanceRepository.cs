using DaymapInventory.Interfaces;
using DaymapInventory.Models;

namespace DaymapInventory.Repositories
{
    public class InMemoryItemInstanceRepository : IItemInstanceRepository
    {
        private readonly List<ItemInstance> _instances = new();
        private int _nextId = 1;

        public Task<ItemInstance?> GetById(int id)
        {
            return Task.FromResult(_instances.FirstOrDefault(i => i.Id == id));
        }

        public Task<IEnumerable<ItemInstance>> GetAll()
        {
            return Task.FromResult<IEnumerable<ItemInstance>>(_instances);
        }

        public Task Add(ItemInstance instance)
        {
            instance.Id = _nextId++;
            instance.CreatedAt = DateTime.UtcNow;
            instance.UpdatedAt = DateTime.UtcNow;
            _instances.Add(instance);
            return Task.CompletedTask;
        }

        public Task Update(ItemInstance instance)
        {
            var existing = _instances.FirstOrDefault(i => i.Id == instance.Id);
            if (existing == null) return Task.CompletedTask;

            existing.SerialNumber = instance.SerialNumber;
            existing.ExpiryDate = instance.ExpiryDate;
            existing.Status = instance.Status;
            existing.Metadata = instance.Metadata;
            existing.UpdatedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        public Task Delete(int id)
        {
            _instances.RemoveAll(i => i.Id == id);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<ItemInstance>> GetByItemId(int itemId)
        {
            return Task.FromResult<IEnumerable<ItemInstance>>(_instances.Where(i => i.ItemId == itemId));
        }

        public Task<IEnumerable<ItemInstance>> GetByStatus(string status)
        {
            return Task.FromResult<IEnumerable<ItemInstance>>(_instances.Where(i => i.Status == status));
        }

        public Task<IEnumerable<ItemInstance>> GetExpiringSoon(DateTime before)
        {
            return Task.FromResult<IEnumerable<ItemInstance>>(_instances.Where(i =>
                i.ExpiryDate.HasValue &&
                i.ExpiryDate.Value <= before));
        }
    }
}