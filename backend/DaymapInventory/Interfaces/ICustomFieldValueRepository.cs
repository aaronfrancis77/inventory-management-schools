using DaymapInventory.Models;

namespace DaymapInventory.Interfaces
{
    public interface ICustomFieldValueRepository : IRepository<CustomFieldValue>
    {
        Task<IEnumerable<CustomFieldValue>> GetByCustomFieldId(int customFieldId);
        Task<IEnumerable<CustomFieldValue>> GetByItemId(int itemId);
        Task<IEnumerable<CustomFieldValue>> GetByItemInstanceId(int itemInstanceId);
    }
}
