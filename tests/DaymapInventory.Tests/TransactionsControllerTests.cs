using DaymapInventory.Controllers;
using DaymapInventory.Data;
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
            _controller = new TransactionsController(
                new SqlTransactionRepository(_context),
                new SqlItemRepository(_context),
                new SqlItemInstanceRepository(_context));
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
            Assert.AreEqual(1, await _context.Transactions.CountAsync());
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
            Assert.AreEqual(0, await _context.Transactions.CountAsync());
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
            Assert.AreEqual(0, await _context.Transactions.CountAsync());
        }

        private async Task<(Item Item, ItemInstance Instance)> SeedAvailableInstance()
        {
            var item = new Item { Name = "Laptop", StockCount = 0 };
            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            var instance = new ItemInstance { ItemId = item.Id, Status = "Available" };
            await new SqlItemInstanceRepository(_context).Add(instance);
            return (item, instance);
        }
    }
}
