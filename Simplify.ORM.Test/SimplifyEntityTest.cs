using Xunit;

namespace Simplify.ORM.Test
{
    public partial class User : SimplifyEntity
    {
        public int UserId { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public DateTime CreatedAt { get; set; }

        private const int SomeConst = 1;
        public virtual List<string>? Permissions { get; set; }
    }

    public partial class UserCustomMethods : SimplifyEntity
    {
        public override string GetTableName() => "Custom";

        public override Dictionary<string, object> GetColumnValues()
        {
            return new Dictionary<string, object>() {
                { "Custom", "Value" },
            };
        }
    }

    public class SimplifyEntityTest
    {
        [Fact]
        public void GetColumnValues_ShouldReturn_DefaultValue()
        {
            var user = new User 
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
        public void GetTableName_ShouldReturn_DefaultValue()
        {
            var user = new User();

            var tableName = user.GetTableName();

            var expectedTableName = nameof(User);

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
    }
}
