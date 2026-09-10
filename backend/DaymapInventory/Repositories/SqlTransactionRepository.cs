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

        public async Task<TransactionResponseDto> CreateAsync(CreateTransactionDto dto)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TransactionResponseDto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<TransactionResponseDto?> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TransactionResponseDto>> GetByItemIdAsync(Guid itemId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TransactionResponseDto>> GetByInstanceIdAsync(Guid instanceId)
        {
            throw new NotImplementedException();
        }
    }
}