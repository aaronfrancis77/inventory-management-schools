using DaymapInventory.Interfaces;
using DaymapInventory.Models;
using Microsoft.AspNetCore.Mvc;

namespace DaymapInventory.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemRepository _repository;
        private readonly ICustomFieldValueRepository _customFieldValueRepository;

        public ItemsController(
            IItemRepository repository,
            ICustomFieldValueRepository customFieldValueRepository)
        {
            _repository = repository;
            _customFieldValueRepository = customFieldValueRepository;
        }

        // GET: api/items
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _repository.GetAll();
            return Ok(items);
        }

        // GET: api/items/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _repository.GetById(id);
            if (item == null) return NotFound();

            var customFieldValues = await _customFieldValueRepository.GetByItemIdWithFieldDetails(id);

            return Ok(new
            {
                item.Id,
                item.Name,
                item.Description,
                item.CreatedAt,
                item.UpdatedAt,
                CustomFieldValues = customFieldValues
            });
        }

        // POST: api/items
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Item item)
        {
            await _repository.Add(item);
            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }

        // PUT: api/items/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Item item)
        {
            if (await _repository.GetById(id) == null) return NotFound();

            item.Id = id;
            await _repository.Update(item);

            var updated = await _repository.GetById(id);
            return Ok(updated);
        }

        // DELETE: api/items/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await _repository.GetById(id) == null) return NotFound();

            await _repository.Delete(id);
            return NoContent();
        }
    }
}