using DaymapInventory.Data;
using DaymapInventory.Interfaces;
using DaymapInventory.Models;
using DaymapInventory.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaymapInventory.Tests
{
    [TestClass]
    public class SqlItemRepositoryTests
    {
        private AppDbContext _context = null!;
        private IItemRepository _repository = null!;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _repository = new SqlItemRepository(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        [TestMethod]
        public async Task Add_ShouldStoreItem()
        {
            var item = new Item { Name = "Textbook", Description = "Year 12 Maths" };

            await _repository.Add(item);

            Assert.AreEqual(1, (await _repository.GetAll()).Count());
        }

        [TestMethod]
        public async Task GetById_ShouldReturnCorrectItem()
        {
            var item = new Item { Name = "Laptop", StockCount = 5 };
            await _repository.Add(item);

            var result = await _repository.GetById(item.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual("Laptop", result.Name);
            Assert.AreEqual(5, result.StockCount);
        }

        [TestMethod]
        public async Task GetById_ShouldReturnNullWhenNotFound()
        {
            var result = await _repository.GetById(999);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task Update_ShouldModifyExistingItem()
        {
            var item = new Item { Name = "Stethoscope", StockCount = 10 };
            await _repository.Add(item);

            item.StockCount = 8;
            item.Status = ItemStatus.Disabled.ToString();
            await _repository.Update(item);

            var result = await _repository.GetById(item.Id);
            Assert.AreEqual(8, result!.StockCount);
            Assert.AreEqual("Disabled", result.Status);
        }

        [TestMethod]
        public async Task Delete_ShouldRemoveItem()
        {
            var item = new Item { Name = "Projector" };
            await _repository.Add(item);

            await _repository.Delete(item.Id);

            Assert.AreEqual(0, (await _repository.GetAll()).Count());
            Assert.IsNull(await _repository.GetById(item.Id));
        }

        [TestMethod]
        public async Task GetByStatus_ShouldFilterCorrectly()
        {
            await _repository.Add(new Item { Name = "Active Item", Status = ItemStatus.Active.ToString() });
            await _repository.Add(new Item { Name = "Archived Item", Status = ItemStatus.Archived.ToString() });
            await _repository.Add(new Item { Name = "Another Active", Status = ItemStatus.Active.ToString() });

            var activeItems = await _repository.GetByStatus(ItemStatus.Active.ToString());

            Assert.AreEqual(2, activeItems.Count());
        }

        [TestMethod]
        public async Task GetLowStock_ShouldReturnItemsBelowThreshold()
        {
            await _repository.Add(new Item { Name = "Bandages", StockCount = 3, LowStockThreshold = 5 });
            await _repository.Add(new Item { Name = "Gloves", StockCount = 50, LowStockThreshold = 10 });

            var lowStock = await _repository.GetLowStock();

            Assert.AreEqual(1, lowStock.Count());
            Assert.AreEqual("Bandages", lowStock.First().Name);
        }
    }
}
