using DaymapInventory.Data;
using DaymapInventory.Interfaces;
using DaymapInventory.Models;

namespace DaymapInventory.Repositories
{
    public class SqlTransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _context;

        public SqlTransactionRepository(AppDbContext context)
        {
            _context = context;
        }

        public Transaction? GetById(int id)
        {
            return _context.Transactions.Find(id);
        }

        public IEnumerable<Transaction> GetAll()
        {
            return _context.Transactions.ToList();
        }

        public void Add(Transaction entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            _context.Transactions.Add(entity);
            _context.SaveChanges();
        }

        public void Update(Transaction entity)
        {
            _context.Transactions.Update(entity);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var transaction = _context.Transactions.Find(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
                _context.SaveChanges();
            }
        }

        public IEnumerable<Transaction> GetByItemId(int itemId)
        {
            return _context.Transactions.Where(t => t.ItemId == itemId).ToList();
        }

        public IEnumerable<Transaction> GetByType(string type)
        {
            return _context.Transactions.Where(t => t.Type == type).ToList();
        }
    }
}
