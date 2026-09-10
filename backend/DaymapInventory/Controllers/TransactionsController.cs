using DaymapInventory.Interfaces;
using DaymapInventory.Models;
using Microsoft.AspNetCore.Mvc;

namespace DaymapInventory.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IItemInstanceRepository _instanceRepository;

        public TransactionsController(
            ITransactionRepository transactionRepository,
            IItemRepository itemRepository,
            IItemInstanceRepository instanceRepository)
        {
            _transactionRepository = transactionRepository;
            _itemRepository = itemRepository;
            _instanceRepository = instanceRepository;
        }

        // GET: api/transactions
        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _transactionRepository.GetAll());

        // GET: api/transactions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var transaction = await _transactionRepository.GetById(id);
            return transaction == null ? NotFound() : Ok(transaction);
        }

        // GET: api/items/5/transactions
        [HttpGet("/api/items/{itemId}/transactions")]
        public async Task<IActionResult> GetByItemId(int itemId)
        {
            if (await _itemRepository.GetById(itemId) == null) return NotFound();
            return Ok(await _transactionRepository.GetByItemId(itemId));
        }

        // GET: api/iteminstances/5/transactions
        [HttpGet("/api/iteminstances/{instanceId}/transactions")]
        public async Task<IActionResult> GetByItemInstanceId(int instanceId)
        {
            if (await _instanceRepository.GetById(instanceId) == null) return NotFound();
            return Ok(await _transactionRepository.GetByItemInstanceId(instanceId));
        }

        // POST: api/transactions
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Transaction transaction)
        {
            if (transaction.QuantityChanged <= 0)
                return BadRequest("QuantityChanged must be greater than 0.");

            var isLoan = string.Equals(
                transaction.Type,
                TransactionType.Loan.ToString(),
                StringComparison.OrdinalIgnoreCase);
            var isReturn = string.Equals(
                transaction.Type,
                TransactionType.Return.ToString(),
                StringComparison.OrdinalIgnoreCase);

            if (!isLoan && !isReturn)
                return BadRequest("This endpoint currently supports only Loan and Return transactions.");

            var item = await _itemRepository.GetById(transaction.ItemId);
            if (item == null)
                return NotFound($"Item with id {transaction.ItemId} not found.");

            ItemInstance? instance = null;
            if (transaction.ItemInstanceId.HasValue)
            {
                instance = await _instanceRepository.GetById(transaction.ItemInstanceId.Value);
                if (instance == null)
                    return NotFound($"Item instance with id {transaction.ItemInstanceId.Value} not found.");

                if (instance.ItemId != transaction.ItemId)
                    return BadRequest("The item instance does not belong to the supplied item.");

                if (isLoan && string.Equals(
                    instance.Status,
                    InstanceStatus.Loaned.ToString(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest("The item instance is already loaned.");
                }
            }

            var updatedStockCount = isLoan
                ? item.StockCount - transaction.QuantityChanged
                : item.StockCount + transaction.QuantityChanged;

            if (updatedStockCount < 0)
                return BadRequest("The loan would reduce item stock below 0.");

            if (instance != null)
            {
                // SyncStockCount in the instance repo recalculates stock from available
                // instances after save, so no manual stock update needed here.
                instance.Status = isLoan
                    ? InstanceStatus.Loaned.ToString()
                    : InstanceStatus.Available.ToString();
                await _instanceRepository.Update(instance);
            }
            else
            {
                // No instance — manually adjust stock count.
                item.StockCount = updatedStockCount;
                await _itemRepository.Update(item);
            }

            transaction.Type = isLoan
                ? TransactionType.Loan.ToString()
                : TransactionType.Return.ToString();
            transaction.Status = isLoan
                ? TransactionStatus.Active.ToString()
                : TransactionStatus.Returned.ToString();

            await _transactionRepository.Add(transaction);

            return CreatedAtAction(nameof(GetById), new { id = transaction.Id }, transaction);
        }
    }
}
