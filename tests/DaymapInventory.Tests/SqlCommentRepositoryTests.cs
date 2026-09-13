#if false
using DaymapInventory.Data;
using DaymapInventory.Interfaces;
using DaymapInventory.Models;
using DaymapInventory.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaymapInventory.Tests
{
    [TestClass]
    public class SqlCommentRepositoryTests
    {
        private AppDbContext _context = null!;
        private ICommentRepository _repository = null!;

        [TestInitialize]
        public void Setup()
        {
            _context = TestDbContextFactory.CreateInMemoryContext();
            _repository = new SqlCommentRepository(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        // Test methods go here once Antonio's Comment model and
        // SqlCommentRepository implementation lands (SCRUM 144 to 148).
        // Remove the #if false / #endif above and below once that code exists.
    }
}
#endif