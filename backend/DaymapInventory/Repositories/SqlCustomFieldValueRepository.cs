using DaymapInventory.Data;
using DaymapInventory.Interfaces;
using DaymapInventory.Models;
using Microsoft.EntityFrameworkCore;

namespace DaymapInventory.Repositories
{
    public class SqlCustomFieldValueRepository : ICustomFieldValueRepository
    {
        private readonly AppDbContext _context;

        public SqlCustomFieldValueRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CustomFieldValue?> GetById(int id) => await _context.CustomFieldValues.FindAsync(id);

        public async Task<IEnumerable<CustomFieldValue>> GetAll() => await _context.CustomFieldValues.ToListAsync();

        public async Task Add(CustomFieldValue entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;
            await _context.CustomFieldValues.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Update(CustomFieldValue entity)
        {
            var existing = await _context.CustomFieldValues.FindAsync(entity.Id);
            if (existing == null) return;
            _context.Entry(existing).CurrentValues.SetValues(entity);
            existing.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var v = await _context.CustomFieldValues.FindAsync(id);
            if (v != null)
            {
                _context.CustomFieldValues.Remove(v);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<CustomFieldValue>> GetByCustomFieldId(int customFieldId) =>
            await _context.CustomFieldValues.Where(cfv => cfv.CustomFieldId == customFieldId).ToListAsync();

        public async Task<IEnumerable<CustomFieldValue>> GetByItemId(int itemId) =>
            await _context.CustomFieldValues.Where(cfv => cfv.ItemId == itemId).ToListAsync();

        public async Task<IEnumerable<CustomFieldValue>> GetByItemInstanceId(int itemInstanceId) =>
            await _context.CustomFieldValues.Where(cfv => cfv.ItemInstanceId == itemInstanceId).ToListAsync();
    }
}
