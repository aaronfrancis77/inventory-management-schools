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
    public class TagsControllerTests
    {
        private AppDbContext _context = null!;
        private TagsController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _controller = new TagsController(new SqlTagRepository(_context));
        }

        [TestCleanup]
        public void Cleanup() => _context.Dispose();

        [TestMethod]
        public async Task Create_ValidTag_ReturnsCreatedAtAction()
        {
            var tag = new Tag
            {
                Name = "Science",
                Colour = "#ff0000",
                IsDefault = false
            };

            var result = await _controller.Create(tag);

            Assert.IsInstanceOfType<CreatedAtActionResult>(result);
            Assert.AreEqual(1, await _context.Tags.CountAsync());
            Assert.AreEqual("#ff0000", (await _context.Tags.FirstAsync()).Colour);
        }

        [TestMethod]
        public async Task GetById_MissingTag_ReturnsNotFound()
        {
            var result = await _controller.GetById(999);

            Assert.IsInstanceOfType<NotFoundResult>(result);
        }

        [TestMethod]
        public async Task Delete_ExistingTag_RemovesTag()
        {
            var tag = new Tag { Name = "History", Colour = "#00ff00" };
            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();

            var result = await _controller.Delete(tag.Id);

            Assert.IsInstanceOfType<NoContentResult>(result);
            Assert.AreEqual(0, await _context.Tags.CountAsync());
        }
    }
}
