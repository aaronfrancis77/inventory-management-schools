using DaymapInventory.Interfaces;
using DaymapInventory.Models;
using Microsoft.AspNetCore.Mvc;

namespace DaymapInventory.Controllers
{
    [ApiController]
    [Route("api")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentRepository _repository;
        private readonly IItemRepository _itemRepository;

        public CommentsController(ICommentRepository repository, IItemRepository itemRepository)
        {
            _repository = repository;
            _itemRepository = itemRepository;
        }

        [HttpGet("items/{itemId}/comments")]
        public async Task<IActionResult> GetComments(int itemId)
        {
            if (await _itemRepository.GetById(itemId) == null)
                return NotFound();

            var comments = await _repository.GetByItemId(itemId);
            return Ok(comments);
        }

        [HttpPost("items/{itemId}/comments")]
        public async Task<IActionResult> Create(int itemId, [FromBody] Comment comment)
        {
            if (await _itemRepository.GetById(itemId) == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(comment.Body))
                return BadRequest("Comment body is required.");

            comment.ItemId = itemId;
            await _repository.Add(comment);
            return CreatedAtAction(nameof(GetComments), new { itemId }, comment);
        }

        [HttpDelete("comments/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _repository.GetById(id);
            if (existing == null)
                return NotFound();

            await _repository.Delete(id);
            return NoContent();
        }
    }
}
