using DaymapInventory.Interfaces;
using DaymapInventory.Models;

namespace DaymapInventory.Repositories
{
    public class InMemoryTagRepository : ITagRepository
    {
        private readonly List<Tag> _tags = new();
        private int _nextId = 1;

        public Tag? GetById(int id)
        {
            return _tags.FirstOrDefault(t => t.Id == id);
        }

        public IEnumerable<Tag> GetAll()
        {
            return _tags;
        }

        public void Add(Tag tag)
        {
            tag.Id = _nextId++;
            tag.CreatedAt = DateTime.UtcNow;
            _tags.Add(tag);
        }

        public void Update(Tag tag)
        {
            var existing = GetById(tag.Id);
            if (existing == null) return;

            existing.Name = tag.Name;
            existing.IsDefault = tag.IsDefault;
        }

        public void Delete(int id)
        {
            _tags.RemoveAll(t => t.Id == id);
        }

        public Tag? GetByName(string name)
        {
            return _tags.FirstOrDefault(t =>
                t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<Tag> GetDefaults()
        {
            return _tags.Where(t => t.IsDefault);
        }
    }
}
