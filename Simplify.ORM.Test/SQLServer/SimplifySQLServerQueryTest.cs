using Simplify.ORM.SQLServer;

namespace Simplify.ORM.Test.SQLServer
{
    public class SimplifySQLServerQueryTest
    {
        [Fact]
        public void SelectFieldsTest()
        {
            var tableName = "User";
            var columns = new List<string> { "UserId", "Username", "Password" };

            var query = new SimplifySQLServerQuery()
                .SelectFields(tableName, columns);

            var expected = "SELECT [User].[UserId], [User].[Username], [User].[Password]";

            Assert.Equal(expected, query.BuildQuery());
        }

        [Fact]
        public void SelectFieldsMoreThanOneTest()
        {
            var tableName = "User";
            var columns = new List<string> { "UserId", "Username", "Password" };

            var tableName2 = "Permission";
            var columns2 = new List<string> { "PermissionId", "UserId", "Value" };

            var query = new SimplifySQLServerQuery()
                .SelectFields(tableName, columns)
                .SelectFields(tableName2, columns2);

            var expected = "SELECT [User].[UserId], [User].[Username], [User].[Password], [Permission].[PermissionId], [Permission].[UserId], [Permission].[Value]";

            Assert.Equal(expected, query.BuildQuery());
        }

        [Fact]
        public void SelectAllFieldsTest()
        {
            var tableName = "User";

            var query = new SimplifySQLServerQuery()
                .SelectAllFields(tableName);

            var expected = "SELECT [User].*";

            Assert.Equal(expected, query.BuildQuery());
        }

        [Fact]
        public void SelectTopTest()
        {
            var tableName = "User";
            var columns = new List<string> { "UserId", "Username", "Password" };

            var query = new SimplifySQLServerQuery()
                .Top(10)
                .SelectFields(tableName, columns);

            var expected = "SELECT TOP 10 [User].[UserId], [User].[Username], [User].[Password]";

            Assert.Equal(expected, query.BuildQuery());
        }

        [Fact]
        public void FromTest()
        {
            var tableName = "User";

            var query = new SimplifySQLServerQuery()
                .From(tableName);

            var expected = "FROM [User]";

            Assert.Equal(expected, query.BuildQuery());
        }

        [Fact]
        public void FromWithAliasTest()
        {
            var tableName = "User";

            var query = new SimplifySQLServerQuery()
                .From(tableName, "u");

            var expected = "FROM [User] u";

            Assert.Equal(expected, query.BuildQuery());
        }

        #region Join

        [Fact]
        public void JoinTest()
        {
            var query = new SimplifySQLServerQuery()
                .Join("Permission", "UserId", "User", "UserId");

            var expected = "JOIN [Permission] ON [Permission].[UserId] = [User].[UserId]";

            Assert.Equal(expected, query.BuildQuery());
        }

        [Fact]
        public void InnerJoinTest()
        {
            var query = new SimplifySQLServerQuery()
                .InnerJoin("Permission", "UserId", "User", "UserId");

            var expected = "INNER JOIN [Permission] ON [Permission].[UserId] = [User].[UserId]";

            Assert.Equal(expected, query.BuildQuery());
        }

        [Fact]
        public void LeftJoinTest()
        {
            var query = new SimplifySQLServerQuery()
                .LeftJoin("Permission", "UserId", "User", "UserId");

            var expected = "LEFT JOIN [Permission] ON [Permission].[UserId] = [User].[UserId]";

            Assert.Equal(expected, query.BuildQuery());
        }

        [Fact]
        public void RightJoinTest()
        {
            var query = new SimplifySQLServerQuery()
                .RightJoin("Permission", "UserId", "User", "UserId");

            var expected = "RIGHT JOIN [Permission] ON [Permission].[UserId] = [User].[UserId]";

            Assert.Equal(expected, query.BuildQuery());
        }

        #endregion

        #region Where

        [Fact]
        public void WhereEqualsTest()
        {
            var tableName = "User";
            var column = "UserId";

            var query = new SimplifySQLServerQuery()
                .WhereEquals(tableName, column, 1);

            var expected = "WHERE [User].[UserId] = @UserId0";

            Assert.Equal(expected, query.BuildQuery());
            SimplifyQueryAsserts.AssertParameter(query, "UserId0", 1);
        }

