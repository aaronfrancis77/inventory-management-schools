using DaymapInventory.Data;
using DaymapInventory.Interfaces;
using DaymapInventory.Models;
using DaymapInventory.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaymapInventory.Tests
{
    [TestClass]
    public class SqlTagRepositoryTests
    {
        private AppDbContext _context = null!;
        private ITagRepository _repository = null!;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _repository = new SqlTagRepository(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        [TestMethod]
        public async Task Add_ShouldStoreTag()
        {
            var tag = new Tag { Name = "Science", Colour = "#ff0000", IsDefault = false };

            await _repository.Add(tag);

            Assert.AreEqual(1, (await _repository.GetAll()).Count());
        }

        [TestMethod]
        public async Task Add_ShouldSetCreatedAtAutomatically()
        {
            var tag = new Tag { Name = "History", Colour = "#00ff00", IsDefault = false };

            await _repository.Add(tag);

            Assert.AreNotEqual(default(DateTime), tag.CreatedAt);
        }

        [TestMethod]
        public async Task GetById_ShouldReturnCorrectTag()
        {
            var tag = new Tag { Name = "Maths", Colour = "#0000ff", IsDefault = false };
            await _repository.Add(tag);

            var result = await _repository.GetById(tag.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual("Maths", result.Name);
            Assert.AreEqual("#0000ff", result.Colour);
        }

        [TestMethod]
        public async Task GetById_ShouldReturnNullWhenNotFound()
        {
            var result = await _repository.GetById(999);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task Update_ShouldModifyTagFields()
        {
            var tag = new Tag { Name = "Old Name", Colour = "#111111", IsDefault = false };
            await _repository.Add(tag);

            tag.Name = "New Name";
            tag.Colour = "#222222";
            await _repository.Update(tag);

            var result = await _repository.GetById(tag.Id);
            Assert.AreEqual("New Name", result!.Name);
            Assert.AreEqual("#222222", result.Colour);
        }

        [TestMethod]
        public async Task Delete_ShouldRemoveTag()
        {
            var tag = new Tag { Name = "Temporary", Colour = "#333333", IsDefault = false };
            await _repository.Add(tag);

            await _repository.Delete(tag.Id);

            Assert.IsNull(await _repository.GetById(tag.Id));
            Assert.AreEqual(0, (await _repository.GetAll()).Count());
        }

        [TestMethod]
        public async Task Delete_NonExistentTag_ShouldNotThrow()
        {
            // SqlTagRepository.Delete checks for null before removing,
            // so this should complete without error rather than throw
            await _repository.Delete(999);

            Assert.AreEqual(0, (await _repository.GetAll()).Count());
        }

        [TestMethod]
        public async Task GetByName_ShouldReturnMatchingTag()
        {
            var tag = new Tag { Name = "Geography", Colour = "#444444", IsDefault = false };
            await _repository.Add(tag);

            var result = await _repository.GetByName("Geography");

            Assert.IsNotNull(result);
            Assert.AreEqual(tag.Id, result.Id);
        }

        [TestMethod]
        public async Task GetByName_ShouldReturnNullWhenNoMatch()
        {
            var result = await _repository.GetByName("Nonexistent");

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetDefaults_ShouldReturnOnlyDefaultTags()
        {
            await _repository.Add(new Tag { Name = "Default One", Colour = "#555555", IsDefault = true });
            await _repository.Add(new Tag { Name = "Default Two", Colour = "#666666", IsDefault = true });
            await _repository.Add(new Tag { Name = "Custom", Colour = "#777777", IsDefault = false });

            var results = await _repository.GetDefaults();

            Assert.AreEqual(2, results.Count());
            Assert.IsTrue(results.All(t => t.IsDefault));
        }


        [TestMethod]
        public async Task AssignToItem_ShouldCreateItemTagLink()
        {
            var item = new Item { Name = "Laptop" };
            var tag = new Tag { Name = "Electronics" };
            _context.Items.Add(item);
            await _repository.Add(tag);
            await _context.SaveChangesAsync();

            await _repository.AssignToItem(item.Id, tag.Id);

            Assert.IsTrue(await _repository.IsAssignedToItem(item.Id, tag.Id));
        }

        [TestMethod]
        public async Task IsAssignedToItem_ShouldReturnFalseWhenNotAssigned()
        {
            var result = await _repository.IsAssignedToItem(1, 1);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task RemoveFromItem_ShouldDeleteItemTagLink()
        {
            var item = new Item { Name = "Projector" };
            var tag = new Tag { Name = "AV Equipment" };
            _context.Items.Add(item);
            await _repository.Add(tag);
            await _context.SaveChangesAsync();
            await _repository.AssignToItem(item.Id, tag.Id);

            await _repository.RemoveFromItem(item.Id, tag.Id);

            Assert.IsFalse(await _repository.IsAssignedToItem(item.Id, tag.Id));
        }

        [TestMethod]
        public async Task RemoveFromItem_NonExistentLink_ShouldNotThrow()
        {
            await _repository.RemoveFromItem(999, 999);

            Assert.IsFalse(await _repository.IsAssignedToItem(999, 999));
        }

        [TestMethod]
        public async Task GetTagsForItem_ShouldReturnOnlyAssignedTags()
        {
            var item = new Item { Name = "Whiteboard" };
            var tagA = new Tag { Name = "Classroom" };
            var tagB = new Tag { Name = "Unassigned" };
            _context.Items.Add(item);
            await _repository.Add(tagA);
            await _repository.Add(tagB);
            await _context.SaveChangesAsync();
            await _repository.AssignToItem(item.Id, tagA.Id);

            var results = await _repository.GetTagsForItem(item.Id);

            Assert.AreEqual(1, results.Count());
            Assert.AreEqual("Classroom", results.First().Name);
        }
    }
}