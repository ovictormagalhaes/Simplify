using Simplify.ORM.Attributes;
using Simplify.ORM.Builders;
using Simplify.ORM.Enumerations;
using Simplify.ORM.Interfaces;
using Xunit;

namespace Simplify.ORM.Test.Builders
{
    [Table(NamingConvention.PascalCase)]
    public partial class UserMockEntity : SimplifyEntity
    {
        public int UserMockId { get; set; }
    }


    public class SimplifyCommandBuilderTest
    {
        public static IEnumerable<object[]> Types()
        {
            return [
                [typeof(SimplifySQLServerCommandBuilder)],
                [typeof(SimplifyPostgresSQLCommandBuilder)],
                [typeof(SimplifyMySQLCommandBuilder)]
            ];
        }

        private static ISimplifyCommandBuilder GetCommandInstance(Type type) => (ISimplifyCommandBuilder)Activator.CreateInstance(type);


        [Theory]
        [MemberData(nameof(Types))]
        public void AddInsertTest(Type type)
        {
            var table = "User";
            var columnValues = new Dictionary<string, object>() {
                { "Username", "Victor" },
                { "Password", "secret" }
            };

            var queryBuilder = GetCommandInstance(type)
                .AddInsert(table, columnValues);

            var query = queryBuilder.BuildQuery();

            string expectedQuery;
            if (type == typeof(SimplifySQLServerCommandBuilder))
                expectedQuery = "INSERT INTO [User] ([Username], [Password]) VALUES (@Username, @Password);";
            else if (type == typeof(SimplifyPostgresSQLCommandBuilder))
                expectedQuery = "INSERT INTO \"User\" (\"Username\", \"Password\") VALUES (@Username, @Password);";
            else if (type == typeof(SimplifyMySQLCommandBuilder))
                expectedQuery = "INSERT INTO `User` (`Username`, `Password`) VALUES (@Username, @Password);";
            else
                throw new NotSupportedException("Unsupported type");

            Assert.Equal(expectedQuery, query);
        }

        [Fact]
        public void AddInsertMockEntityTest()
        {
            var entity = new UserMockEntity { UserMockId = 10 };

            var query = new SimplifyPostgresSQLCommandBuilder()
                .AddInsert(entity)
                .BuildQuery();

            var expectedQuery = "INSERT INTO UserMockEntity (UserMockId) VALUES (@UserMockId);";

            Assert.Equal(expectedQuery, query);
        }

        [Fact]
        public void AddUpdateTest()
        {
            var table = "User";
            var columnValues = new Dictionary<string, object>() {
                { "Username", "Victor" },
                { "Password", "secret" }
            };
            var whereOperations = new List<WhereOperation>() {
                new(SimplifyWhereOperation.Equals, table, "Id", "@Id", 1)
            };
            var query = new SimplifyMySQLCommandBuilder()
                .AddUpdate(table, columnValues, whereOperations)
                .BuildQuery();

            var expectedQuery = "UPDATE User SET Username = @Username, Password = @Password WHERE User.Id = @Id ;";

            Assert.Equal(expectedQuery, query);
        }

    }
}