        [Fact]
        public void WhereNotEqualsTest()
        {
            var tableName = "User";
            var column = "UserId";

            var query = new SimplifySQLServerQuery()
                .WhereNotEquals(tableName, column, 1);

            var expected = "WHERE [User].[UserId] <> @UserId0";

            Assert.Equal(expected, query.BuildQuery());
            SimplifyQueryAsserts.AssertParameter(query, "UserId0", 1);
        }

        [Fact]
        public void WhereGreaterTest()
        {
            var tableName = "User";
            var column = "UserId";

            var query = new SimplifySQLServerQuery()
                .WhereGreater(tableName, column, 1);

            var expected = "WHERE [User].[UserId] > @UserId0";

            Assert.Equal(expected, query.BuildQuery());
            SimplifyQueryAsserts.AssertParameter(query, "UserId0", 1);
        }

        [Fact]
        public void WhereGreaterOrEqualTest()
        {
            var tableName = "User";
            var column = "UserId";

            var query = new SimplifySQLServerQuery()
                .WhereGreaterOrEqual(tableName, column, 1);

            var expected = "WHERE [User].[UserId] >= @UserId0";

            Assert.Equal(expected, query.BuildQuery());
            SimplifyQueryAsserts.AssertParameter(query, "UserId0", 1);
        }

        [Fact]
        public void WhereLowerTest()
        {
            var tableName = "User";
            var column = "UserId";

            var query = new SimplifySQLServerQuery()
                .WhereLower(tableName, column, 1);

            var expected = "WHERE [User].[UserId] < @UserId0";

            Assert.Equal(expected, query.BuildQuery());
            SimplifyQueryAsserts.AssertParameter(query, "UserId0", 1);
        }

        [Fact]
        public void WhereLowerOrEqualTest()
        {
            var tableName = "User";
            var column = "UserId";

            var query = new SimplifySQLServerQuery()
                .WhereLowerOrEqual(tableName, column, 1);

            var expected = "WHERE [User].[UserId] <= @UserId0";

            Assert.Equal(expected, query.BuildQuery());
            SimplifyQueryAsserts.AssertParameter(query, "UserId0", 1);
        }

        [Fact]
        public void WhereBetweenTest()
        {
            var tableName = "User";
            var column = "UserId";

            var query = new SimplifySQLServerQuery()
                .WhereBetween(tableName, column, 1, 10);

            var expected = "WHERE [User].[UserId] BETWEEN @UserId0 AND @UserId1";

            Assert.Equal(expected, query.BuildQuery());
            SimplifyQueryAsserts.AssertParameter(query, "UserId0", 1);
            SimplifyQueryAsserts.AssertParameter(query, "UserId1", 10);
        }

        #endregion

        #region And

        [Fact]
        public void AndEqualsTest()
        {
            var query = new SimplifySQLServerQuery()
                .WhereEquals("User", "UserId", 1)
                .AndEquals("User", "Permission", 2);

            var expected = "WHERE [User].[UserId] = @UserId0 AND [User].[Permission] = @Permission0";

            Assert.Equal(expected, query.BuildQuery());

            SimplifyQueryAsserts.AssertParameter(query, "UserId0", 1);
            SimplifyQueryAsserts.AssertParameter(query, "Permission0", 2);
        }

        [Fact]
        public void AndNotEqualsTest()
        {
            var query = new SimplifySQLServerQuery()
                .WhereEquals("User", "UserId", 1)
                .AndNotEquals("User", "Permission", 2);

            var expected = "WHERE [User].[UserId] = @UserId0 AND [User].[Permission] <> @Permission0";

            Assert.Equal(expected, query.BuildQuery());

            SimplifyQueryAsserts.AssertParameter(query, "UserId0", 1);
            SimplifyQueryAsserts.AssertParameter(query, "Permission0", 2);
        }

        [Fact]
        public void AndGreaterTest()
        {
            var query = new SimplifySQLServerQuery()
                .WhereEquals("User", "UserId", 1)
                .AndGreater("User", "Permission", 2);

            var expected = "WHERE [User].[UserId] = @UserId0 AND [User].[Permission] > @Permission0";

            Assert.Equal(expected, query.BuildQuery());

            SimplifyQueryAsserts.AssertParameter(query, "UserId0", 1);
            SimplifyQueryAsserts.AssertParameter(query, "Permission0", 2);
        }


        [Fact]
        public void AndGreaterOrEqualsTest()
        {
            var query = new SimplifySQLServerQuery()
                .WhereEquals("User", "UserId", 1)
                .AndGreaterOrEqual("User", "Permission", 2);

            var expected = "WHERE [User].[UserId] = @UserId0 AND [User].[Permission] >= @Permission0";

            Assert.Equal(expected, query.BuildQuery());

            SimplifyQueryAsserts.AssertParameter(query, "UserId0", 1);
            SimplifyQueryAsserts.AssertParameter(query, "Permission0", 2);
        }

