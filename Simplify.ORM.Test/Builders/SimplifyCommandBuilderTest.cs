using Simplify.ORM.Builders;
using Simplify.ORM.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Simplify.ORM.Test.Builders
{
    public class SimplifyCommandBuilderTest
    {
        [Fact]
        public void AddInsertTest()
        {
            var table = "User";
            var columnValues = new Dictionary<string, object>() {
                { "Username", "Victor" },
                { "Password", "secret" }
            };
            var query = new SimplifyCommandBuilder()
                .AddInsert(table, columnValues)
                .BuildQuery();

            var expectedQuery = "INSERT INTO User (Username, Password) VALUES (@Username, @Password);";

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
            var query = new SimplifyCommandBuilder()
                .AddUpdate(table, columnValues, whereOperations)
                .BuildQuery();

            var expectedQuery = "UPDATE User SET Username = @Username, Password = @Password WHERE User.Id = @Id ;";

            Assert.Equal(expectedQuery, query);
        }

    }
}
