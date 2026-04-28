using DaymapInventory.Data;
using DaymapInventory.Interfaces;
using DaymapInventory.Models;

namespace DaymapInventory.Repositories
{
    public class SqlTagRepository : ITagRepository
    {
        private readonly AppDbContext _context;

        public SqlTagRepository(AppDbContext context)
        {
            _context = context;
        }

        public Tag? GetById(int id)
        {
            return _context.Tags.Find(id);
        }

        public IEnumerable<Tag> GetAll()
        {
            return _context.Tags.ToList();
        }

        public void Add(Tag entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            _context.Tags.Add(entity);
            _context.SaveChanges();
        }

        public void Update(Tag entity)
        {
            _context.Tags.Update(entity);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var tag = _context.Tags.Find(id);
            if (tag != null)
            {
                _context.Tags.Remove(tag);
                _context.SaveChanges();
            }
        }

        public Tag? GetByName(string name)
        {
            return _context.Tags.FirstOrDefault(t => t.Name == name);
        }

        public IEnumerable<Tag> GetDefaults()
        {
            return _context.Tags.Where(t => t.IsDefault).ToList();
        }
    }
}
