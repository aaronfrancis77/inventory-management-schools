using DaymapInventory.Data;
using DaymapInventory.Interfaces;
using DaymapInventory.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DaymapInventory.Repositories
{
    public class SqlTransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SqlTransactionRepository>? _logger;

        public SqlTransactionRepository(
            AppDbContext context, 
            ILogger<SqlTransactionRepository>? logger = null)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Transaction?> GetById(int id) => await _context.Transactions.FindAsync(id);

        public async Task<IEnumerable<Transaction>> GetAll() => await _context.Transactions.ToListAsync();

        public async Task Add(Transaction entity)
        {
            entity.CreatedAt = DateTime.UtcNow;
            await _context.Transactions.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public Task Update(Transaction entity)
        {
            var exception = new InvalidOperationException("Transactions are append-only. Create a new Adjustment transaction instead.");
            _logger?.LogError(exception, "Attempted to update transaction ID {Id}, but transactions are append-only.", entity.Id);
            throw exception;
        }

        public Task Delete(int id)
        {
            var exception = new InvalidOperationException("Transactions cannot be deleted. They form an immutable audit log.");
            _logger?.LogError(exception, "Attempted to delete transaction ID {Id}, but transactions are immutable.", id);
            throw exception;
        }

        public async Task<IEnumerable<Transaction>> GetByItemId(int itemId) =>
            await _context.Transactions.Where(t => t.ItemId == itemId).ToListAsync();
            
        public async Task<IEnumerable<Transaction>> GetByItemInstanceId(int itemInstanceId) =>
            await _context.Transactions.Where(t => t.ItemInstanceId == itemInstanceId).ToListAsync();

        public async Task<IEnumerable<Transaction>> GetByType(string type) =>
            await _context.Transactions.Where(t => t.Type == type).ToListAsync();
    }
}