        [Fact]
        public void AndLowerTest()
        {
            var query = new SimplifySQLServerQuery()
                .WhereEquals("User", "UserId", 1)
                .AndLower("User", "Permission", 2);

            var expected = "WHERE [User].[UserId] = @UserId0 AND [User].[Permission] < @Permission0";

            Assert.Equal(expected, query.BuildQuery());

            SimplifyQueryAsserts.AssertParameter(query, "UserId0", 1);
            SimplifyQueryAsserts.AssertParameter(query, "Permission0", 2);
        }

        [Fact]
        public void AndLowerOrEqualsTest()
        {
            var query = new SimplifySQLServerQuery()
                .WhereEquals("User", "UserId", 1)
                .AndLowerOrEqual("User", "Permission", 2);

            var expected = "WHERE [User].[UserId] = @UserId0 AND [User].[Permission] <= @Permission0";

            Assert.Equal(expected, query.BuildQuery());

            SimplifyQueryAsserts.AssertParameter(query, "UserId0", 1);
            SimplifyQueryAsserts.AssertParameter(query, "Permission0", 2);
        }

        [Fact]
        public void AndBetweenTest()
        {
            var query = new SimplifySQLServerQuery()
                .WhereEquals("User", "UserId", 1)
                .AndBetween("User", "Permission", 10, 20);

            var expected = "WHERE [User].[UserId] = @UserId0 AND [User].[Permission] BETWEEN @Permission0 AND @Permission1";

            Assert.Equal(expected, query.BuildQuery());

            SimplifyQueryAsserts.AssertParameter(query, "UserId0", 1);
            SimplifyQueryAsserts.AssertParameter(query, "Permission0", 10);
            SimplifyQueryAsserts.AssertParameter(query, "Permission1", 20);
        }

        #endregion

        #region Or

        [Fact]
        public void OrEqualsTest()
        {
            var query = new SimplifySQLServerQuery()
                .WhereEquals("User", "UserId", 1)
                .OrEquals("User", "Permission", 2);

            var expected = "WHERE [User].[UserId] = @UserId0 OR [User].[Permission] = @Permission0";

            var actual = query.BuildQuery();

            Assert.Equal(expected, actual);
            SimplifyQueryAsserts.AssertParameter(query, "UserId0", 1);
            SimplifyQueryAsserts.AssertParameter(query, "Permission0", 2);
        }

        [Fact]
        public void OrNotEqualsTest()
        {
            var query = new SimplifySQLServerQuery()
                .WhereEquals("User", "UserId", 1)
                .OrNotEquals("User", "Permission", 2);

            var expected = "WHERE [User].[UserId] = @UserId0 OR [User].[Permission] <> @Permission0";

            var actual = query.BuildQuery();

            Assert.Equal(expected, actual);
            SimplifyQueryAsserts.AssertParameter(query, "UserId0", 1);
            SimplifyQueryAsserts.AssertParameter(query, "Permission0", 2);
        }

        [Fact]
        public void OrGreaterTest()
        {
            var query = new SimplifySQLServerQuery()
                .WhereEquals("User", "UserId", 1)
                .OrGreater("User", "Permission", 2);

            var expected = "WHERE [User].[UserId] = @UserId0 OR [User].[Permission] > @Permission0";

            var actual = query.BuildQuery();

            Assert.Equal(expected, actual);
            SimplifyQueryAsserts.AssertParameter(query, "UserId0", 1);
            SimplifyQueryAsserts.AssertParameter(query, "Permission0", 2);
        }


        [Fact]
        public void OrGreaterOrEqualsTest()
        {
            var query = new SimplifySQLServerQuery()
                .WhereEquals("User", "UserId", 1)
                .OrGreaterEqual("User", "Permission", 2);

            var expected = "WHERE [User].[UserId] = @UserId0 OR [User].[Permission] >= @Permission0";

            var actual = query.BuildQuery();

            Assert.Equal(expected, actual);
            SimplifyQueryAsserts.AssertParameter(query, "UserId0", 1);
            SimplifyQueryAsserts.AssertParameter(query, "Permission0", 2);
        }

