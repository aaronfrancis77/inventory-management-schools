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

        private static object Project(CustomFieldValue cfv) => new
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
        };

        private static bool ValidateDataType(string dataType, string value) => dataType switch
        {
            nameof(CustomFieldDataType.Number) => double.TryParse(value, out _),
            nameof(CustomFieldDataType.Integer) => int.TryParse(value, out _),
            nameof(CustomFieldDataType.Boolean) => value == "true" || value == "false",
            nameof(CustomFieldDataType.Date) => DateTime.TryParseExact(value, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out _),
            nameof(CustomFieldDataType.DateTime) => DateTime.TryParse(value, out _),
            nameof(CustomFieldDataType.Time) => TimeSpan.TryParse(value, out _),
            _ => true
        };

        // GET: api/customfieldvalues
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var all = await _repo.GetAll();
            return Ok(all.Select(Project));
        }

        // GET: api/customfieldvalues/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var v = await _repo.GetById(id);
            if (v == null) return NotFound();
            return Ok(Project(v));
        }

        // POST: api/customfieldvalues
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CustomFieldValue value)
        {
            var cf = await _context.CustomFields.FindAsync(value.CustomFieldId);
            if (cf == null) return NotFound($"CustomField {value.CustomFieldId} not found.");

            if (!ValidateDataType(cf.DataType, value.Value))
                return BadRequest($"Value '{value.Value}' is not valid for DataType '{cf.DataType}'.");

            if (value.ItemId.HasValue)
            {
                var item = await _itemRepository.GetById(value.ItemId.Value);
                if (item == null) return NotFound($"Item {value.ItemId.Value} not found.");
            }

            if (value.ItemInstanceId.HasValue)
            {
                var instance = await _instanceRepository.GetById(value.ItemInstanceId.Value);
                if (instance == null) return NotFound($"ItemInstance {value.ItemInstanceId.Value} not found.");
            }

            if (cf.IsUnique && value.ItemInstanceId.HasValue)
            {
                var duplicate = (await _repo.GetByCustomFieldId(value.CustomFieldId))
                    .FirstOrDefault(v =>
                        v.ItemInstanceId.HasValue &&
                        v.ItemInstanceId != value.ItemInstanceId &&
                        string.Equals(v.Value, value.Value, StringComparison.OrdinalIgnoreCase));

                if (duplicate != null)
                    return Conflict($"A unique custom field '{cf.Name}' already has the value '{value.Value}' on another instance.");
            }

            await _repo.Add(value);
            var created = await _repo.GetById(value.Id);
            return CreatedAtAction(nameof(GetById), new { id = value.Id }, Project(created!));
        }

        // PUT: api/customfieldvalues/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CustomFieldValue value)
        {
            var existing = await _repo.GetById(id);
            if (existing == null) return NotFound();

            var cf = await _context.CustomFields.FindAsync(existing.CustomFieldId);
            if (cf == null) return NotFound($"CustomField {existing.CustomFieldId} not found.");

            if (!ValidateDataType(cf.DataType, value.Value))
                return BadRequest($"Value '{value.Value}' is not valid for DataType '{cf.DataType}'.");

            if (cf.IsUnique && existing.ItemInstanceId.HasValue)
            {
                var duplicate = (await _repo.GetByCustomFieldId(existing.CustomFieldId))
                    .FirstOrDefault(v =>
                        v.ItemInstanceId.HasValue &&
                        v.Id != id &&
                        string.Equals(v.Value, value.Value, StringComparison.OrdinalIgnoreCase));

                if (duplicate != null)
                    return Conflict($"A unique custom field '{cf.Name}' already has the value '{value.Value}' on another instance.");
            }

            value.Id = id;
            await _repo.Update(value);
            var updated = await _repo.GetById(id);
            return Ok(Project(updated!));
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
