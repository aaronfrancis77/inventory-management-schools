using DaymapInventory.Data;
using DaymapInventory.Interfaces;
using DaymapInventory.Models;
using DaymapInventory.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaymapInventory.Tests
{
    [TestClass]
    public class SqlCommentRepositoryTests
    {
        private AppDbContext _context = null!;
        private ICommentRepository _repository = null!;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _repository = new SqlCommentRepository(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        [TestMethod]
        public async Task Add_ShouldStoreComment()
        {
            var item = new Item { Name = "Projector" };
            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            var comment = new Comment { ItemId = item.Id, Body = "Needs a new bulb", CreatedBy = 1 };

            await _repository.Add(comment);

            Assert.AreEqual(1, (await _repository.GetAll()).Count());
        }

        [TestMethod]
        public async Task Add_ShouldSetCreatedAtAutomatically()
        {
            var item = new Item { Name = "Laptop" };
            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            var comment = new Comment { ItemId = item.Id, Body = "Screen flickers" };

            await _repository.Add(comment);

            Assert.AreNotEqual(default(DateTime), comment.CreatedAt);
        }

        [TestMethod]
        public async Task GetById_ShouldReturnCorrectComment()
        {
            var item = new Item { Name = "Whiteboard" };
            _context.Items.Add(item);
            await _context.SaveChangesAsync();
            var comment = new Comment { ItemId = item.Id, Body = "Missing marker tray" };
            await _repository.Add(comment);

            var result = await _repository.GetById(comment.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual("Missing marker tray", result.Body);
        }

        [TestMethod]
        public async Task GetById_ShouldReturnNullWhenNotFound()
        {
            var result = await _repository.GetById(999);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task Update_ShouldModifyCommentBody()
        {
            var item = new Item { Name = "Camera" };
            _context.Items.Add(item);
            await _context.SaveChangesAsync();
            var comment = new Comment { ItemId = item.Id, Body = "Old body" };
            await _repository.Add(comment);

            comment.Body = "Updated body";
            await _repository.Update(comment);

            var result = await _repository.GetById(comment.Id);
            Assert.AreEqual("Updated body", result!.Body);
        }

        [TestMethod]
        public async Task Delete_ShouldRemoveComment()
        {
            var item = new Item { Name = "Speaker" };
            _context.Items.Add(item);
            await _context.SaveChangesAsync();
            var comment = new Comment { ItemId = item.Id, Body = "Crackling sound" };
            await _repository.Add(comment);

            await _repository.Delete(comment.Id);

            Assert.IsNull(await _repository.GetById(comment.Id));
            Assert.AreEqual(0, (await _repository.GetAll()).Count());
        }

        [TestMethod]
        public async Task Delete_NonExistentComment_ShouldNotThrow()
        {
            await _repository.Delete(999);

            Assert.AreEqual(0, (await _repository.GetAll()).Count());
        }

        [TestMethod]
        public async Task GetByItemId_ShouldReturnOnlyCommentsForThatItem()
        {
            var itemA = new Item { Name = "Item A" };
            var itemB = new Item { Name = "Item B" };
            _context.Items.AddRange(itemA, itemB);
            await _context.SaveChangesAsync();

            await _repository.Add(new Comment { ItemId = itemA.Id, Body = "Comment on A - 1" });
            await _repository.Add(new Comment { ItemId = itemA.Id, Body = "Comment on A - 2" });
            await _repository.Add(new Comment { ItemId = itemB.Id, Body = "Comment on B" });

            var results = await _repository.GetByItemId(itemA.Id);

            Assert.AreEqual(2, results.Count());
            Assert.IsTrue(results.All(c => c.ItemId == itemA.Id));
        }

        [TestMethod]
        public async Task GetByItemId_ShouldReturnEmptyWhenNoComments()
        {
            var item = new Item { Name = "Untouched Item" };
            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            var results = await _repository.GetByItemId(item.Id);

            Assert.AreEqual(0, results.Count());
        }

        [TestMethod]
        public async Task GetByItemId_ShouldReturnInChronologicalOrder()
        {
            var item = new Item { Name = "Ordered Item" };
            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            var first = new Comment { ItemId = item.Id, Body = "First", CreatedAt = DateTime.UtcNow.AddMinutes(-10) };
            var second = new Comment { ItemId = item.Id, Body = "Second", CreatedAt = DateTime.UtcNow };
            _context.Comments.Add(first);
            _context.Comments.Add(second);
            await _context.SaveChangesAsync();

            var results = (await _repository.GetByItemId(item.Id)).ToList();

            Assert.AreEqual("First", results[0].Body);
            Assert.AreEqual("Second", results[1].Body);
        }
    }
}
