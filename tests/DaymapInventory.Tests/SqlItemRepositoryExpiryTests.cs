using DaymapInventory.Data;
using DaymapInventory.Interfaces;
using DaymapInventory.Models;
using DaymapInventory.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaymapInventory.Tests
{
    [TestClass]
    public class SqlItemRepositoryExpiryTests
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
        public async Task GetExpiringSoon_ShouldReturnItemsWithinRange()
        {
            var withinRange = new Item { Name = "Milk", ExpiryDate = DateTime.UtcNow.AddDays(10) };
            _context.Items.Add(withinRange);
            await _context.SaveChangesAsync();

            var results = await _repository.GetExpiringSoon(30);

            Assert.AreEqual(1, results.Count());
            Assert.AreEqual("Milk", results.First().Name);
        }

        [TestMethod]
        public async Task GetExpiringSoon_ShouldExcludeItemsOutsideRange()
        {
            var outsideRange = new Item { Name = "Canned Beans", ExpiryDate = DateTime.UtcNow.AddDays(60) };
            _context.Items.Add(outsideRange);
            await _context.SaveChangesAsync();

            var results = await _repository.GetExpiringSoon(30);

            Assert.AreEqual(0, results.Count());
        }

        [TestMethod]
        public async Task GetExpiringSoon_ShouldExcludeItemsWithNoExpiryDate()
        {
            var noExpiry = new Item { Name = "Whiteboard", ExpiryDate = null };
            _context.Items.Add(noExpiry);
            await _context.SaveChangesAsync();

            var results = await _repository.GetExpiringSoon(30);

            Assert.AreEqual(0, results.Count());
        }

        [TestMethod]
        public async Task GetExpiringSoon_ShouldExcludeAlreadyExpiredItems()
        {
            var alreadyExpired = new Item { Name = "Yogurt", ExpiryDate = DateTime.UtcNow.AddDays(-2) };
            _context.Items.Add(alreadyExpired);
            await _context.SaveChangesAsync();

            var results = await _repository.GetExpiringSoon(30);

            Assert.AreEqual(0, results.Count());
        }

        [TestMethod]
        public async Task GetExpired_ShouldReturnExpiredItemsCorrectly()
        {
            var expired = new Item { Name = "Old Bread", ExpiryDate = DateTime.UtcNow.AddDays(-5) };
            _context.Items.Add(expired);
            await _context.SaveChangesAsync();

            var results = await _repository.GetExpired();

            Assert.AreEqual(1, results.Count());
            Assert.AreEqual("Old Bread", results.First().Name);
        }

        [TestMethod]
        public async Task GetExpired_ShouldExcludeItemsNotYetExpired()
        {
            var notExpired = new Item { Name = "Fresh Juice", ExpiryDate = DateTime.UtcNow.AddDays(5) };
            _context.Items.Add(notExpired);
            await _context.SaveChangesAsync();

            var results = await _repository.GetExpired();

            Assert.AreEqual(0, results.Count());
        }

        [TestMethod]
        public async Task GetExpired_ShouldExcludeItemsWithNoExpiryDate()
        {
            var noExpiry = new Item { Name = "Chair", ExpiryDate = null };
            _context.Items.Add(noExpiry);
            await _context.SaveChangesAsync();

            var results = await _repository.GetExpired();

            Assert.AreEqual(0, results.Count());
        }
    }
}