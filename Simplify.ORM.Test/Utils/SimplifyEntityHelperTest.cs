using Simplify.ORM.Attributes;
using Simplify.ORM.Enumerations;
using Simplify.ORM.Test.Entities;
using Simplify.ORM.Utils;
using Xunit;

namespace Simplify.ORM.Test.Utils
{
    public class SimplifyEntityHelperTests
    {
        [Fact]
        public void TableName_Entity_ShouldReturnTableName()
        {
            // Arrange
            var mockEntity = new MockEntity();

            // Act
            var result = mockEntity.TableName();

            // Assert
            Assert.Equal("MockEntity", result);
        }

        [Fact]
        public void TableName_Generic_ShouldReturnTableName()
        {
            // Act
            var result = SimplifyEntityHelper.TableName<MockEntity>();

            // Assert
            Assert.Equal("MockEntity", result);
        }

        [Fact]
        public void TableName_Type_ShouldReturnTableName()
        {
            // Arrange
            var type = typeof(MockEntity);

            // Act
            var result = SimplifyEntityHelper.TableName(type);

            // Assert
            Assert.Equal("MockEntity", result);
        }

        [Fact]
        public void ColumnName_Entity_ShouldReturnColumnName()
        {
            // Arrange
            var mockEntity = new MockEntity();

            // Act
            var result = mockEntity.ColumnName("MockId");

            // Assert
            Assert.Equal("MockId", result);
        }

        [Fact]
        public void ColumnName_Generic_ShouldReturnColumnName()
        {
            // Act
            var result = SimplifyEntityHelper.ColumnName<MockEntity>("MockId");

            // Assert
            Assert.Equal("MockId", result);
        }

        [Fact]
        public void ColumnName_Type_ShouldReturnColumnName()
        {
            // Arrange
            var type = typeof(MockEntity);

            // Act
            var result = SimplifyEntityHelper.ColumnName(type, "MockId");

            // Assert
            Assert.Equal("MockId", result);
        }

    }

}
