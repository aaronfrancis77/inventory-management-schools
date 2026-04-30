using DaymapInventory.Data;
using DaymapInventory.Interfaces;
using DaymapInventory.Models;
using Microsoft.EntityFrameworkCore;

namespace DaymapInventory.Repositories
{
    public class SqlCategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public SqlCategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Category?> GetById(int id) => await _context.Categories.FindAsync(id);

        public async Task<IEnumerable<Category>> GetAll() => await _context.Categories.ToListAsync();

        public async Task Add(Category entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            await _context.Categories.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Category entity)
        {
            _context.Categories.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Category?> GetByName(string name) =>
            await _context.Categories.FirstOrDefaultAsync(c => c.Name == name);
    }
}
