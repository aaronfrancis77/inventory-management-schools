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

        public async Task<Tag?> GetByName(string name) =>
            await _context.Tags.FirstOrDefaultAsync(t => t.Name == name);

        public async Task<IEnumerable<Tag>> GetDefaults() =>
            await _context.Tags.Where(t => t.IsDefault).ToListAsync();

        // Item tag assignment (SCRUM-93)

        public async Task<bool> IsAssignedToItem(int itemId, int tagId) =>
            await _context.ItemTags.AnyAsync(it => it.ItemId == itemId && it.TagId == tagId);

        public async Task AssignToItem(int itemId, int tagId)
        {
            _context.ItemTags.Add(new ItemTag { ItemId = itemId, TagId = tagId });
            await _context.SaveChangesAsync();
        }

        public async Task RemoveFromItem(int itemId, int tagId)
        {
            var link = await _context.ItemTags
                .FirstOrDefaultAsync(it => it.ItemId == itemId && it.TagId == tagId);

            if (link != null)
            {
                _context.ItemTags.Remove(link);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Tag>> GetTagsForItem(int itemId) =>
            await _context.ItemTags
                .Where(it => it.ItemId == itemId)
                .Select(it => it.Tag!)
                .ToListAsync();
    }
}
