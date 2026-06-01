using DaymapInventory.Models;

namespace DaymapInventory.Interfaces
{
    public interface ICustomFieldRepository : IRepository<CustomField>
    {
        Task<IEnumerable<CustomField>> GetByItemId(int itemId);
    }
}
