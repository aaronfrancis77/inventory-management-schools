using DaymapInventory.Interfaces;
using DaymapInventory.Models;

namespace DaymapInventory.Repositories
{
    public class InMemoryTransactionRepository : ITransactionRepository
    {
        private readonly List<Transaction> _transactions = new();
        private int _nextId = 1;

        public Task<Transaction?> GetById(int id)
        {
            return Task.FromResult(_transactions.FirstOrDefault(t => t.Id == id));
        }

        public Task<IEnumerable<Transaction>> GetAll()
        {
            return Task.FromResult<IEnumerable<Transaction>>(_transactions);
        }

        public Task Add(Transaction transaction)
        {
            transaction.Id = _nextId++;
            transaction.CreatedAt = DateTime.UtcNow;
            _transactions.Add(transaction);
            return Task.CompletedTask;
        }

        public Task Update(Transaction transaction)
        {
            throw new InvalidOperationException(
                "Transactions are append-only. Create a new Adjustment transaction instead.");
        }

        public Task Delete(int id)
        {
            throw new InvalidOperationException(
                "Transactions cannot be deleted. They form an immutable audit log.");
        }

        public Task<IEnumerable<Transaction>> GetByItemId(int itemId)
        {
            return Task.FromResult<IEnumerable<Transaction>>(_transactions.Where(t => t.ItemId == itemId));
        }

        public Task<IEnumerable<Transaction>> GetByType(string type)
        {
            return Task.FromResult<IEnumerable<Transaction>>(_transactions.Where(t =>
                t.Type.Equals(type, StringComparison.OrdinalIgnoreCase)));
        }
    }
}