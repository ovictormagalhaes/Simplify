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
        public override Dictionary<string, object> GetColumnValues()
        {
            return new Dictionary<string, object>() {
                { "Custom", "Value" },
            };
        }

        public new static string TableName => "Custom";
    }

    [Table(Name = "Custom2")]
    public partial class UserCustomMethods2 : SimplifyEntity
    {
        public override Dictionary<string, object> GetColumnValues()
        {
            return new Dictionary<string, object>() {
                { "Custom2", "Value2" },
            };
        }
    }


    public class SimplifyEntityTest
    {

        #region GetColumnValues

        #endregion
        [Fact]
        public void GetColumnValues_ExampleWithoutAttribute()
        {
            var user = new ExampleWithoutAttribute 
            { 
                UserId = 1,
                Username = "Test",
                Password = "1234",
                CreatedAt = new DateTime(2024, 7, 1),
            };

            var columnValues = user.GetColumnValues();

            var expectedColumnValues = new Dictionary<string, object>
            {
                { nameof(user.UserId), user.UserId },
                { nameof(user.Username), user.Username },
                { nameof(user.Password), user.Password },
                { nameof(user.CreatedAt), user.CreatedAt }
            };

            Assert.Equal(expectedColumnValues, columnValues);
        }

        [Fact]
        public void GetColumnValues_ExampleTablePascalCase()
        {
            var user = new ExampleTablePascalCaseCase
            {
                UserId = 1,
                Username = "Test",
                Password = "1234",
                CreatedAt = new DateTime(2024, 7, 1),
            };

            var columnValues = user.GetColumnValues();

            var expectedColumnValues = new Dictionary<string, object>
            {
                { nameof(user.UserId).ToPascalCase(), user.UserId },
                { nameof(user.Username).ToPascalCase(), user.Username },
                { nameof(user.Password).ToPascalCase(), user.Password },
                { nameof(user.CreatedAt).ToPascalCase(), user.CreatedAt }
            };

            Assert.Equal(expectedColumnValues, columnValues);
        }

        [Fact]
        public void GetColumnValues_ExampleTableCamelCase()
        {
            var user = new ExampleTableCamelCaseCase
            {
                UserId = 1,
                Username = "Test",
                Password = "1234",
                CreatedAt = new DateTime(2024, 7, 1),
            };

            var columnValues = user.GetColumnValues();

            var expectedColumnValues = new Dictionary<string, object>
            {
                { nameof(user.UserId).ToCamelCase(), user.UserId },
                { nameof(user.Username).ToCamelCase(), user.Username },
                { nameof(user.Password).ToCamelCase(), user.Password },
                { nameof(user.CreatedAt).ToCamelCase(), user.CreatedAt }
            };

            Assert.Equal(expectedColumnValues, columnValues);
        }

        [Fact]
        public void GetColumnValues_ExampleTableSnakeCase()
        {
            var user = new ExampleTableSnakeCase
            {
                UserId = 1,
                Username = "Test",
                Password = "1234",
                CreatedAt = new DateTime(2024, 7, 1),
            };

            var columnValues = user.GetColumnValues();

            var expectedColumnValues = new Dictionary<string, object>
            {
                { nameof(user.UserId).ToSnakeCase(), user.UserId },
                { nameof(user.Username).ToSnakeCase(), user.Username },
                { nameof(user.Password).ToSnakeCase(), user.Password },
                { nameof(user.CreatedAt).ToSnakeCase(), user.CreatedAt }
            };

            Assert.Equal(expectedColumnValues, columnValues);
        }

        [Fact]
        public void GetColumnValues_ShouldReturn_CustomValue()
        {
            var user = new UserCustomMethods();

            var columnValues = user.GetColumnValues();

            var expectedColumnValues = new Dictionary<string, object>
            {
                { "Custom", "Value" },
            };

            Assert.Equal(expectedColumnValues, columnValues);
        }

        [Fact]
        public void GetColumnValues_ShouldReturn_CustomValue2()
        {
            var user = new UserCustomMethods2();

            var columnValues = user.GetColumnValues();

            var expectedColumnValues = new Dictionary<string, object>
            {
                { "Custom2", "Value2" },
            };

            Assert.Equal(expectedColumnValues, columnValues);
        }

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
