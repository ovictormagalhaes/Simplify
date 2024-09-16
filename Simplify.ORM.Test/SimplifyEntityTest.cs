using Simplify.ORM.Attributes;
using Simplify.ORM.Enumerations;
using Simplify.ORM.Extensions;
using Xunit;

namespace Simplify.ORM.Test
{
    public partial class ExampleWithoutAttribute : SimplifyEntity
    {
        public int UserId { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public DateTime CreatedAt { get; set; }

        public const int SomeConst = 1;
        public virtual List<string>? Permissions { get; set; }
    }

    [Table(NamingConvention.PascalCase)]
    public partial class ExampleTablePascalCaseCase : SimplifyEntity
    {
        public int UserId { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    [Table(NamingConvention.CamelCase)]
    public partial class ExampleTableCamelCaseCase : SimplifyEntity
    {
        public int UserId { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    [Table(NamingConvention.SnakeCase)]
    public partial class ExampleTableSnakeCase : SimplifyEntity
    {
        public int UserId { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    [Table("Custom")]
    public partial class UserCustomMethods : SimplifyEntity
    {
        public override IEnumerable<SimplifyEntityProperty> GetProperties()
        {
            return [
                new ("Custom", "Custom", "Value")
            ]; 
        }
    }

    [Table(Name = "Custom2")]
    public partial class UserCustomMethods2 : SimplifyEntity
    {

        public override IEnumerable<SimplifyEntityProperty> GetProperties()
        {
            return [ new ("Custom2", "Custom2", "Value") ];
        }
    }


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
            var user = new ExampleTablePascalCaseCase
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
            var user = new ExampleTableCamelCaseCase
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

        [Fact]
        public void GetTableName_ShouldReturn_DefaultValue()
        {
            var user = new ExampleWithoutAttribute();

            var tableName = user.GetTableName();

            var expectedTableName = nameof(ExampleWithoutAttribute);

            Assert.Equal(expectedTableName, tableName);
        }

        [Fact]
        public void GetTableName_ShouldReturn_CustomValue()
        {
            var user = new UserCustomMethods();

            var tableName = user.GetTableName();

            var expectedTableName = "Custom";

            Assert.Equal(expectedTableName, tableName);
        }

        [Fact]
        public void GetTableName_ShouldReturn_CustomValue2()
        {
            var user = new UserCustomMethods2();

            var tableName = user.GetTableName();

            var expectedTableName = "Custom2";

            Assert.Equal(expectedTableName, tableName);
        }
    }
}
