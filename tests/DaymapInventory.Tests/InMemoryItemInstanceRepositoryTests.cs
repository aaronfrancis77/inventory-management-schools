using System.Linq;
using DaymapInventory.Interfaces;
using DaymapInventory.Models;
using DaymapInventory.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaymapInventory.Tests
{
    [TestClass]
    public class InMemoryItemInstanceRepositoryTests
    {
        private IItemInstanceRepository _repository;

        [TestInitialize]
        public void Setup()
        {
            _repository = new InMemoryItemInstanceRepository();
        }

        [TestMethod]
        public void Add_ShouldAssignIdAndStore()
        {
            var instance = new ItemInstance
            {
                ItemId = 1,
                SerialNumber = "LAPTOP-001",
                Status = "Available"
            };

            _repository.Add(instance);

            Assert.IsTrue(instance.Id > 0);
            Assert.AreEqual(1, _repository.GetAll().Count());
        }

        [TestMethod]
        public void GetById_ShouldReturnCorrectInstance()
        {
            var instance = new ItemInstance
            {
                ItemId = 1,
                SerialNumber = "LAPTOP-002",
                Status = "Available"
            };

            _repository.Add(instance);

            var result = _repository.GetById(instance.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual("LAPTOP-002", result.SerialNumber);
            Assert.AreEqual(1, result.ItemId);
        }

        [TestMethod]
        public void GetById_ShouldReturnNullWhenNotFound()
        {
            var result = _repository.GetById(999);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Delete_ShouldRemoveInstance()
        {
            var instance = new ItemInstance
            {
                ItemId = 1,
                SerialNumber = "DELETE-001"
            };

            _repository.Add(instance);

            _repository.Delete(instance.Id);

            Assert.AreEqual(0, _repository.GetAll().Count());
            Assert.IsNull(_repository.GetById(instance.Id));
        }

        [TestMethod]
        public void GetByItemId_ShouldReturnCorrectInstances()
        {
            _repository.Add(new ItemInstance { ItemId = 1, SerialNumber = "A001" });
            _repository.Add(new ItemInstance { ItemId = 1, SerialNumber = "A002" });
            _repository.Add(new ItemInstance { ItemId = 2, SerialNumber = "B001" });

            var results = _repository.GetByItemId(1);

            Assert.AreEqual(2, results.Count());
        }
    }
}