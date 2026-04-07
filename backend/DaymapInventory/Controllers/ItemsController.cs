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

        public ItemsController(IItemRepository repository)
        {
            _repository = repository;
        }

        // GET: api/items
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_repository.GetAll());
        }

        // GET: api/items/5
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var item = _repository.GetById(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        // POST: api/items
        [HttpPost]
        public IActionResult Create([FromBody] Item item)
        {
            _repository.Add(item);
            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }

        // PUT: api/items/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Item item)
        {
            if (_repository.GetById(id) == null) return NotFound();
            item.Id = id;
            _repository.Update(item);
            return Ok(_repository.GetById(id));
        }

        // DELETE: api/items/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (_repository.GetById(id) == null) return NotFound();
            _repository.Delete(id);
            return NoContent();
        }
    }
}
