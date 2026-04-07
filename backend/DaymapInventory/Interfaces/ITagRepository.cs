using DaymapInventory.Models;

namespace DaymapInventory.Interfaces
{
    public interface ITagRepository : IRepository<Tag>
    {
        Tag? GetByName(string name);
        IEnumerable<Tag> GetDefaults();
    }
}
