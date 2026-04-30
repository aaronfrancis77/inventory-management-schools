using DaymapInventory.Data;
using DaymapInventory.Interfaces;
using DaymapInventory.Models;

namespace DaymapInventory.Repositories
{
    public class SqlCategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public SqlCategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public Category? GetById(int id)
        {
            return _context.Categories.Find(id);
        }

        public IEnumerable<Category> GetAll()
        {
            return _context.Categories.ToList();
        }

        public void Add(Category entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            _context.Categories.Add(entity);
            _context.SaveChanges();
        }

        public void Update(Category entity)
        {
            _context.Categories.Update(entity);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var category = _context.Categories.Find(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                _context.SaveChanges();
            }
        }

        public Category? GetByName(string name)
        {
            return _context.Categories.FirstOrDefault(c => c.Name == name);
        }
    }
}
