using DaymapInventory.Data;
using DaymapInventory.Interfaces;
using DaymapInventory.Models;

namespace DaymapInventory.Repositories
{
    public class SqlItemInstanceRepository : IItemInstanceRepository
    {
        private readonly AppDbContext _context;

        public SqlItemInstanceRepository(AppDbContext context)
        {
            _context = context;
        }

        public ItemInstance? GetById(int id)
        {
            return _context.ItemInstances.Find(id);
        }

        public IEnumerable<ItemInstance> GetAll()
        {
            return _context.ItemInstances.ToList();
        }

        public void Add(ItemInstance entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;
            _context.ItemInstances.Add(entity);
            _context.SaveChanges();
        }

        public void Update(ItemInstance entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            _context.ItemInstances.Update(entity);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var instance = _context.ItemInstances.Find(id);
            if (instance != null)
            {
                _context.ItemInstances.Remove(instance);
                _context.SaveChanges();
            }
        }

        public IEnumerable<ItemInstance> GetByItemId(int itemId)
        {
            return _context.ItemInstances.Where(ii => ii.ItemId == itemId).ToList();
        }

        public IEnumerable<ItemInstance> GetByStatus(string status)
        {
            return _context.ItemInstances.Where(ii => ii.Status == status).ToList();
        }

        public IEnumerable<ItemInstance> GetExpiringSoon(DateTime before)
        {
            return _context.ItemInstances
                .Where(ii => ii.ExpiryDate.HasValue && ii.ExpiryDate.Value <= before)
                .ToList();
        }
    }
}
