using DaymapInventory.Interfaces;
using DaymapInventory.Models;
using DaymapInventory.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaymapInventory.Tests
{
    [TestClass]
    public class InMemoryItemInstanceRepositoryTests
    {
        private IItemInstanceRepository _repository = null!;

        [TestInitialize]
        public void Setup()
        {
            _repository = new InMemoryItemInstanceRepository();
        }

        [TestMethod]
        public async Task Add_ShouldAssignIdAndStore()
        {
            var instance = new ItemInstance
            {
                ItemId = 1,
                SerialNumber = "LAPTOP-001",
                Status = "Available"
            };

            await _repository.Add(instance);

            Assert.IsTrue(instance.Id > 0);
            Assert.AreEqual(1, (await _repository.GetAll()).Count());
        }

        [TestMethod]
        public async Task GetById_ShouldReturnCorrectInstance()
        {
            var instance = new ItemInstance
            {
                ItemId = 1,
                SerialNumber = "LAPTOP-002",
                Status = "Available"
            };

            await _repository.Add(instance);

            var result = await _repository.GetById(instance.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual("LAPTOP-002", result.SerialNumber);
            Assert.AreEqual(1, result.ItemId);
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
            var instance = new ItemInstance
            {
                ItemId = 1,
                SerialNumber = "DELETE-001"
            };

            await _repository.Add(instance);
            await _repository.Delete(instance.Id);

            Assert.AreEqual(0, (await _repository.GetAll()).Count());
            Assert.IsNull(await _repository.GetById(instance.Id));
        }

        [TestMethod]
        public async Task GetByItemId_ShouldReturnCorrectInstances()
        {
            await _repository.Add(new ItemInstance { ItemId = 1, SerialNumber = "A001" });
            await _repository.Add(new ItemInstance { ItemId = 1, SerialNumber = "A002" });
            await _repository.Add(new ItemInstance { ItemId = 2, SerialNumber = "B001" });

            var results = await _repository.GetByItemId(1);

            Assert.AreEqual(2, results.Count());
        }

        [TestMethod]
        public async Task GetByItemId_ShouldShowCorrectCountAfterAddingInstance()
        {
            await _repository.Add(new ItemInstance { ItemId = 1, SerialNumber = "LAPTOP-001" });

            var instances = await _repository.GetByItemId(1);

            Assert.AreEqual(1, instances.Count());
        }
    }
}
