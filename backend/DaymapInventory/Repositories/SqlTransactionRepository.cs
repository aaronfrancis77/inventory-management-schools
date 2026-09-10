using DaymapInventory.Data;
using DaymapInventory.Interfaces;
using DaymapInventory.Models;
using Microsoft.EntityFrameworkCore;

namespace DaymapInventory.Repositories
{
    public class SqlTransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _context;

        public SqlTransactionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Transaction?> GetById(int id) => await _context.Transactions.FindAsync(id);

        public async Task<IEnumerable<Transaction>> GetAll() => await _context.Transactions.ToListAsync();

        public async Task Add(Transaction entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            await _context.Transactions.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public Task Update(Transaction entity) =>
            throw new InvalidOperationException("Transactions are append-only. Create a new Adjustment transaction instead.");

        public Task Delete(int id) =>
            throw new InvalidOperationException("Transactions cannot be deleted. They form an immutable audit log.");

        public async Task<IEnumerable<Transaction>> GetByItemId(int itemId) =>
            await _context.Transactions.Where(t => t.ItemId == itemId).ToListAsync();

        public async Task<IEnumerable<Transaction>> GetByType(string type) =>
            await _context.Transactions.Where(t => t.Type == type).ToListAsync();
    }
}
