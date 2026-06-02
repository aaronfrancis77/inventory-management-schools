using DaymapInventory.Data;
using DaymapInventory.Models;
using DaymapInventory.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaymapInventory.Tests
{
    [TestClass]
    public class CustomFieldCascadeTests
    {
        private AppDbContext _context = null!;
        private SqlCustomFieldRepository _repository = null!;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
            _repository = new SqlCustomFieldRepository(_context);

            _context.Items.Add(new Item { Id = 1, Name = "Laptop" });
            _context.SaveChanges();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        [TestMethod]
        public async Task Delete_Field_Should_Remove_Associated_Values()
        {
            var field = new CustomField { Name = "Serial Number", ItemId = 1 };
            await _repository.Add(field);

            _context.CustomFieldValues.Add(new CustomFieldValue { CustomFieldId = field.Id, ItemId = 1, Value = "SN-001" });
            _context.CustomFieldValues.Add(new CustomFieldValue { CustomFieldId = field.Id, ItemId = 1, Value = "SN-002" });
            await _context.SaveChangesAsync();

            await _repository.Delete(field.Id);

            var remainingValues = _context.CustomFieldValues
                .Where(v => v.CustomFieldId == field.Id)
                .ToList();

            Assert.AreEqual(0, remainingValues.Count);
            Assert.IsNull(await _repository.GetById(field.Id));
        }

        [TestMethod]
        public async Task Delete_Field_Should_Not_Affect_Other_Fields()
        {
            var field1 = new CustomField { Name = "Brand", ItemId = 1 };
            var field2 = new CustomField { Name = "Model", ItemId = 1 };
            await _repository.Add(field1);
            await _repository.Add(field2);

            _context.CustomFieldValues.Add(new CustomFieldValue { CustomFieldId = field1.Id, ItemId = 1, Value = "Dell" });
            _context.CustomFieldValues.Add(new CustomFieldValue { CustomFieldId = field2.Id, ItemId = 1, Value = "XPS 15" });
            await _context.SaveChangesAsync();

            await _repository.Delete(field1.Id);

            var remainingValues = _context.CustomFieldValues
                .Where(v => v.CustomFieldId == field2.Id)
                .ToList();

            Assert.AreEqual(1, remainingValues.Count);
            Assert.AreEqual("XPS 15", remainingValues[0].Value);
        }
    }
}
