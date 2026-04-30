using DaymapInventory.Models;

namespace DaymapInventory.Interfaces
{
    public interface ITagRepository : IRepository<Tag>
    {
        Task<Tag?> GetByName(string name);
        Task<IEnumerable<Tag>> GetDefaults();
    }
}