        [Fact]
        public void OrLowerTest()
        {
            var query = new SimplifySQLServerQuery()
                .WhereEquals("User", "UserId", 1)
                .OrLower("User", "Permission", 2);

            var expected = "WHERE [User].[UserId] = @UserId0 OR [User].[Permission] < @Permission0";

            var actual = query.BuildQuery();

            Assert.Equal(expected, actual);
            SimplifyQueryAsserts.AssertParameter(query, "UserId0", 1);
            SimplifyQueryAsserts.AssertParameter(query, "Permission0", 2);
        }

        [Fact]
        public void OrLowerOrEqualsTest()
        {
            var query = new SimplifySQLServerQuery()
                .WhereEquals("User", "UserId", 1)
                .OrLowerEqual("User", "Permission", 2);

            var expected = "WHERE [User].[UserId] = @UserId0 OR [User].[Permission] <= @Permission0";

            var actual = query.BuildQuery();

            Assert.Equal(expected, actual);
            SimplifyQueryAsserts.AssertParameter(query, "UserId0", 1);
            SimplifyQueryAsserts.AssertParameter(query, "Permission0", 2);
        }

        [Fact]
        public void OrBetweenTest()
        {
            var query = new SimplifySQLServerQuery()
                .WhereEquals("User", "UserId", 1)
                .OrBetween("User", "Permission", 10, 20);

            var expected = "WHERE [User].[UserId] = @UserId0 OR [User].[Permission] BETWEEN @Permission0 AND @Permission1";

            var actual = query.BuildQuery();

            Assert.Equal(expected, actual);
            SimplifyQueryAsserts.AssertParameter(query, "UserId0", 1);
            SimplifyQueryAsserts.AssertParameter(query, "Permission0", 10);
            SimplifyQueryAsserts.AssertParameter(query, "Permission1", 20);
        }


        [Fact]
        public void ComplexOrTest()
        {
            var query = new SimplifySQLServerQuery()
                .WhereEquals("User", "UserId", 1)
                .OrEquals("User", "Permission", 10)
                .OrNotEquals("User", "Permission", 11)
                .OrGreater("User", "Permission", 12)
                .OrGreaterEqual("User", "Permission", 13)
                .OrLower("User", "Permission", 14)
                .OrLowerEqual("User", "Permission", 15);

            var expected = "WHERE [User].[UserId] = @UserId0 OR [User].[Permission] = @Permission0 OR [User].[Permission] <> @Permission1 OR [User].[Permission] > @Permission2 OR [User].[Permission] >= @Permission3 OR [User].[Permission] < @Permission4 OR [User].[Permission] <= @Permission5";

            var actual = query.BuildQuery();

            Assert.Equal(expected, actual);
            SimplifyQueryAsserts.AssertParameter(query, "UserId0", 1);
            SimplifyQueryAsserts.AssertParameter(query, "Permission0", 10);
            SimplifyQueryAsserts.AssertParameter(query, "Permission1", 11);
            SimplifyQueryAsserts.AssertParameter(query, "Permission2", 12);
            SimplifyQueryAsserts.AssertParameter(query, "Permission3", 13);
            SimplifyQueryAsserts.AssertParameter(query, "Permission4", 14);
            SimplifyQueryAsserts.AssertParameter(query, "Permission5", 15);
        }

        #endregion

        #region Order By
        
        [Fact]
        public void OrderByTest()
        {
            var query = new SimplifySQLServerQuery()
                .OrderBy("User", "Username", SimplifyOrderOperation.Asc)
                .OrderBy("User", "Name", SimplifyOrderOperation.Desc);

            var expected = "ORDER BY [User].[Username] ASC, [User].[Name] DESC";

            Assert.Equal(expected, query.BuildQuery());
        }

        #endregion

        [Fact]
        public void QueryTest()
        {
            var query = new SimplifySQLServerQuery()
                .SelectFields("User", new List<string> { "UserId", "Username", "Password" })
                .From("User")
                .InnerJoin("Permission", "UserId", "User", "UserId")
                .WhereEquals("User", "UserId", 1)
                .AndEquals("Permission", "Permission", 2)
                .OrGreater("Permission", "Permission", 10)
                .AndLower("Permission", "Permission", 50);
            ;

            var expected = "SELECT [User].[UserId], [User].[Username], [User].[Password] FROM [User] INNER JOIN [Permission] ON [Permission].[UserId] = [User].[UserId] WHERE [User].[UserId] = @UserId0 AND [Permission].[Permission] = @Permission0 OR [Permission].[Permission] > @Permission1 AND [Permission].[Permission] < @Permission2";
            Assert.Equal(expected, query.BuildQuery());
        }
    }
}