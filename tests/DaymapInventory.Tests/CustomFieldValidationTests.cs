using DaymapInventory.Data;
using DaymapInventory.Models;
using DaymapInventory.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DaymapInventory.Tests
{
    [TestClass]
    public class CustomFieldValidationTests
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
        public async Task Create_Should_Allow_Unique_FieldNames_For_Same_Item()
        {
            await _repository.Add(new CustomField { Name = "Brand", ItemId = 1 });
            await _repository.Add(new CustomField { Name = "Model", ItemId = 1 });

            var fields = await _repository.GetByItemId(1);
            Assert.AreEqual(2, fields.Count());
        }

        [TestMethod]
        public async Task Update_Should_Propagate_FieldName_To_All_Values()
        {
            var field = new CustomField { Name = "Colour", ItemId = 1 };
            await _repository.Add(field);

            _context.CustomFieldValues.Add(new CustomFieldValue { CustomFieldId = field.Id, ItemId = 1, Value = "Red" });
            _context.CustomFieldValues.Add(new CustomFieldValue { CustomFieldId = field.Id, ItemId = 1, Value = "Blue" });
            await _context.SaveChangesAsync();

            field.Name = "Color";
            await _repository.Update(field);

            var updated = await _repository.GetById(field.Id);
            Assert.AreEqual("Color", updated!.Name);

            // All values still reference the same field — name change reflects automatically via FK
            var values = _context.CustomFieldValues
                .Include(v => v.CustomField)
                .Where(v => v.CustomFieldId == field.Id)
                .ToList();

            Assert.IsTrue(values.All(v => v.CustomField!.Name == "Color"));
        }
    }
}
