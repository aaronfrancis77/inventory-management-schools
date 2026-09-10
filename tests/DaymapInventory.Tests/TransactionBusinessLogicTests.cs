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
    public class TransactionBusinessLogicTests
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
        public async Task Loan_ShouldDecrementStock()
        {
            var (item, instance) = await SeedAvailableInstance();

            var result = await _controller.Create(new Transaction
            {
                ItemId = item.Id,
                ItemInstanceId = instance.Id,
                Type = "Loan",
                QuantityChanged = 1,
                LoanedToId = 42
            });

            Assert.IsInstanceOfType<CreatedAtActionResult>(result);
            Assert.AreEqual(0, (await _context.Items.FindAsync(item.Id))!.StockCount);
        }

        [TestMethod]
        public async Task Return_ShouldIncrementStock()
        {
            var item = new Item { Name = "Laptop", StockCount = 0 };
            _context.Items.Add(item);
            await _context.SaveChangesAsync();
            var instance = new ItemInstance { ItemId = item.Id, Status = "Loaned" };
            _context.ItemInstances.Add(instance);
            await _context.SaveChangesAsync();

            var result = await _controller.Create(new Transaction
            {
                ItemId = item.Id,
                ItemInstanceId = instance.Id,
                Type = "Return",
                QuantityChanged = 1,
                LoanedToId = 42
            });

            Assert.IsInstanceOfType<CreatedAtActionResult>(result);
            Assert.AreEqual(1, (await _context.Items.FindAsync(item.Id))!.StockCount);
        }

        [TestMethod]
        public async Task Loan_OnAlreadyLoanedInstance_ShouldBeRejected()
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
        public async Task Loan_ShouldNotAllowStockBelowZero()
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

        [TestMethod]
        public async Task Create_InvalidType_ShouldBeRejected()
        {
            var item = new Item { Name = "Laptop", StockCount = 1 };
            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            var result = await _controller.Create(new Transaction
            {
                ItemId = item.Id,
                Type = "Sell",
                QuantityChanged = 1
            });

            Assert.IsInstanceOfType<BadRequestObjectResult>(result);
        }

        [TestMethod]
        public async Task Create_MissingItem_ShouldReturnNotFound()
        {
            var result = await _controller.Create(new Transaction
            {
                ItemId = 999,
                Type = "Loan",
                QuantityChanged = 1
            });

            Assert.IsInstanceOfType<NotFoundObjectResult>(result);
        }

        [TestMethod]
        public async Task Loan_InstanceNotBelongingToItem_ShouldBeRejected()
        {
            var itemA = new Item { Name = "Laptop", StockCount = 1 };
            var itemB = new Item { Name = "Projector", StockCount = 1 };
            _context.Items.AddRange(itemA, itemB);
            await _context.SaveChangesAsync();
            var instance = new ItemInstance { ItemId = itemB.Id, Status = "Available" };
            _context.ItemInstances.Add(instance);
            await _context.SaveChangesAsync();

            var result = await _controller.Create(new Transaction
            {
                ItemId = itemA.Id,
                ItemInstanceId = instance.Id,
                Type = "Loan",
                QuantityChanged = 1
            });

            Assert.IsInstanceOfType<BadRequestObjectResult>(result);
        }

        [TestMethod]
        public async Task Loan_WithInstance_QuantityChangedIsIgnoredForActualStockUpdate()
{
    // Documents current behaviour: when an ItemInstanceId is provided, the
    // actual stock count comes from SyncStockCount (recounting available
    // instances), not from QuantityChanged. The "stock cannot go below 0"
    // check uses QuantityChanged, but the real outcome can differ from what
    // was validated. Worth raising with the team: QuantityChanged arguably
    // should be constrained to 1 whenever an instance is provided.

    var item = new Item { Name = "Laptop" };
    _context.Items.Add(item);
    await _context.SaveChangesAsync();

    var instanceRepo = new SqlItemInstanceRepository(_context);
    var loanedInstance = new ItemInstance { ItemId = item.Id, Status = "Available" };
    await instanceRepo.Add(loanedInstance);
    await instanceRepo.Add(new ItemInstance { ItemId = item.Id, Status = "Available" });
    await instanceRepo.Add(new ItemInstance { ItemId = item.Id, Status = "Available" });
    // Three available instances. StockCount is now synced to 3 by the Add calls above.

    var result = await _controller.Create(new Transaction
    {
        ItemId = item.Id,
        ItemInstanceId = loanedInstance.Id,
        Type = "Loan",
        QuantityChanged = 2, // passes the "below zero" check (3 - 2 = 1), but does not match the real outcome
        LoanedToId = 42
    });

    Assert.IsInstanceOfType<CreatedAtActionResult>(result);
    // Only 1 of the 3 available instances was loaned, so 2 remain available.
    // The real result (2) does not match what QuantityChanged (2) would imply (1).
    Assert.AreEqual(2, (await _context.Items.FindAsync(item.Id))!.StockCount);
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