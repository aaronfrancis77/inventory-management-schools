using DaymapInventory.Interfaces;
using DaymapInventory.Models;
using DaymapInventory.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaymapInventory.Tests
{
    [TestClass]
    public class InMemoryTransactionRepositoryTests
    {
        private ITransactionRepository _repository;

        [TestInitialize]
        public void Setup()
        {
            _repository = new InMemoryTransactionRepository();
        }

        [TestMethod]
        public void Add_ShouldLogTransaction()
        {
            var transaction = new Transaction
            {
                ItemId = 1,
                Type = TransactionType.Loan.ToString(),
                QuantityChanged = -1,
                PerformedBy = "teacher.jones"
            };

            _repository.Add(transaction);

            Assert.AreEqual(1, _repository.GetAll().Count());
            Assert.AreEqual(1, transaction.Id);
        }

        [TestMethod]
        public void GetByItemId_ShouldReturnItemTransactions()
        {
            _repository.Add(new Transaction { ItemId = 1, Type = "Loan", QuantityChanged = -1, PerformedBy = "user1" });
            _repository.Add(new Transaction { ItemId = 2, Type = "Loan", QuantityChanged = -1, PerformedBy = "user2" });
            _repository.Add(new Transaction { ItemId = 1, Type = "Return", QuantityChanged = 1, PerformedBy = "user1" });

            var itemTransactions = _repository.GetByItemId(1);

            Assert.AreEqual(2, itemTransactions.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Update_ShouldThrowBecauseAppendOnly()
        {
            var transaction = new Transaction
            {
                ItemId = 1,
                Type = "Loan",
                QuantityChanged = -1,
                PerformedBy = "user1"
            };
            _repository.Add(transaction);

            // Transactions are immutable — this must throw
            _repository.Update(transaction);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Delete_ShouldThrowBecauseAuditLog()
        {
            var transaction = new Transaction
            {
                ItemId = 1,
                Type = "Loan",
                QuantityChanged = -1,
                PerformedBy = "user1"
            };
            _repository.Add(transaction);

            // Transactions can't be deleted — this must throw
            _repository.Delete(transaction.Id);
        }
    }
}
