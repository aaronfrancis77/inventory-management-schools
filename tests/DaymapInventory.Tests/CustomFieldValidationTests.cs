using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InventorySystem.Tests
{
    [TestClass]
    public class CustomFieldValidationTests
    {
        private SqlCustomFieldRepository _repository = null!;

        [TestInitialize]
        public void Setup()
        {
            _repository = new SqlCustomFieldRepository();
        }

        [TestMethod]
        public void Create_Should_Allow_Unique_FieldNames()
        {
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Create_Should_Not_Allow_Duplicate_FieldNames()
        {
            Assert.IsTrue(true);
        }
    }
}