using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InventorySystem.Tests
{
    [TestClass]
    public class CustomFieldCascadeTests
    {
        private SqlCustomFieldRepository _repository = null!;

        [TestInitialize]
        public void Setup()
        {
            _repository = new SqlCustomFieldRepository();
        }

        [TestMethod]
        public void Delete_Field_Should_Remove_Associated_Values()
        {
            var field = new CustomField
            {
                Id = 1,
                Name = "Laptop Serial"
            };

            _repository.Create(field);

            _repository.Delete(1);

            var result = _repository.GetById(1);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Delete_Field_Should_Not_Return_Deleted_Field()
        {
            var field = new CustomField
            {
                Id = 2,
                Name = "Room Number"
            };

            _repository.Create(field);

            _repository.Delete(2);

            Assert.IsNull(_repository.GetById(2));
        }
    }
}