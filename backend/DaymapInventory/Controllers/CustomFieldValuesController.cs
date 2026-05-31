using DaymapInventory.Data;
using DaymapInventory.Interfaces;
using DaymapInventory.Models;
using Microsoft.AspNetCore.Mvc;

namespace DaymapInventory.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomFieldValuesController : ControllerBase
    {
        private readonly ICustomFieldValueRepository _repo;
        private readonly IItemRepository _itemRepository;
        private readonly IItemInstanceRepository _instanceRepository;
        private readonly AppDbContext _context;

        public CustomFieldValuesController(ICustomFieldValueRepository repo, IItemRepository itemRepository, IItemInstanceRepository instanceRepository, AppDbContext context)
        {
            _repo = repo;
            _itemRepository = itemRepository;
            _instanceRepository = instanceRepository;
            _context = context;
        }

        // GET: api/customfieldvalues
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var all = await _repo.GetAll();
            return Ok(all);
        }

        // GET: api/customfieldvalues/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var v = await _repo.GetById(id);
            if (v == null) return NotFound();
            return Ok(v);
        }

        // POST: api/customfieldvalues
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CustomFieldValue value)
        {
            // Ensure the custom field exists
            var cf = await _context.CustomFields.FindAsync(value.CustomFieldId);
            if (cf == null) return NotFound($"CustomField {value.CustomFieldId} not found.");

            // If an ItemId is provided, ensure item exists
            if (value.ItemId.HasValue)
            {
                var item = await _itemRepository.GetById(value.ItemId.Value);
                if (item == null) return NotFound($"Item {value.ItemId.Value} not found.");
            }

            // If an ItemInstanceId is provided, ensure instance exists
            if (value.ItemInstanceId.HasValue)
            {
                var instance = await _instanceRepository.GetById(value.ItemInstanceId.Value);
                if (instance == null) return NotFound($"ItemInstance {value.ItemInstanceId.Value} not found.");
            }

            // Uniqueness check: if the custom field is marked IsUnique and this value is for an instance,
            // reject if another instance already has the same value for this field
            if (cf.IsUnique && value.ItemInstanceId.HasValue)
            {
                var existingValues = await _repo.GetByCustomFieldId(value.CustomFieldId);
                var duplicate = existingValues.FirstOrDefault(v =>
                    v.ItemInstanceId.HasValue &&
                    v.ItemInstanceId != value.ItemInstanceId &&
                    string.Equals(v.Value, value.Value, StringComparison.OrdinalIgnoreCase));

                if (duplicate != null)
                    return Conflict($"A unique custom field '{cf.Name}' already has the value '{value.Value}' on another instance.");
            }

            await _repo.Add(value);
            return CreatedAtAction(nameof(GetById), new { id = value.Id }, value);
        }

        // PUT: api/customfieldvalues/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CustomFieldValue value)
        {
            var existing = await _repo.GetById(id);
            if (existing == null) return NotFound();
            value.Id = id;
            await _repo.Update(value);
            return Ok(await _repo.GetById(id));
        }

        // DELETE: api/customfieldvalues/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _repo.GetById(id);
            if (existing == null) return NotFound();
            await _repo.Delete(id);
            return NoContent();
        }
    }
}
