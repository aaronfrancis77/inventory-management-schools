using DaymapInventory.Interfaces;
using DaymapInventory.Models;

namespace DaymapInventory.Repositories
{
    public class InMemoryCategoryRepository : ICategoryRepository
    {
        private readonly List<Category> _categories = new();
        private int _nextId = 1;

        public Category? GetById(int id)
        {
            return _categories.FirstOrDefault(c => c.Id == id);
        }

        public IEnumerable<Category> GetAll()
        {
            return _categories;
        }

        public void Add(Category category)
        {
            category.Id = _nextId++;
            category.CreatedAt = DateTime.UtcNow;
            _categories.Add(category);
        }

        public void Update(Category category)
        {
            var existing = GetById(category.Id);
            if (existing == null) return;

            existing.Name = category.Name;
            existing.Description = category.Description;
        }

        public void Delete(int id)
        {
            _categories.RemoveAll(c => c.Id == id);
        }

        public Category? GetByName(string name)
        {
            return _categories.FirstOrDefault(c =>
                c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
