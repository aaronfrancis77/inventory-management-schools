using DaymapInventory.Data;
using DaymapInventory.Interfaces;
using DaymapInventory.Models;
using Microsoft.EntityFrameworkCore;

namespace DaymapInventory.Repositories
{
    public class SqlItemRepository : IItemRepository
    {
        private readonly AppDbContext _context;

        public SqlItemRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Item?> GetById(int id) =>
            await _context.Items.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);

        public async Task<IEnumerable<Item>> GetAll() => await _context.Items.ToListAsync();

        public async Task Add(Item entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;
            await _context.Items.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Item entity)
        {
            var existing = await _context.Items.FindAsync(entity.Id);
            if (existing == null) return;
            _context.Entry(existing).CurrentValues.SetValues(entity);
            existing.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item != null)
            {
                _context.Items.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Item>> GetByStatus(string status) =>
            await _context.Items.Where(i => i.Status == status).ToListAsync();

        public async Task<IEnumerable<Item>> GetLowStock() =>
            await _context.Items.Where(i => i.StockCount <= i.LowStockThreshold).ToListAsync();
    }
}
