using DaymapInventory.Interfaces;
using DaymapInventory.Models;
using Microsoft.AspNetCore.Mvc;

namespace DaymapInventory.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemInstancesController : ControllerBase
    {
        private readonly IItemInstanceRepository _instanceRepository;
        private readonly IItemRepository _itemRepository;
        private readonly ICustomFieldValueRepository _customFieldValueRepository;

        public ItemInstancesController(
            IItemInstanceRepository instanceRepository,
            IItemRepository itemRepository,
            ICustomFieldValueRepository customFieldValueRepository)
        {
            _instanceRepository = instanceRepository;
            _itemRepository = itemRepository;
            _customFieldValueRepository = customFieldValueRepository;
        }

        // POST: api/iteminstances
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ItemInstance instance)
        {
            var item = await _itemRepository.GetById(instance.ItemId);
            if (item == null) return NotFound($"Item with id {instance.ItemId} not found.");
            // Uniqueness check: if a serial number is provided, ensure no other instance
            // for the same item has the same serial number.
            if (!string.IsNullOrWhiteSpace(instance.SerialNumber))
            {
                var existing = (await _instanceRepository.GetByItemId(instance.ItemId))
                    .FirstOrDefault(ii => string.Equals(ii.SerialNumber, instance.SerialNumber, StringComparison.OrdinalIgnoreCase));
                if (existing != null)
                {
                    return Conflict($"An item instance with serial number '{instance.SerialNumber}' already exists for item {instance.ItemId}.");
                }
            }

            await _instanceRepository.Add(instance);
            return CreatedAtAction(nameof(GetById), new { id = instance.Id }, instance);
        }

        // GET: api/iteminstances/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var instance = await _instanceRepository.GetById(id);
            if (instance == null) return NotFound();

            var customFieldValues = await _customFieldValueRepository.GetByItemInstanceId(id);

            return Ok(new
            {
                instance.Id,
                instance.ItemId,
                instance.SerialNumber,
                instance.Status,
                instance.CreatedAt,
                instance.UpdatedAt,
                CustomFieldValues = customFieldValues
            });
        }

        // PUT: api/iteminstances/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ItemInstance instance)
        {
            var existing = await _instanceRepository.GetById(id);
            if (existing == null) return NotFound();

            instance.Id = id;
            await _instanceRepository.Update(instance);

            return Ok(await _instanceRepository.GetById(id));
        }

        // GET: api/items/5/instances
        [HttpGet("/api/items/{itemId}/instances")]
        public async Task<IActionResult> GetByItemId(int itemId)
        {
            var item = await _itemRepository.GetById(itemId);
            if (item == null) return NotFound();

            var instances = await _instanceRepository.GetByItemId(itemId);
            var instanceList = instances.ToList();
            return Ok(new { itemId, count = instanceList.Count, instances = instanceList });
        }
    }
}