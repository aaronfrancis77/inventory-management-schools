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

        public ItemInstancesController(IItemInstanceRepository instanceRepository, IItemRepository itemRepository)
        {
            _instanceRepository = instanceRepository;
            _itemRepository = itemRepository;
        }

        // POST: api/iteminstances
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ItemInstance instance)
        {
            var item = await _itemRepository.GetById(instance.ItemId);
            if (item == null) return NotFound($"Item with id {instance.ItemId} not found.");

            await _instanceRepository.Add(instance);
            return CreatedAtAction(nameof(GetById), new { id = instance.Id }, instance);
        }

        // GET: api/iteminstances/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var instance = await _instanceRepository.GetById(id);
            if (instance == null) return NotFound();
            return Ok(instance);
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
