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

        public async Task<CustomFieldValue?> GetById(int id)
        {
            return await _context.CustomFieldValues
                .Include(cfv => cfv.CustomField)
                .FirstOrDefaultAsync(cfv => cfv.Id == id);
        }

        public async Task<IEnumerable<CustomFieldValue>> GetAll()
        {
            return await _context.CustomFieldValues
                .Include(cfv => cfv.CustomField)
                .ToListAsync();
        }

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

            if (existing == null)
            {
                return;
            }

            existing.CustomFieldId = entity.CustomFieldId;
            existing.ItemId = entity.ItemId;
            existing.ItemInstanceId = entity.ItemInstanceId;
            existing.Value = entity.Value;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var value = await _context.CustomFieldValues.FindAsync(id);

            if (value == null)
            {
                return;
            }

            _context.CustomFieldValues.Remove(value);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<CustomFieldValue>> GetByCustomFieldId(int customFieldId)
        {
            return await _context.CustomFieldValues
                .Where(cfv => cfv.CustomFieldId == customFieldId)
                .Include(cfv => cfv.CustomField)
                .ToListAsync();
        }

        public async Task<IEnumerable<CustomFieldValue>> GetByItemId(int itemId)
        {
            return await _context.CustomFieldValues
                .Where(cfv => cfv.ItemId == itemId)
                .ToListAsync();
        }

        public async Task<IEnumerable<CustomFieldValue>> GetByItemInstanceId(int itemInstanceId)
        {
            return await _context.CustomFieldValues
                .Where(cfv => cfv.ItemInstanceId == itemInstanceId)
                .ToListAsync();
        }

        public async Task<IEnumerable<CustomFieldValue>> GetByItemIdWithFieldDetails(int itemId)
        {
            return await _context.CustomFieldValues
                .Where(cfv => cfv.ItemId == itemId)
                .Include(cfv => cfv.CustomField)
                .ToListAsync();
        }

        public async Task<IEnumerable<CustomFieldValue>> GetByItemInstanceIdWithFieldDetails(int itemInstanceId)
        {
            return await _context.CustomFieldValues
                .Where(cfv => cfv.ItemInstanceId == itemInstanceId)
                .Include(cfv => cfv.CustomField)
                .ToListAsync();
        }

        public async Task<CustomFieldValue?> GetByFieldAndItem(int customFieldId, int itemId)
        {
            return await _context.CustomFieldValues
                .Include(cfv => cfv.CustomField)
                .FirstOrDefaultAsync(cfv =>
                    cfv.CustomFieldId == customFieldId &&
                    cfv.ItemId == itemId);
        }

        public async Task<CustomFieldValue?> GetByFieldAndItemInstance(int customFieldId, int itemInstanceId)
        {
            return await _context.CustomFieldValues
                .Include(cfv => cfv.CustomField)
                .FirstOrDefaultAsync(cfv =>
                    cfv.CustomFieldId == customFieldId &&
                    cfv.ItemInstanceId == itemInstanceId);
        }

        public async Task<bool> UniqueValueExists(int customFieldId, string value, int itemId, int? excludeItemInstanceId = null)
        {
            var query = _context.CustomFieldValues
                .Include(cfv => cfv.CustomField)
                .Where(cfv =>
                    cfv.CustomFieldId == customFieldId &&
                    cfv.Value.ToLower() == value.ToLower() &&
                    cfv.ItemInstanceId != null &&
                    cfv.CustomField != null &&
                    cfv.CustomField.ItemId == itemId);

            if (excludeItemInstanceId.HasValue)
            {
                query = query.Where(cfv => cfv.ItemInstanceId != excludeItemInstanceId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task DeleteByCustomFieldId(int customFieldId)
        {
            var values = await _context.CustomFieldValues
                .Where(cfv => cfv.CustomFieldId == customFieldId)
                .ToListAsync();

            if (!values.Any())
            {
                return;
            }

            _context.CustomFieldValues.RemoveRange(values);
            await _context.SaveChangesAsync();
        }
    }
}
