#if false
using DaymapInventory.Controllers;
using DaymapInventory.Data;
using DaymapInventory.Interfaces;
using DaymapInventory.Models;
using DaymapInventory.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaymapInventory.Tests
{
    [TestClass]
    public class CommentsControllerTests
    {
        private AppDbContext _context = null!;
        private ICommentRepository _commentRepository = null!;
        private IItemRepository _itemRepository = null!;
        private CommentsController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _context = TestDbContextFactory.CreateInMemoryContext();
            _commentRepository = new SqlCommentRepository(_context);
            _itemRepository = new SqlItemRepository(_context);
            _controller = new CommentsController(_commentRepository, _itemRepository);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        // Test methods go here once Shreyas's CommentsController
        // implementation lands (SCRUM 151 to 154), and depends on
        // Antonio's repository being in place first.
        // Remove the #if false / #endif above and below once that code exists.
    }
}
#endif