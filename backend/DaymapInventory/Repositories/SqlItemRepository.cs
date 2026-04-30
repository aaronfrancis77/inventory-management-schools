using DaymapInventory.Data;
using DaymapInventory.Interfaces;
using DaymapInventory.Models;

namespace DaymapInventory.Repositories
{
    public class SqlItemRepository : IItemRepository
    {
        private readonly AppDbContext _context;

        public SqlItemRepository(AppDbContext context)
        {
            _context = context;
        }

        public Item? GetById(int id)
        {
            return _context.Items.Find(id);
        }

        public IEnumerable<Item> GetAll()
        {
            return _context.Items.ToList();
        }

        public void Add(Item entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;
            _context.Items.Add(entity);
            _context.SaveChanges();
        }

        public void Update(Item entity)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            _context.Items.Update(entity);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var item = _context.Items.Find(id);
            if (item != null)
            {
                _context.Items.Remove(item);
                _context.SaveChanges();
            }
        }

        public IEnumerable<Item> GetByStatus(string status)
        {
            return _context.Items.Where(i => i.Status == status).ToList();
        }

        public IEnumerable<Item> GetLowStock()
        {
            return _context.Items.Where(i => i.StockCount <= i.LowStockThreshold).ToList();
        }
    }
}
