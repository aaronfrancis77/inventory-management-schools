using DaymapInventory.Controllers;
using DaymapInventory.Data;
using DaymapInventory.Interfaces;
using DaymapInventory.Models;
using DaymapInventory.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaymapInventory.Tests
{
    [TestClass]
    public class CommentsControllerTests
    {
        private AppDbContext _context = null!;
        private ICommentRepository _commentRepository = null!;
        private IItemRepository _itemRepository = null!;
        private CommentsController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _context = TestDbContextFactory.CreateInMemoryContext();
            _commentRepository = new SqlCommentRepository(_context);
            _itemRepository = new SqlItemRepository(_context);
            _controller = new CommentsController(_commentRepository, _itemRepository);
        }

        [TestCleanup]
        public void Cleanup() => _context.Dispose();

        [TestMethod]
        public async Task GetComments_ExistingItemWithComments_ReturnsOk()
        {
            var item = new Item { Name = "Notebook", Description = "Used for tests", Status = "Active" };
            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            _context.Comments.Add(new Comment { ItemId = item.Id, Body = "First note" });
            await _context.SaveChangesAsync();

            var result = await _controller.GetComments(item.Id);

            Assert.IsInstanceOfType<OkObjectResult>(result);
        }

        [TestMethod]
        public async Task CreateComment_EmptyBody_ReturnsBadRequest()
        {
            var item = new Item { Name = "Notebook", Description = "Used for tests", Status = "Active" };
            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            var result = await _controller.Create(item.Id, new Comment { Body = "" });

            Assert.IsInstanceOfType<BadRequestObjectResult>(result);
        }

        [TestMethod]
        public async Task DeleteComment_MissingComment_ReturnsNotFound()
        {
            var result = await _controller.Delete(999);

            Assert.IsInstanceOfType<NotFoundResult>(result);
        }

        // SCRUM 164: remaining coverage for all three endpoints

        private async Task<Item> SeedItem(string name = "Notebook")
        {
            var item = new Item { Name = name, Description = "Used for tests", Status = "Active" };
            _context.Items.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        [TestMethod]
        public async Task GetComments_MissingItem_ReturnsNotFound()
        {
            var result = await _controller.GetComments(999);

            Assert.IsInstanceOfType<NotFoundResult>(result);
        }

        [TestMethod]
        public async Task GetComments_ItemWithNoComments_ReturnsOkWithEmptyList()
        {
            var item = await SeedItem();

            var result = await _controller.GetComments(item.Id);

            var ok = result as OkObjectResult;
            Assert.IsNotNull(ok);
            var comments = ok.Value as IEnumerable<Comment>;
            Assert.IsNotNull(comments);
            Assert.AreEqual(0, comments.Count());
        }

        [TestMethod]
        public async Task GetComments_ReturnsOnlyCommentsForRequestedItem()
        {
            var itemA = await SeedItem("Item A");
            var itemB = await SeedItem("Item B");
            _context.Comments.Add(new Comment { ItemId = itemA.Id, Body = "On A" });
            _context.Comments.Add(new Comment { ItemId = itemB.Id, Body = "On B" });
            await _context.SaveChangesAsync();

            var result = await _controller.GetComments(itemA.Id);

            var comments = ((result as OkObjectResult)!.Value as IEnumerable<Comment>)!.ToList();
            Assert.AreEqual(1, comments.Count);
            Assert.AreEqual("On A", comments[0].Body);
        }

        [TestMethod]
        public async Task CreateComment_ValidBody_ReturnsCreatedAndPersists()
        {
            var item = await SeedItem();

            var result = await _controller.Create(item.Id, new Comment { Body = "Charger missing", CreatedBy = 3 });

            var created = result as CreatedAtActionResult;
            Assert.IsNotNull(created);
            Assert.AreEqual(nameof(CommentsController.GetComments), created.ActionName);

            _context.ChangeTracker.Clear();
            var stored = (await _commentRepository.GetByItemId(item.Id)).Single();
            Assert.AreEqual("Charger missing", stored.Body);
            Assert.AreEqual(3, stored.CreatedBy);
        }

        [TestMethod]
        public async Task CreateComment_ItemIdTakenFromRouteNotBody()
        {
            var item = await SeedItem();

            await _controller.Create(item.Id, new Comment { ItemId = 12345, Body = "Route wins" });

            _context.ChangeTracker.Clear();
            var stored = (await _commentRepository.GetAll()).Single();
            Assert.AreEqual(item.Id, stored.ItemId);
        }

        [TestMethod]
        public async Task CreateComment_MissingItem_ReturnsNotFoundAndSavesNothing()
        {
            var result = await _controller.Create(999, new Comment { Body = "Orphan comment" });

            Assert.IsInstanceOfType<NotFoundResult>(result);
            Assert.AreEqual(0, (await _commentRepository.GetAll()).Count());
        }

        [TestMethod]
        public async Task CreateComment_WhitespaceBody_ReturnsBadRequest()
        {
            var item = await SeedItem();

            var result = await _controller.Create(item.Id, new Comment { Body = "   " });

            Assert.IsInstanceOfType<BadRequestObjectResult>(result);
            Assert.AreEqual(0, (await _commentRepository.GetAll()).Count());
        }

        [TestMethod]
        public async Task DeleteComment_ExistingComment_ReturnsNoContentAndRemoves()
        {
            var item = await SeedItem();
            var comment = new Comment { ItemId = item.Id, Body = "To be deleted" };
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            var result = await _controller.Delete(comment.Id);

            Assert.IsInstanceOfType<NoContentResult>(result);
            _context.ChangeTracker.Clear();
            Assert.IsNull(await _commentRepository.GetById(comment.Id));
        }
    }
}