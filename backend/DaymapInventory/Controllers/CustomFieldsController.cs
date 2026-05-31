// Controllers/CustomFieldsController.cs
using DaymapInventory.Interfaces;
using DaymapInventory.Models;
using Microsoft.AspNetCore.Mvc;

namespace DaymapInventory.Controllers
{
    [ApiController]
    [Route("api/")]
    public class CustomFieldsController : ControllerBase
    {
        private readonly ICustomFieldRepository _repo;
        private readonly IItemRepository _itemRepository;

        public CustomFieldsController(ICustomFieldRepository repo, IItemRepository itemRepository)
        {
            _repo = repo;
            _itemRepository = itemRepository;
        }

        // POST /api/items/{id}/customfields — define a new field for an item
        [HttpPost("items/{id}/customfields")]
        public async Task<IActionResult> Create(int id, [FromBody] CustomField field)
        {
            var item = await _itemRepository.GetById(id);
            if (item == null) return NotFound($"Item {id} not found.");

            field.ItemId = id;
            await _repo.Add(field);
            return CreatedAtAction(nameof(GetById), new { id = field.Id }, field);
        }

        // GET /api/customfields/{id} — get a custom field definition
        [HttpGet("customfields/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var field = await _repo.GetById(id);
            if (field == null) return NotFound();
            return Ok(field);
        }

        // PUT /api/customfields/{id} — update field name (cascades to all instances)
        [HttpPut("customfields/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CustomField field)
        {
            var existing = await _repo.GetById(id);
            if (existing == null) return NotFound();

            field.Id = id;
            await _repo.Update(field);
            return Ok(await _repo.GetById(id));
        }

        // DELETE /api/customfields/{id} — delete field and all its values
        [HttpDelete("customfields/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _repo.GetById(id);
            if (existing == null) return NotFound();

            await _repo.Delete(id);
            return NoContent();
        }
    }
}