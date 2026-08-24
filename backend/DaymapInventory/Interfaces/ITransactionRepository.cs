using DaymapInventory.Models;

namespace DaymapInventory.Interfaces
{
    public interface ITransactionRepository
    {
        Task<TransactionResponseDto> CreateAsync(CreateTransactionDto dto);
        Task<IEnumerable<TransactionResponseDto>> GetAllAsync();
        Task<TransactionResponseDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<TransactionResponseDto>> GetByItemIdAsync(Guid itemId);
        Task<IEnumerable<TransactionResponseDto>> GetByInstanceIdAsync(Guid instanceId);
    }

    public class TransactionResponseDto
    {
    }

    public class CreateTransactionDto
    {
    }
}