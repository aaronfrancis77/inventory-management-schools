using DaymapInventory.Interfaces;
using DaymapInventory.Models;

namespace DaymapInventory.Repositories
{
    public class InMemoryCategoryRepository : ICategoryRepository
    {
        private readonly List<Category> _categories = new();
        private int _nextId = 1;

        public Task<Category?> GetById(int id)
        {
            return Task.FromResult(_categories.FirstOrDefault(c => c.Id == id));
        }

        public Task<IEnumerable<Category>> GetAll()
        {
            return Task.FromResult<IEnumerable<Category>>(_categories);
        }

        public Task Add(Category category)
        {
            category.Id = _nextId++;
            category.CreatedAt = DateTime.UtcNow;
            _categories.Add(category);
            return Task.CompletedTask;
        }

        public Task Update(Category category)
        {
            var existing = _categories.FirstOrDefault(c => c.Id == category.Id);
            if (existing == null) return Task.CompletedTask;

            existing.Name = category.Name;
            existing.Description = category.Description;
            return Task.CompletedTask;
        }

        public Task Delete(int id)
        {
            _categories.RemoveAll(c => c.Id == id);
            return Task.CompletedTask;
        }

        public Task<Category?> GetByName(string name)
        {
            return Task.FromResult(_categories.FirstOrDefault(c =>
                c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)));
        }
    }
}
