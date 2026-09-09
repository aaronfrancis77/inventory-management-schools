using DaymapInventory.Models;

namespace DaymapInventory.Interfaces
{
    public interface IItemRepository : IRepository<Item>
    {
        Task<IEnumerable<Item>> Search(string query, int? categoryId);
        Task<IEnumerable<Item>> GetByStatus(string status);
        Task<IEnumerable<Item>> GetLowStock();
    }
}
