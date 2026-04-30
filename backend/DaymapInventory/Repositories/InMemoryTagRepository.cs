using DaymapInventory.Interfaces;
using DaymapInventory.Models;

namespace DaymapInventory.Repositories
{
    public class InMemoryTagRepository : ITagRepository
    {
        private readonly List<Tag> _tags = new();
        private int _nextId = 1;

        public Task<Tag?> GetById(int id)
        {
            return Task.FromResult(_tags.FirstOrDefault(t => t.Id == id));
        }

        public Task<IEnumerable<Tag>> GetAll()
        {
            return Task.FromResult<IEnumerable<Tag>>(_tags);
        }

        public Task Add(Tag tag)
        {
            tag.Id = _nextId++;
            tag.CreatedAt = DateTime.UtcNow;
            _tags.Add(tag);
            return Task.CompletedTask;
        }

        public Task Update(Tag tag)
        {
            var existing = _tags.FirstOrDefault(t => t.Id == tag.Id);
            if (existing == null) return Task.CompletedTask;

            existing.Name = tag.Name;
            existing.IsDefault = tag.IsDefault;
            return Task.CompletedTask;
        }

        public Task Delete(int id)
        {
            _tags.RemoveAll(t => t.Id == id);
            return Task.CompletedTask;
        }

        public Task<Tag?> GetByName(string name)
        {
            return Task.FromResult(_tags.FirstOrDefault(t =>
                t.Name.Equals(name, StringComparison.OrdinalIgnoreCase)));
        }

        public Task<IEnumerable<Tag>> GetDefaults()
        {
            return Task.FromResult<IEnumerable<Tag>>(_tags.Where(t => t.IsDefault));
        }
    }
}