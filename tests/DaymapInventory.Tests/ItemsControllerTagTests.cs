using DaymapInventory.Controllers;
using DaymapInventory.Data;
using DaymapInventory.Models;
using DaymapInventory.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaymapInventory.Tests
{
    // SCRUM-93: Tags - item tag assignment endpoints
    [TestClass]
    public class ItemsControllerTagTests
    {
        private AppDbContext _context = null!;
        private ItemsController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _controller = new ItemsController(
                new SqlItemRepository(_context),
                new SqlCustomFieldValueRepository(_context),
                new SqlTagRepository(_context));
        }

        [TestCleanup]
        public void Cleanup() => _context.Dispose();

        private async Task<(Item item, Tag tag)> SeedItemAndTag()
        {
            var item = new Item { Name = "Microscope" };
            var tag = new Tag { Name = "Science", Colour = "#00ff00" };
            _context.Items.Add(item);
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();
            return (item, tag);
        }

        [TestMethod]
        public async Task AssignTag_ValidItemAndTag_ReturnsNoContent()
        {
            var (item, tag) = await SeedItemAndTag();

            var result = await _controller.AssignTag(item.Id, tag.Id);

            Assert.IsInstanceOfType<NoContentResult>(result);
            Assert.AreEqual(1, await _context.ItemTags.CountAsync());
        }

        [TestMethod]
        public async Task AssignTag_MissingItem_ReturnsNotFound()
        {
            var tag = new Tag { Name = "Science" };
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();

            var result = await _controller.AssignTag(999, tag.Id);

            Assert.IsInstanceOfType<NotFoundResult>(result);
        }

        [TestMethod]
        public async Task AssignTag_MissingTag_ReturnsNotFound()
        {
            var item = new Item { Name = "Microscope" };
            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            var result = await _controller.AssignTag(item.Id, 999);

            Assert.IsInstanceOfType<NotFoundResult>(result);
        }

        [TestMethod]
        public async Task AssignTag_AlreadyAssigned_ReturnsConflict()
        {
            var (item, tag) = await SeedItemAndTag();
            await _controller.AssignTag(item.Id, tag.Id);

            var result = await _controller.AssignTag(item.Id, tag.Id);

            Assert.IsInstanceOfType<ConflictObjectResult>(result);
            Assert.AreEqual(1, await _context.ItemTags.CountAsync());
        }

        [TestMethod]
        public async Task RemoveTag_ExistingAssignment_ReturnsNoContent()
        {
            var (item, tag) = await SeedItemAndTag();
            await _controller.AssignTag(item.Id, tag.Id);

            var result = await _controller.RemoveTag(item.Id, tag.Id);

            Assert.IsInstanceOfType<NoContentResult>(result);
            Assert.AreEqual(0, await _context.ItemTags.CountAsync());
        }

        [TestMethod]
        public async Task RemoveTag_MissingItem_ReturnsNotFound()
        {
            var tag = new Tag { Name = "Science" };
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();

            var result = await _controller.RemoveTag(999, tag.Id);

            Assert.IsInstanceOfType<NotFoundResult>(result);
        }

        [TestMethod]
        public async Task GetTags_ReturnsAssignedTagsForItem()
        {
            var (item, tag) = await SeedItemAndTag();
            await _controller.AssignTag(item.Id, tag.Id);

            var result = await _controller.GetTags(item.Id);

            var ok = result as OkObjectResult;
            Assert.IsNotNull(ok);
        }

        [TestMethod]
        public async Task GetTags_MissingItem_ReturnsNotFound()
        {
            var result = await _controller.GetTags(999);

            Assert.IsInstanceOfType<NotFoundResult>(result);
        }
    }
}
