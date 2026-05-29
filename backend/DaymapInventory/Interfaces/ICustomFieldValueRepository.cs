using DaymapInventory.Models;

namespace DaymapInventory.Interfaces
{
    public interface ICustomFieldValueRepository : IRepository<CustomFieldValue>
    {
        Task<CustomFieldValue?> GetById(int id);
        Task<IEnumerable<CustomFieldValue>> GetAll();
        Task Add(CustomFieldValue entity);
        Task Update(CustomFieldValue entity);
        Task Delete(int id);

        Task<IEnumerable<CustomFieldValue>> GetByCustomFieldId(int customFieldId);
        Task<IEnumerable<CustomFieldValue>> GetByItemId(int itemId);
        Task<IEnumerable<CustomFieldValue>> GetByItemInstanceId(int itemInstanceId);

        Task<IEnumerable<CustomFieldValue>> GetByItemIdWithFieldDetails(int itemId);
        Task<IEnumerable<CustomFieldValue>> GetByItemInstanceIdWithFieldDetails(int itemInstanceId);
        Task<CustomFieldValue?> GetByFieldAndItem(int customFieldId, int itemId);
        Task<CustomFieldValue?> GetByFieldAndItemInstance(int customFieldId, int itemInstanceId);
        Task<bool> UniqueValueExists(int customFieldId, string value, int itemId, int? excludeItemInstanceId = null);
        Task DeleteByCustomFieldId(int customFieldId);
    }
}
