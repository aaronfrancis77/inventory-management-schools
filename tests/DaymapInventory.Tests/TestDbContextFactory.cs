using DaymapInventory.Data;
using Microsoft.EntityFrameworkCore;

namespace DaymapInventory.Tests
{
    // Shared helper for Sprint 5 test files (SqlItemRepositoryExpiryTests,
    // SqlItemRepositorySearchTests, SqlCommentRepositoryTests, CommentsControllerTests).
    // Follows the same in memory setup pattern used in SqlTagRepositoryTests
    // (Guid.NewGuid database name, so each test gets an isolated database).
    public static class TestDbContextFactory
    {
        public static AppDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }
    }
}