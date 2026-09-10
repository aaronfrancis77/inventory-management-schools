using DaymapInventory.Models;

namespace DaymapInventory.Interfaces
{
    public interface ITransactionRepository : IRepository<Transaction>
    {
        Task<IEnumerable<Transaction>> GetByItemId(int itemId);
        Task<IEnumerable<Transaction>> GetByItemInstanceId(int itemInstanceId);
        Task<IEnumerable<Transaction>> GetByType(string type);
    }
}
