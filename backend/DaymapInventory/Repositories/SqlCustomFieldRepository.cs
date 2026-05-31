// Repositories/SqlCustomFieldRepository.cs
using DaymapInventory.Data;
using DaymapInventory.Interfaces;
using DaymapInventory.Models;
using Microsoft.EntityFrameworkCore;

namespace DaymapInventory.Repositories
{
    public class SqlCustomFieldRepository : ICustomFieldRepository
    {
        private readonly AppDbContext _context;

        public SqlCustomFieldRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CustomField?> GetById(int id)
        {
            return await _context.CustomFields
                .FirstOrDefaultAsync(cf => cf.Id == id);
        }

        public async Task<IEnumerable<CustomField>> GetAll()
        {
            return await _context.CustomFields.ToListAsync();
        }

        public async Task<IEnumerable<CustomField>> GetByItemId(int itemId)
        {
            return await _context.CustomFields
                .Where(cf => cf.ItemId == itemId)
                .ToListAsync();
        }

        public async Task Add(CustomField entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            await _context.CustomFields.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Update(CustomField entity)
        {
            var existing = await _context.CustomFields.FindAsync(entity.Id);
            if (existing == null) return;

            // Cascade: updating Name here cascades to all values via the FK relationship
            existing.Name = entity.Name;
            existing.ControlType = entity.ControlType;
            existing.DataType = entity.DataType;
            existing.IsRequired = entity.IsRequired;
            existing.IsUnique = entity.IsUnique;
            existing.DefaultValue = entity.DefaultValue;

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var field = await _context.CustomFields.FindAsync(id);
            if (field == null) return;

            // EF cascade delete handles removing all CustomFieldValues too
            // (configured in AppDbContext with OnDelete(DeleteBehavior.Cascade))
            _context.CustomFields.Remove(field);
            await _context.SaveChangesAsync();
        }
    }
}