using DaymapInventory.Data;
using DaymapInventory.Interfaces;
using DaymapInventory.Models;
using Microsoft.EntityFrameworkCore;

namespace DaymapInventory.Repositories
{
    public class SqlItemInstanceRepository : IItemInstanceRepository
    {
        private readonly AppDbContext _context;

        public SqlItemInstanceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ItemInstance?> GetById(int id) => await _context.ItemInstances.FindAsync(id);

        public async Task<IEnumerable<ItemInstance>> GetAll() => await _context.ItemInstances.ToListAsync();

        public async Task Add(ItemInstance entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;
            await _context.ItemInstances.AddAsync(entity);
            await _context.SaveChangesAsync();
            await SyncStockCount(entity.ItemId);
        }

        public async Task Update(ItemInstance entity)
        {
            var existing = await _context.ItemInstances.FindAsync(entity.Id);
            if (existing == null) return;
            var oldItemId = existing.ItemId;
            _context.Entry(existing).CurrentValues.SetValues(entity);
            existing.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            await SyncStockCount(oldItemId);
            if (entity.ItemId != oldItemId) await SyncStockCount(entity.ItemId);
        }

        public async Task Delete(int id)
        {
            var instance = await _context.ItemInstances.FindAsync(id);
            if (instance != null)
            {
                _context.ItemInstances.Remove(instance);
                await _context.SaveChangesAsync();
                await SyncStockCount(instance.ItemId);
            }
        }

        public async Task<IEnumerable<ItemInstance>> GetByItemId(int itemId) =>
            await _context.ItemInstances.Where(ii => ii.ItemId == itemId).ToListAsync();

        public async Task<IEnumerable<ItemInstance>> GetByStatus(string status) =>
            await _context.ItemInstances.Where(ii => ii.Status == status).ToListAsync();

        public async Task<IEnumerable<ItemInstance>> GetExpiringSoon(DateTime before) =>
            await _context.ItemInstances
                .Where(ii => ii.ExpiryDate.HasValue && ii.ExpiryDate.Value <= before)
                .ToListAsync();

        private async Task SyncStockCount(int itemId)
        {
            var item = await _context.Items.FindAsync(itemId);
            if (item == null) return;
            item.StockCount = await _context.ItemInstances
                .CountAsync(ii => ii.ItemId == itemId && ii.Status == InstanceStatus.Available.ToString());
            item.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
