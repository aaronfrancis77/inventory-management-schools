using DaymapInventory.Models;

namespace DaymapInventory.Interfaces
{
    public interface IItemInstanceRepository : IRepository<ItemInstance>
    {
        IEnumerable<ItemInstance> GetByItemId(int itemId);
        IEnumerable<ItemInstance> GetByStatus(string status);
        IEnumerable<ItemInstance> GetExpiringSoon(DateTime before);
    }
}
