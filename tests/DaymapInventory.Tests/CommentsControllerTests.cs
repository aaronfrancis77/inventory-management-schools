using DaymapInventory.Controllers;
using DaymapInventory.Data;
using DaymapInventory.Models;
using DaymapInventory.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaymapInventory.Tests
{
    [TestClass]
    public class CommentsControllerTests
    {
        private AppDbContext _context = null!;
        private CommentsController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _controller = new CommentsController(
                new SqlCommentRepository(_context),
                new SqlItemRepository(_context));
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
    }
}
