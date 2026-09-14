using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DaymapInventory.Models;
using DaymapInventory.Repositories;

namespace DaymapInventory.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly SqlTransactionRepository _repository;
        private readonly ILogger<TransactionsController> _logger;

        public TransactionsController(
            SqlTransactionRepository repository, 
            ILogger<TransactionsController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var transactions = await _repository.GetAll();
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all transactions.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Transaction entity)
        {
            try
            {
                await _repository.Add(entity);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a transaction.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}