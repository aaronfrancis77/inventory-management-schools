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
        private readonly ITagRepository _tagRepository;

        public ItemsController(
            IItemRepository repository,
            ICustomFieldValueRepository customFieldValueRepository,
            ITagRepository tagRepository)
        {
            _repository = repository;
            _customFieldValueRepository = customFieldValueRepository;
            _tagRepository = tagRepository;
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

            var customFieldValues = (await _customFieldValueRepository.GetByItemIdWithFieldDetails(id))
                .Select(cfv => new
                {
                    cfv.Id,
                    cfv.CustomFieldId,
                    FieldName = cfv.CustomField?.Name,
                    ControlType = cfv.CustomField?.ControlType,
                    DataType = cfv.CustomField?.DataType,
                    cfv.ItemId,
                    cfv.ItemInstanceId,
                    cfv.Value,
                    cfv.CreatedAt,
                    cfv.UpdatedAt
                });

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

        // GET: api/items/5/tags
        [HttpGet("{id}/tags")]
        public async Task<IActionResult> GetTags(int id)
        {
            if (await _repository.GetById(id) == null) return NotFound();

            var tags = (await _tagRepository.GetTagsForItem(id))
                .Select(t => new { t.Id, t.Name, t.Colour, t.IsDefault });

            return Ok(tags);
        }

        // POST: api/items/5/tags/3
        [HttpPost("{id}/tags/{tagId}")]
        public async Task<IActionResult> AssignTag(int id, int tagId)
        {
            if (await _repository.GetById(id) == null) return NotFound();
            if (await _tagRepository.GetById(tagId) == null) return NotFound();

            if (await _tagRepository.IsAssignedToItem(id, tagId))
                return Conflict($"Tag {tagId} is already assigned to item {id}.");

            await _tagRepository.AssignToItem(id, tagId);
            return NoContent();
        }

        // DELETE: api/items/5/tags/3
        [HttpDelete("{id}/tags/{tagId}")]
        public async Task<IActionResult> RemoveTag(int id, int tagId)
        {
            if (await _repository.GetById(id) == null) return NotFound();
            if (await _tagRepository.GetById(tagId) == null) return NotFound();

            await _tagRepository.RemoveFromItem(id, tagId);
            return NoContent();
        }
    }
}