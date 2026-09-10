using DaymapInventory.Data;
using DaymapInventory.Interfaces;
using DaymapInventory.Models;
using DaymapInventory.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaymapInventory.Tests
{
    [TestClass]
    public class SqlItemRepositorySearchTests
    {
        private AppDbContext _context = null!;
        private IItemRepository _repository = null!;

        [TestInitialize]
        public void Setup()
        {
            _context = TestDbContextFactory.CreateInMemoryContext();
            _repository = new SqlItemRepository(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        [TestMethod]
        public async Task Search_ShouldMatchKeywordInName()
        {
            var item = new Item { Name = "Dell Laptop" };
            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            var results = await _repository.Search("laptop", null);

            Assert.AreEqual(1, results.Count());
            Assert.AreEqual("Dell Laptop", results.First().Name);
        }

        [TestMethod]
        public async Task Search_ShouldMatchKeywordInDescription()
        {
            var item = new Item { Name = "Projector", Description = "Great for classroom presentations" };
            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            var results = await _repository.Search("classroom", null);

            Assert.AreEqual(1, results.Count());
            Assert.AreEqual("Projector", results.First().Name);
        }

        [TestMethod]
        public async Task Search_ShouldBeCaseInsensitive()
        {
            var item = new Item { Name = "Whiteboard Marker" };
            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            var results = await _repository.Search("WHITEBOARD", null);

            Assert.AreEqual(1, results.Count());
        }

        [TestMethod]
        public async Task Search_ShouldFilterByCategory()
        {
            var electronics = new Category { Name = "Electronics" };
            var stationery = new Category { Name = "Stationery" };
            _context.Categories.AddRange(electronics, stationery);
            await _context.SaveChangesAsync();

            var laptop = new Item { Name = "Gaming Laptop" };
            var laptopSleeve = new Item { Name = "Laptop Sleeve" };
            _context.Items.AddRange(laptop, laptopSleeve);
            await _context.SaveChangesAsync();

            _context.ItemCategories.Add(new ItemCategory { ItemId = laptop.Id, CategoryId = electronics.Id });
            _context.ItemCategories.Add(new ItemCategory { ItemId = laptopSleeve.Id, CategoryId = stationery.Id });
            await _context.SaveChangesAsync();

            var results = await _repository.Search("laptop", electronics.Id);

            Assert.AreEqual(1, results.Count());
            Assert.AreEqual("Gaming Laptop", results.First().Name);
        }

        [TestMethod]
        public async Task Search_ShouldReturnEmptyArrayWhenNoMatches()
        {
            var item = new Item { Name = "Stapler" };
            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            var results = await _repository.Search("nonexistentkeyword", null);

            Assert.AreEqual(0, results.Count());
        }

        [TestMethod]
        public async Task Search_ShouldMatchKeywordInTags()
        {
            var tag = new Tag { Name = "Fragile" };
            _context.Tags.Add(tag);
            var item = new Item { Name = "Glass Beaker" };
            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            _context.ItemTags.Add(new ItemTag { ItemId = item.Id, TagId = tag.Id });
            await _context.SaveChangesAsync();

            var results = await _repository.Search("fragile", null);

            Assert.AreEqual(1, results.Count());
            Assert.AreEqual("Glass Beaker", results.First().Name);
        }
    }
}