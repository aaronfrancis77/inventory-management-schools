using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace InventorySystem.Tests
{
    public class CustomField
    {
        public int Id { get; set; }

        // Fixes CS8618 warning
        public string Name { get; set; } = string.Empty;
    }

    public class SqlCustomFieldRepository
    {
        private readonly List<CustomField> _fields = new();

        public void Create(CustomField field)
        {
            _fields.Add(field);
        }

        // Fixes CS8603 warning
        public CustomField? GetById(int id)
        {
            return _fields.Find(f => f.Id == id);
        }

        public void Update(CustomField updatedField)
        {
            var field = GetById(updatedField.Id);

            if (field != null)
            {
                field.Name = updatedField.Name;
            }
        }

        public void Delete(int id)
        {
            var field = GetById(id);

            if (field != null)
            {
                _fields.Remove(field);
            }
        }

        public List<CustomField> GetAll()
        {
            return _fields;
        }
    }

    [TestClass]
    public class SqlCustomFieldRepositoryTests
    {
        // Fixes CS8618 warning
        private SqlCustomFieldRepository _repository = null!;

        [TestInitialize]
        public void Setup()
        {
            _repository = new SqlCustomFieldRepository();
        }

        [TestMethod]
        public void Create_Should_Add_CustomField()
        {
            var field = new CustomField
            {
                Id = 1,
                Name = "Laptop Serial"
            };

            _repository.Create(field);

            var result = _repository.GetById(1);

            Assert.IsNotNull(result);
            Assert.AreEqual("Laptop Serial", result.Name);
        }

        [TestMethod]
        public void GetById_Should_Return_Field()
        {
            var field = new CustomField
            {
                Id = 2,
                Name = "Room Number"
            };

            _repository.Create(field);

            var result = _repository.GetById(2);

            Assert.IsNotNull(result);
            Assert.AreEqual("Room Number", result.Name);
        }

        [TestMethod]
        public void Update_Should_Modify_Field()
        {
            var field = new CustomField
            {
                Id = 3,
                Name = "Old Name"
            };

            _repository.Create(field);

            field.Name = "Updated Name";
            _repository.Update(field);

            var updatedField = _repository.GetById(3);

            Assert.IsNotNull(updatedField);
            Assert.AreEqual("Updated Name", updatedField.Name);
        }

        [TestMethod]
        public void Delete_Should_Remove_Field()
        {
            var field = new CustomField
            {
                Id = 4,
                Name = "Delete Test"
            };

            _repository.Create(field);

            _repository.Delete(4);

            var result = _repository.GetById(4);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetAll_Should_Return_All_Fields()
        {
            var field1 = new CustomField
            {
                Id = 5,
                Name = "Field One"
            };

            var field2 = new CustomField
            {
                Id = 6,
                Name = "Field Two"
            };

            _repository.Create(field1);
            _repository.Create(field2);

            var results = _repository.GetAll();

            Assert.AreEqual(2, results.Count);
        }
    }
}