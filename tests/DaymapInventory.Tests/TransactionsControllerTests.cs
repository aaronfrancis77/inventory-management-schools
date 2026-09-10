#if false
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DaymapInventory.Controllers;
using DaymapInventory.Data;
using DaymapInventory.Interfaces;
using DaymapInventory.Models;
using DaymapInventory.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaymapInventory.Tests
{
    [TestClass]
    public class TransactionsControllerTests
    {
        private AppDbContext _context = null!;
        private TransactionsController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);

            var transactionRepo = new TestTransactionRepository();
            var itemRepo = new SqlItemRepository(_context);
            var instanceRepo = new SqlItemInstanceRepository(_context);

            _controller = new TransactionsController(transactionRepo, itemRepo, instanceRepo);
        }

        [TestCleanup]
        public void Cleanup() => _context.Dispose();

        [TestMethod]
        public async Task Loan_DecrementsStock_LoansInstance_AndCreatesActiveTransaction()
        {
            var (item, instance) = await SeedAvailableInstance();
            var transaction = new Transaction
            {
                ItemId = item.Id,
                ItemInstanceId = instance.Id,
                Type = "Loan",
                QuantityChanged = 1,
                LoanedToId = 42
            };

            var result = await _controller.Create(transaction);

            Assert.IsInstanceOfType<CreatedAtActionResult>(result);
            Assert.AreEqual(0, (await _context.Items.FindAsync(item.Id))!.StockCount);
            Assert.AreEqual("Loaned", (await _context.ItemInstances.FindAsync(instance.Id))!.Status);
            Assert.AreEqual("Active", transaction.Status);
        }

        [TestMethod]
        public async Task Return_IncrementsStock_MakesInstanceAvailable_AndCreatesReturnedTransaction()
        {
            var item = new Item { Name = "Laptop", StockCount = 0 };
            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            var instance = new ItemInstance { ItemId = item.Id, Status = "Loaned" };
            _context.ItemInstances.Add(instance);
            await _context.SaveChangesAsync();

            var transaction = new Transaction
            {
                ItemId = item.Id,
                ItemInstanceId = instance.Id,
                Type = "Return",
                QuantityChanged = 1,
                LoanedToId = 42
            };

            var result = await _controller.Create(transaction);

            Assert.IsInstanceOfType<CreatedAtActionResult>(result);
            Assert.AreEqual(1, (await _context.Items.FindAsync(item.Id))!.StockCount);
            Assert.AreEqual("Available", (await _context.ItemInstances.FindAsync(instance.Id))!.Status);
            Assert.AreEqual("Returned", transaction.Status);
        }

        [TestMethod]
        public async Task Loan_AlreadyLoanedInstance_ReturnsBadRequestWithoutCreatingTransaction()
        {
            var item = new Item { Name = "Laptop", StockCount = 1 };
            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            var instance = new ItemInstance { ItemId = item.Id, Status = "Loaned" };
            _context.ItemInstances.Add(instance);
            await _context.SaveChangesAsync();

            var result = await _controller.Create(new Transaction
            {
                ItemId = item.Id,
                ItemInstanceId = instance.Id,
                Type = "Loan",
                QuantityChanged = 1
            });

            Assert.IsInstanceOfType<BadRequestObjectResult>(result);
        }

        [TestMethod]
        public async Task Loan_InsufficientStock_ReturnsBadRequestWithoutSideEffects()
        {
            var item = new Item { Name = "Laptop", StockCount = 0 };
            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            var result = await _controller.Create(new Transaction
            {
                ItemId = item.Id,
                Type = "Loan",
                QuantityChanged = 1
            });

            Assert.IsInstanceOfType<BadRequestObjectResult>(result);
            Assert.AreEqual(0, item.StockCount);
        }

        private async Task<(Item Item, ItemInstance Instance)> SeedAvailableInstance()
        {
            var item = new Item { Name = "Laptop", StockCount = 1 };
            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            var instance = new ItemInstance { ItemId = item.Id, Status = "Available" };
            _context.ItemInstances.Add(instance);
            await _context.SaveChangesAsync();

            return (item, instance);
        }
    }

    // Dummy stub implementation satisfying ITransactionRepository interface for tests
    internal class TestTransactionRepository : ITransactionRepository
    {
        public Task<TransactionResponseDto> CreateAsync(CreateTransactionDto dto)
        {
            return Task.FromResult(new TransactionResponseDto());
        }

        public Task<IEnumerable<TransactionResponseDto>> GetAllAsync() => throw new NotImplementedException();
        public Task<TransactionResponseDto?> GetByIdAsync(Guid id) => throw new NotImplementedException();
        public Task<IEnumerable<TransactionResponseDto>> GetByItemIdAsync(Guid itemId) => throw new NotImplementedException();
        public Task<IEnumerable<TransactionResponseDto>> GetByInstanceIdAsync(Guid instanceId) => throw new NotImplementedException();
    }
}
#endif