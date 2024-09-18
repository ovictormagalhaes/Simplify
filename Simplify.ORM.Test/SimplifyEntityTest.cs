using Simplify.ORM.Attributes;
using Simplify.ORM.Enumerations;
using Simplify.ORM.Extensions;
using Simplify.ORM.Test.Entities;
using Xunit;

namespace Simplify.ORM.Test
{
    public class SimplifyEntityTest
    {
        #region GetProperties

        [Fact]
        public void GetProperties_ExampleWithoutAttribute()
        {
            var user = new ExampleWithoutAttribute
            {
                UserId = 1,
                Username = "Test",
                Password = "1234",
                CreatedAt = new DateTime(2024, 7, 1),
            };

            var columnValues = user.GetProperties();

            var expectedProperties = new List<SimplifyEntityProperty>
            {
                new (nameof(user.UserId), nameof(user.UserId), user.UserId),
                new (nameof(user.Username), nameof(user.Username), user.Username),
                new (nameof(user.Password),nameof(user.Password), user.Password),
                new (nameof(user.CreatedAt), nameof(user.CreatedAt), user.CreatedAt)
            };

            Assert.Equal(expectedProperties, columnValues);
        }

        [Fact]
        public void GetProperties_ExampleTablePascalCase()
        {
            var user = new ExampleTablePascalCase
            {
                UserId = 1,
                Username = "Test",
                Password = "1234",
                CreatedAt = new DateTime(2024, 7, 1),
            };

            var columnValues = user.GetProperties();

            var expectedProperties = new List<SimplifyEntityProperty>
            {
                new (nameof(user.UserId), nameof(user.UserId).ToPascalCase(), user.UserId),
                new (nameof(user.Username), nameof(user.Username).ToPascalCase(), user.Username),
                new (nameof(user.Password), nameof(user.Password).ToPascalCase(), user.Password),
                new (nameof(user.CreatedAt), nameof(user.CreatedAt).ToPascalCase(), user.CreatedAt)
            };

            Assert.Equal(expectedProperties, columnValues);
        }

        [Fact]
        public void GetProperties_ExampleTableCamelCase()
        {
            var user = new ExampleTableCamelCase
            {
                UserId = 1,
                Username = "Test",
                Password = "1234",
                CreatedAt = new DateTime(2024, 7, 1),
            };

            var columnValues = user.GetProperties();

            var expectedProperties = new List<SimplifyEntityProperty>
            {
                new (nameof(user.UserId), nameof(user.UserId).ToCamelCase(), user.UserId),
                new (nameof(user.Username), nameof(user.Username).ToCamelCase(), user.Username),
                new (nameof(user.Password), nameof(user.Password).ToCamelCase(), user.Password),
                new (nameof(user.CreatedAt), nameof(user.CreatedAt).ToCamelCase(), user.CreatedAt)
            };

            Assert.Equal(expectedProperties, columnValues);
        }

        [Fact]
        public void GetProperties_ExampleTableSnakeCase()
        {
            var user = new ExampleTableSnakeCase
            {
                UserId = 1,
                Username = "Test",
                Password = "1234",
                CreatedAt = new DateTime(2024, 7, 1),
            };

            var columnValues = user.GetProperties();

            var expectedProperties = new List<SimplifyEntityProperty>
            {
                new (nameof(user.UserId), nameof(user.UserId).ToSnakeCase(), user.UserId),
                new (nameof(user.Username), nameof(user.Username).ToSnakeCase(), user.Username),
                new (nameof(user.Password), nameof(user.Password).ToSnakeCase(), user.Password),
                new (nameof(user.CreatedAt), nameof(user.CreatedAt).ToSnakeCase(), user.CreatedAt)
            };

            Assert.Equal(expectedProperties, columnValues);
        }

        [Fact]
        public void GetProperties_ShouldReturn_CustomValue()
        {
            var user = new UserCustomMethods();

            var columnValues = user.GetProperties();

            var expectedProperties = new List<SimplifyEntityProperty>
            {
                new ("Custom", "Custom", "Value"),
            };
            Assert.Equal(expectedProperties, columnValues);
        }

        [Fact]
        public void GetProperties_ShouldReturn_CustomValue2()
        {
            var user = new UserCustomMethods2();

            var columnValues = user.GetProperties();

            var expectedProperties = new List<SimplifyEntityProperty>
            {
                new ("Custom2", "Custom2", "Value"),
            };
            Assert.Equal(expectedProperties, columnValues);
        }

        #endregion

        #region GetTableName

        [Fact]
        public void GetTableName_ExampleWithoutAttribute_ShouldReturn_CorrectValue()
        {
            var user = new ExampleWithoutAttribute();

            var tableName = user.GetTableName();

            var expectedTableName = nameof(ExampleWithoutAttribute);

            Assert.Equal(expectedTableName, tableName);
        }

        [Fact]
        public void GetTableName_ExampleTableSnakeCase_ShouldReturn_CorrectValue()
        {
            var user = new ExampleTableSnakeCase();

            var tableName = user.GetTableName();

            var expectedTableName = nameof(ExampleTableSnakeCase).ToSnakeCase();

            Assert.Equal(expectedTableName, tableName);
        }

        [Fact]
        public void GetTableName_ExampleTablePascalCase_ShouldReturn_CorrectValue()
        {
            var user = new ExampleTablePascalCase();

            var tableName = user.GetTableName();

            var expectedTableName = nameof(ExampleTablePascalCase).ToPascalCase();

            Assert.Equal(expectedTableName, tableName);
        }

        [Fact]
        public void GetTableName_ExampleTableCamelCase_ShouldReturn_CorrectValue()
        {
            var user = new ExampleTableCamelCase();

            var tableName = user.GetTableName();

            var expectedTableName = nameof(ExampleTableCamelCase).ToCamelCase();

            Assert.Equal(expectedTableName, tableName);
        }

        [Fact]
        public void GetTableName_UserCustomMethods_ShouldReturn_CorrectValue()
        {
            var user = new UserCustomMethods();

            var tableName = user.GetTableName();

            var expectedTableName = "Custom";

            Assert.Equal(expectedTableName, tableName);
        }

        [Fact]
        public void GetTableName_UserCustomMethods2_ShouldReturn_CorrectValue()
        {
            var user = new UserCustomMethods2();

            var tableName = user.GetTableName();

            var expectedTableName = "Custom2";

            Assert.Equal(expectedTableName, tableName);
        }

        #endregion
    }
}
