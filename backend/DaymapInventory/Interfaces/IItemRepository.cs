using DaymapInventory.Models;

namespace DaymapInventory.Interfaces
{
    public interface IItemRepository : IRepository<Item>
    {
        IEnumerable<Item> GetByStatus(string status);
        IEnumerable<Item> GetLowStock();
    }
}
