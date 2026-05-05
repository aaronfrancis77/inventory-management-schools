using DaymapInventory.Interfaces;
using DaymapInventory.Models;
using DaymapInventory.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaymapInventory.Tests
{
    [TestClass]
    public class InMemoryTransactionRepositoryTests
    {
        private ITransactionRepository _repository = null!;

        [TestInitialize]
        public void Setup()
        {
            _repository = new InMemoryTransactionRepository();
        }

        [TestMethod]
        public async Task Add_ShouldLogTransaction()
        {
            var transaction = new Transaction
            {
                ItemId = 1,
                Type = TransactionType.Loan.ToString(),
                QuantityChanged = -1,
                PerformedBy = "teacher.aaron"
            };

            await _repository.Add(transaction);

            Assert.AreEqual(1, (await _repository.GetAll()).Count());
        }

        [TestMethod]
        public async Task GetByItemId_ShouldReturnItemTransactions()
        {
            await _repository.Add(new Transaction { ItemId = 1, Type = "Loan", QuantityChanged = -1, PerformedBy = "user1" });
            await _repository.Add(new Transaction { ItemId = 2, Type = "Loan", QuantityChanged = -1, PerformedBy = "user2" });
            await _repository.Add(new Transaction { ItemId = 1, Type = "Return", QuantityChanged = 1, PerformedBy = "user1" });

            var itemTransactions = await _repository.GetByItemId(1);

            Assert.AreEqual(2, itemTransactions.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task Update_ShouldThrowBecauseAppendOnly()
        {
            var transaction = new Transaction
            {
                ItemId = 1,
                Type = "Loan",
                QuantityChanged = -1,
                PerformedBy = "user1"
            };
            await _repository.Add(transaction);

            await _repository.Update(transaction);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public async Task Delete_ShouldThrowBecauseAuditLog()
        {
            var transaction = new Transaction
            {
                ItemId = 1,
                Type = "Loan",
                QuantityChanged = -1,
                PerformedBy = "user1"
            };
            await _repository.Add(transaction);

            await _repository.Delete(transaction.Id);
        }
    }
}
