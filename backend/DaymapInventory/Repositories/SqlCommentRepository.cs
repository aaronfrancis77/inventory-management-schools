using DaymapInventory.Data;
using DaymapInventory.Interfaces;
using DaymapInventory.Models;
using Microsoft.EntityFrameworkCore;

namespace DaymapInventory.Repositories
{
    public class SqlCommentRepository : ICommentRepository
    {
        private readonly AppDbContext _context;

        public SqlCommentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Comment?> GetById(int id) =>
            await _context.Comments.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);

        public async Task<IEnumerable<Comment>> GetAll() =>
            await _context.Comments.AsNoTracking().ToListAsync();

        public async Task Add(Comment entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;
            await _context.Comments.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Comment entity)
        {
            var existing = await _context.Comments.FindAsync(entity.Id);
            if (existing == null) return;

            _context.Entry(existing).CurrentValues.SetValues(entity);
            existing.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Comment>> GetByItemId(int itemId) =>
            await _context.Comments.AsNoTracking()
                .Where(c => c.ItemId == itemId)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();
    }
}
