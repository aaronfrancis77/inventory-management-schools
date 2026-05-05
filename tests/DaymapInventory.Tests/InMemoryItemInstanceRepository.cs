using DaymapInventory.Interfaces;
using DaymapInventory.Models;

namespace DaymapInventory.Repositories
{
    public class InMemoryItemInstanceRepository : IItemInstanceRepository
    {
        private readonly List<ItemInstance> _itemInstances = new();
        private int _nextId = 1;

        public void Add(ItemInstance instance)
        {
            instance.Id = _nextId++;
            _itemInstances.Add(instance);
        }

        public IEnumerable<ItemInstance> GetAll()
        {
            return _itemInstances;
        }

        public ItemInstance? GetById(int id)
        {
            return _itemInstances.FirstOrDefault(i => i.Id == id);
        }

        public IEnumerable<ItemInstance> GetByItemId(int itemId)
        {
            return _itemInstances.Where(i => i.ItemId == itemId);
        }

        public void Delete(int id)
        {
            var instance = GetById(id);

            if (instance != null)
            {
                _itemInstances.Remove(instance);
            }
        }
    }
}