using DaymapInventory.Data;
using DaymapInventory.Interfaces;
using DaymapInventory.Models;
using DaymapInventory.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaymapInventory.Tests
{
    [TestClass]
    public class SqlCustomFieldRepositoryTests
    {
        private AppDbContext _context = null!;
        private ICustomFieldRepository _repository = null!;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _repository = new SqlCustomFieldRepository(_context);

            // Seed a parent item required by the CustomField FK
            _context.Items.Add(new Item { Id = 1, Name = "Laptop" });
            _context.SaveChanges();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        [TestMethod]
        public async Task Add_ShouldStoreCustomField()
        {
            var field = new CustomField { Name = "Serial Number", ItemId = 1 };

            await _repository.Add(field);

            Assert.AreEqual(1, (await _repository.GetAll()).Count());
        }

        [TestMethod]
        public async Task GetById_ShouldReturnCorrectField()
        {
            var field = new CustomField { Name = "Brand", ItemId = 1, ControlType = "Text", DataType = "String" };
            await _repository.Add(field);

            var result = await _repository.GetById(field.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual("Brand", result.Name);
            Assert.AreEqual("Text", result.ControlType);
        }

        [TestMethod]
        public async Task GetById_ShouldReturnNullWhenNotFound()
        {
            var result = await _repository.GetById(999);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task Update_ShouldModifyFieldName()
        {
            var field = new CustomField { Name = "Old Name", ItemId = 1 };
            await _repository.Add(field);

            field.Name = "New Name";
            await _repository.Update(field);

            var result = await _repository.GetById(field.Id);
            Assert.AreEqual("New Name", result!.Name);
        }

        [TestMethod]
        public async Task Delete_ShouldRemoveField()
        {
            var field = new CustomField { Name = "Warranty", ItemId = 1 };
            await _repository.Add(field);

            await _repository.Delete(field.Id);

            Assert.IsNull(await _repository.GetById(field.Id));
            Assert.AreEqual(0, (await _repository.GetAll()).Count());
        }

        [TestMethod]
        public async Task GetByItemId_ShouldReturnOnlyFieldsForThatItem()
        {
            _context.Items.Add(new Item { Id = 2, Name = "Projector" });
            await _context.SaveChangesAsync();

            await _repository.Add(new CustomField { Name = "Brand", ItemId = 1 });
            await _repository.Add(new CustomField { Name = "Model", ItemId = 1 });
            await _repository.Add(new CustomField { Name = "Resolution", ItemId = 2 });

            var results = await _repository.GetByItemId(1);

            Assert.AreEqual(2, results.Count());
            Assert.IsTrue(results.All(f => f.ItemId == 1));
        }

        [TestMethod]
        public async Task Delete_ShouldCascadeToCustomFieldValues()
        {
            var field = new CustomField { Name = "Purchase Date", ItemId = 1 };
            await _repository.Add(field);

            _context.CustomFieldValues.Add(new CustomFieldValue { CustomFieldId = field.Id, ItemId = 1, Value = "2024-01-01" });
            await _context.SaveChangesAsync();

            await _repository.Delete(field.Id);

            var remainingValues = _context.CustomFieldValues.Where(v => v.CustomFieldId == field.Id).ToList();
            Assert.AreEqual(0, remainingValues.Count);
        }
    }
}
