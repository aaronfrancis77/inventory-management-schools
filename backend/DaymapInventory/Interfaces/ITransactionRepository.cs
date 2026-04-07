using DaymapInventory.Models;

namespace DaymapInventory.Interfaces
{
    public interface ITransactionRepository : IRepository<Transaction>
    {
        IEnumerable<Transaction> GetByItemId(int itemId);
        IEnumerable<Transaction> GetByType(string type);
    }
}
