using DaymapInventory.Data;
using DaymapInventory.Interfaces;
using DaymapInventory.Models;
using DaymapInventory.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaymapInventory.Tests
{
    [TestClass]
    public class SqlItemInstanceRepositoryTests
    {
        private AppDbContext _context = null!;
        private IItemInstanceRepository _repository = null!;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _repository = new SqlItemInstanceRepository(_context);

            // Seed a parent item for all tests
            _context.Items.Add(new Item { Id = 1, Name = "Laptop", StockCount = 0 });
            _context.SaveChanges();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        [TestMethod]
        public async Task Add_ShouldStoreInstance()
        {
            var instance = new ItemInstance { ItemId = 1, SerialNumber = "LAPTOP-001", Status = "Available" };

            await _repository.Add(instance);

            Assert.IsTrue(instance.Id > 0);
            Assert.AreEqual(1, (await _repository.GetAll()).Count());
        }

        [TestMethod]
        public async Task GetById_ShouldReturnCorrectInstance()
        {
            var instance = new ItemInstance { ItemId = 1, SerialNumber = "LAPTOP-002", Status = "Available" };
            await _repository.Add(instance);

            var result = await _repository.GetById(instance.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual("LAPTOP-002", result.SerialNumber);
        }

        [TestMethod]
        public async Task GetById_ShouldReturnNullWhenNotFound()
        {
            var result = await _repository.GetById(999);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task Delete_ShouldRemoveInstance()
        {
            var instance = new ItemInstance { ItemId = 1, SerialNumber = "DELETE-001", Status = "Available" };
            await _repository.Add(instance);

            await _repository.Delete(instance.Id);

            Assert.AreEqual(0, (await _repository.GetAll()).Count());
            Assert.IsNull(await _repository.GetById(instance.Id));
        }

        [TestMethod]
        public async Task GetByItemId_ShouldReturnCorrectInstances()
        {
            _context.Items.Add(new Item { Id = 2, Name = "Projector", StockCount = 0 });
            await _context.SaveChangesAsync();

            await _repository.Add(new ItemInstance { ItemId = 1, SerialNumber = "A001", Status = "Available" });
            await _repository.Add(new ItemInstance { ItemId = 1, SerialNumber = "A002", Status = "Available" });
            await _repository.Add(new ItemInstance { ItemId = 2, SerialNumber = "B001", Status = "Available" });

            var results = await _repository.GetByItemId(1);

            Assert.AreEqual(2, results.Count());
        }

        [TestMethod]
        public async Task GetByItemId_ShouldShowCorrectCountAfterAddingInstance()
        {
            await _repository.Add(new ItemInstance { ItemId = 1, SerialNumber = "LAPTOP-001", Status = "Available" });

            var instances = await _repository.GetByItemId(1);

            Assert.AreEqual(1, instances.Count());
        }

        [TestMethod]
        public async Task Add_ShouldSyncParentItemStockCount()
        {
            await _repository.Add(new ItemInstance { ItemId = 1, SerialNumber = "LAPTOP-001", Status = "Available" });
            await _repository.Add(new ItemInstance { ItemId = 1, SerialNumber = "LAPTOP-002", Status = "Available" });

            var item = await _context.Items.FindAsync(1);

            Assert.AreEqual(2, item!.StockCount);
        }

        [TestMethod]
        public async Task Delete_ShouldDecrementParentItemStockCount()
        {
            var instance = new ItemInstance { ItemId = 1, SerialNumber = "LAPTOP-001", Status = "Available" };
            await _repository.Add(instance);

            await _repository.Delete(instance.Id);

            var item = await _context.Items.FindAsync(1);
            Assert.AreEqual(0, item!.StockCount);
        }
    }
}
