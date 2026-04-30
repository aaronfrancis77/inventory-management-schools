using DaymapInventory.Models;

namespace DaymapInventory.Interfaces
{
    public interface IItemInstanceRepository : IRepository<ItemInstance>
    {
        Task<IEnumerable<ItemInstance>> GetByItemId(int itemId);
        Task<IEnumerable<ItemInstance>> GetByStatus(string status);
        Task<IEnumerable<ItemInstance>> GetExpiringSoon(DateTime before);
    }
}