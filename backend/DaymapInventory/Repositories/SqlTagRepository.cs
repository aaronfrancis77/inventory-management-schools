using DaymapInventory.Data;
using DaymapInventory.Interfaces;
using DaymapInventory.Models;
using Microsoft.EntityFrameworkCore;

namespace DaymapInventory.Repositories
{
    public class SqlTagRepository : ITagRepository
    {
        private readonly AppDbContext _context;

        public SqlTagRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Tag?> GetById(int id) => await _context.Tags.FindAsync(id);

        public async Task<IEnumerable<Tag>> GetAll() => await _context.Tags.ToListAsync();

        public async Task Add(Tag entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            await _context.Tags.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Tag entity)
        {
            _context.Tags.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag != null)
            {
                _context.Tags.Remove(tag);
                await _context.SaveChangesAsync();
            }
        }

<<<<<<< Updated upstream
        public async Task<Tag?> GetByName(string name) =>
            await _context.Tags.FirstOrDefaultAsync(t => t.Name == name);
=======
        public Tag? GetByName(string name)
        {
            var normalizedName = name.ToLower();
            return _context.Tags.FirstOrDefault(t => t.Name.ToLower() == normalizedName);
        }
>>>>>>> Stashed changes

        public async Task<IEnumerable<Tag>> GetDefaults() =>
            await _context.Tags.Where(t => t.IsDefault).ToListAsync();
    }
}
