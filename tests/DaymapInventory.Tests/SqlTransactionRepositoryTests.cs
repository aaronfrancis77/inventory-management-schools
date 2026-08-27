using DaymapInventory.Data;
using DaymapInventory.Interfaces;
using DaymapInventory.Models;
using DaymapInventory.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaymapInventory.Tests
{
    [TestClass]
    public class SqlTransactionRepositoryTests
    {
        private AppDbContext _context = null!;
        private ITransactionRepository _repository = null!;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _repository = new SqlTransactionRepository(_context);

            // Seed a parent item required by the Transaction FK
            _context.Items.Add(new Item { Id = 1, Name = "Laptop" });
            _context.SaveChanges();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        [TestMethod]
        public async Task Add_ShouldStoreTransaction()
        {
            var transaction = new Transaction
            {
                ItemId = 1,
                Type = "Loan",
                QuantityChanged = 1,
                Status = "Active",
                LoanedToId = 42
            };

            await _repository.Add(transaction);

            Assert.AreEqual(1, (await _repository.GetAll()).Count());
        }

        [TestMethod]
        public async Task Add_ShouldSetCreatedAtAutomatically()
        {
            var transaction = new Transaction
            {
                ItemId = 1,
                Type = "Loan",
                QuantityChanged = 1,
                Status = "Active"
            };

            await _repository.Add(transaction);

            Assert.AreNotEqual(default(DateTime), transaction.CreatedAt);
        }

        [TestMethod]
        public async Task GetByItemId_ShouldReturnOnlyTransactionsForThatItem()
        {
            _context.Items.Add(new Item { Id = 2, Name = "Projector" });
            await _context.SaveChangesAsync();

            await _repository.Add(new Transaction { ItemId = 1, Type = "Loan", QuantityChanged = 1, Status = "Active" });
            await _repository.Add(new Transaction { ItemId = 1, Type = "Return", QuantityChanged = 1, Status = "Returned" });
            await _repository.Add(new Transaction { ItemId = 2, Type = "Loan", QuantityChanged = 1, Status = "Active" });

            var results = await _repository.GetByItemId(1);

            Assert.AreEqual(2, results.Count());
            Assert.IsTrue(results.All(t => t.ItemId == 1));
        }

        [TestMethod]
        public async Task GetByItemInstanceId_ShouldReturnOnlyTransactionsForThatInstance()
        {
            await _repository.Add(new Transaction { ItemId = 1, ItemInstanceId = 10, Type = "Loan", QuantityChanged = 1, Status = "Active" });
            await _repository.Add(new Transaction { ItemId = 1, ItemInstanceId = 11, Type = "Loan", QuantityChanged = 1, Status = "Active" });

            var results = await _repository.GetByItemInstanceId(10);

            Assert.AreEqual(1, results.Count());
            Assert.AreEqual(10, results.First().ItemInstanceId);
        }

        [TestMethod]
        public async Task GetByType_ShouldReturnOnlyMatchingType()
        {
            await _repository.Add(new Transaction { ItemId = 1, Type = "Loan", QuantityChanged = 1, Status = "Active" });
            await _repository.Add(new Transaction { ItemId = 1, Type = "Return", QuantityChanged = 1, Status = "Returned" });

            var results = await _repository.GetByType("Loan");

            Assert.AreEqual(1, results.Count());
            Assert.AreEqual("Loan", results.First().Type);
        }

        [TestMethod]
        public async Task Update_ShouldThrowInvalidOperationException()
        {
            var transaction = new Transaction { ItemId = 1, Type = "Loan", QuantityChanged = 1, Status = "Active" };
            await _repository.Add(transaction);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _repository.Update(transaction));
        }

        [TestMethod]
        public async Task Delete_ShouldThrowInvalidOperationException()
        {
            var transaction = new Transaction { ItemId = 1, Type = "Loan", QuantityChanged = 1, Status = "Active" };
            await _repository.Add(transaction);

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                () => _repository.Delete(transaction.Id));
        }
    }
}