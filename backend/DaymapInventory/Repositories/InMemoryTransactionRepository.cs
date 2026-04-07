using DaymapInventory.Interfaces;
using DaymapInventory.Models;

namespace DaymapInventory.Repositories
{
    public class InMemoryTransactionRepository : ITransactionRepository
    {
        private readonly List<Transaction> _transactions = new();
        private int _nextId = 1;

        public Transaction? GetById(int id)
        {
            return _transactions.FirstOrDefault(t => t.Id == id);
        }

        public IEnumerable<Transaction> GetAll()
        {
            return _transactions;
        }

        public void Add(Transaction transaction)
        {
            transaction.Id = _nextId++;
            transaction.CreatedAt = DateTime.UtcNow;
            _transactions.Add(transaction);
        }

        public void Update(Transaction transaction)
        {
            // Transactions are append-only by design.
            // Updates are not supported — create a new Adjustment instead.
            throw new InvalidOperationException(
                "Transactions are append-only. Create a new Adjustment transaction instead.");
        }

        public void Delete(int id)
        {
            // Transactions should not be deleted for audit trail integrity.
            throw new InvalidOperationException(
                "Transactions cannot be deleted. They form an immutable audit log.");
        }

        public IEnumerable<Transaction> GetByItemId(int itemId)
        {
            return _transactions.Where(t => t.ItemId == itemId);
        }

        public IEnumerable<Transaction> GetByType(string type)
        {
            return _transactions.Where(t =>
                t.Type.Equals(type, StringComparison.OrdinalIgnoreCase));
        }
    }
}
