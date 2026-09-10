using System;
using System.Threading.Tasks;
using DaymapInventory.Interfaces;
using DaymapInventory.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DaymapInventory.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IItemInstanceRepository _itemInstanceRepository;
        private readonly ILogger<TransactionsController>? _logger;

        public TransactionsController(
            ITransactionRepository transactionRepository,
            IItemRepository itemRepository,
            IItemInstanceRepository itemInstanceRepository,
            ILogger<TransactionsController>? logger = null)
        {
            _transactionRepository = transactionRepository;
            _itemRepository = itemRepository;
            _itemInstanceRepository = itemInstanceRepository;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Transaction transaction)
        {
            var item = await _itemRepository.GetById(transaction.ItemId);
            if (item == null)
            {
                return BadRequest("Item not found.");
            }

            if (transaction.Type == "Loan")
            {
                if (item.StockCount < transaction.QuantityChanged)
                {
                    return BadRequest("Insufficient stock.");
                }

                if (transaction.ItemInstanceId.HasValue)
                {
                    var instance = await _itemInstanceRepository.GetById(transaction.ItemInstanceId.Value);
                    if (instance != null && instance.Status == "Loaned")
                    {
                        return BadRequest("Item instance is already loaned.");
                    }

                    if (instance != null)
                    {
                        instance.Status = "Loaned";
                        await _itemInstanceRepository.Update(instance);
                    }
                }

                item.StockCount -= transaction.QuantityChanged;
                transaction.Status = "Active";
            }
            else if (transaction.Type == "Return")
            {
                if (transaction.ItemInstanceId.HasValue)
                {
                    var instance = await _itemInstanceRepository.GetById(transaction.ItemInstanceId.Value);
                    if (instance != null)
                    {
                        instance.Status = "Available";
                        await _itemInstanceRepository.Update(instance);
                    }
                }

                item.StockCount += transaction.QuantityChanged;
                transaction.Status = "Returned";
            }

            await _itemRepository.Update(item);

            await _transactionRepository.CreateAsync(new CreateTransactionDto());

            return CreatedAtAction(nameof(Create), new { id = transaction.Id }, transaction);
        }
    }
}