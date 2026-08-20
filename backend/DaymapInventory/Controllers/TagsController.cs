using DaymapInventory.Interfaces;
using DaymapInventory.Models;
using Microsoft.AspNetCore.Mvc;

namespace DaymapInventory.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagsController : ControllerBase
    {
        private readonly ITagRepository _repository;

        public TagsController(ITagRepository repository)
        {
            _repository = repository;
        }

        // GET: api/tags
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tags = await _repository.GetAll();
            return Ok(tags);
        }

        // GET: api/tags/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var tag = await _repository.GetById(id);
            if (tag == null) return NotFound();

            return Ok(tag);
        }

        // POST: api/tags
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Tag tag)
        {
            await _repository.Add(tag);
            return CreatedAtAction(nameof(GetById), new { id = tag.Id }, tag);
        }

        // PUT: api/tags/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Tag tag)
        {
            if (await _repository.GetById(id) == null) return NotFound();

            tag.Id = id;
            await _repository.Update(tag);

            var updated = await _repository.GetById(id);
            return Ok(updated);
        }

        // DELETE: api/tags/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await _repository.GetById(id) == null) return NotFound();

            await _repository.Delete(id);
            return NoContent();
        }
    }
}
