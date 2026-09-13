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

        public async Task<Comment?> GetById(int id) => await _context.Comments.FindAsync(id);

        public async Task<IEnumerable<Comment>> GetAll() => await _context.Comments.ToListAsync();

        public async Task Add(Comment entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            await _context.Comments.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Comment entity)
        {
            _context.Comments.Update(entity);
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
            await _context.Comments
                .Where(c => c.ItemId == itemId)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();
    }
}