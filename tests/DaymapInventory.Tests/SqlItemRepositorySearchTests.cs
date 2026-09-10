using DaymapInventory.Data;
using DaymapInventory.Interfaces;
using DaymapInventory.Models;
using DaymapInventory.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaymapInventory.Tests
{
    [TestClass]
    public class SqlItemRepositorySearchTests
    {
        private AppDbContext _context = null!;
        private IItemRepository _repository = null!;

        [TestInitialize]
        public void Setup()
        {
            _context = TestDbContextFactory.CreateInMemoryContext();
            _repository = new SqlItemRepository(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        // Test methods go here once Search implementation
        // lands (SCRUM 139, 140).
    }
}