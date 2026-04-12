using DaymapInventory.Interfaces;
using DaymapInventory.Models;
using DaymapInventory.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaymapInventory.Tests
{
    [TestClass]
    public class InMemoryItemRepositoryTests
    {
        private IItemRepository _repository;

        [TestInitialize]
        public void Setup()
        {
            // Programming against the interface means we can swap in the SQL
            // implementation here later without changing a single test.
            _repository = new InMemoryItemRepository();
        }

        [TestMethod]
        public void Add_ShouldAssignIdAndStore()
        {
            var item = new Item { Name = "Textbook", Description = "Year 12 Maths" };

            _repository.Add(item);

            Assert.AreEqual(1, item.Id);
            Assert.AreEqual(1, _repository.GetAll().Count());
        }

        [TestMethod]
        public void GetById_ShouldReturnCorrectItem()
        {
            var item = new Item { Name = "Laptop", StockCount = 5 };
            _repository.Add(item);

            var result = _repository.GetById(item.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual("Laptop", result.Name);
            Assert.AreEqual(5, result.StockCount);
        }

        [TestMethod]
        public void GetById_ShouldReturnNullWhenNotFound()
        {
            var result = _repository.GetById(999);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Update_ShouldModifyExistingItem()
        {
            var item = new Item { Name = "Stethoscope", StockCount = 10 };
            _repository.Add(item);

            item.StockCount = 8;
            item.Status = ItemStatus.Disabled.ToString();
            _repository.Update(item);

            var result = _repository.GetById(item.Id);
            Assert.AreEqual(8, result!.StockCount);
            Assert.AreEqual("Disabled", result.Status);
        }

        [TestMethod]
        public void Delete_ShouldRemoveItem()
        {
            var item = new Item { Name = "Projector" };
            _repository.Add(item);

            _repository.Delete(item.Id);

            Assert.AreEqual(0, _repository.GetAll().Count());
            Assert.IsNull(_repository.GetById(item.Id));
        }

        [TestMethod]
        public void GetByStatus_ShouldFilterCorrectly()
        {
            _repository.Add(new Item { Name = "Active Item", Status = ItemStatus.Active.ToString() });
            _repository.Add(new Item { Name = "Archived Item", Status = ItemStatus.Archived.ToString() });
            _repository.Add(new Item { Name = "Another Active", Status = ItemStatus.Active.ToString() });

            var activeItems = _repository.GetByStatus(ItemStatus.Active.ToString());

            Assert.AreEqual(2, activeItems.Count());
        }

        [TestMethod]
        public void GetLowStock_ShouldReturnItemsBelowThreshold()
        {
            _repository.Add(new Item { Name = "Bandages", StockCount = 3, LowStockThreshold = 5 });
            _repository.Add(new Item { Name = "Gloves", StockCount = 50, LowStockThreshold = 10 });

            var lowStock = _repository.GetLowStock();

            Assert.AreEqual(1, lowStock.Count());
            Assert.AreEqual("Bandages", lowStock.First().Name);
        }
    }
}
