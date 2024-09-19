using Simplify.ORM.Builders;
using Simplify.ORM.Enumerations;
using Simplify.ORM.Interfaces;
using Xunit;

namespace Simplify.ORM.Test.Builders
{
    public class SimplifyQueryBuilderTest
    {
        public static IEnumerable<object[]> Types()
        {
            return [
                [typeof(SimplifySQLServerQueryBuilder)],
                [typeof(SimplifyPostgresSQLQueryBuilder)],
                [typeof(SimplifyMySQLQueryBuilder)]
            ];
        }

        private static ISimplifyQueryBuilder GetInstance(Type type) => (ISimplifyQueryBuilder)Activator.CreateInstance(type)!;
        
        [Theory]
        [MemberData(nameof(Types))]
        public void SelectFieldsTest(Type type)
        {
            var tableName = "User";
            var columns = new List<string> { "UserId", "Username", "Password" };

            var queryBuilder = GetInstance(type).SelectFields(tableName, columns);
            var query = queryBuilder.BuildQuery();

            string expected;
            if (type == typeof(SimplifySQLServerQueryBuilder))
                expected = "SELECT [User].[UserId], [User].[Username], [User].[Password] ;";
            else if (type == typeof(SimplifyPostgresSQLQueryBuilder))
                expected = "SELECT \"User\".\"UserId\", \"User\".\"Username\", \"User\".\"Password\" ;";
            else if (type == typeof(SimplifyMySQLQueryBuilder))
                expected = "SELECT `User`.`UserId`, `User`.`Username`, `User`.`Password` ;";
            else
                throw new NotSupportedException("Not suppoted type");

           Assert.Equal(expected, query);
        }

        [Theory]
        [MemberData(nameof(Types))]
        public void SelectFieldsMoreThanOneTest(Type type)
        {
            var tableName = "User";
            var columns = new List<string> { "UserId", "Username", "Password" };

            var tableName2 = "Permission";
            var columns2 = new List<string> { "PermissionId", "UserId", "Value" };

            var queryBuilder = GetInstance(type)
                .SelectFields(tableName, columns)
                .SelectFields(tableName2, columns2);

            var query = queryBuilder.BuildQuery();

            string expected;
            if (type == typeof(SimplifySQLServerQueryBuilder))
                expected = "SELECT [User].[UserId], [User].[Username], [User].[Password], [Permission].[PermissionId], [Permission].[UserId], [Permission].[Value] ;";
            else if (type == typeof(SimplifyPostgresSQLQueryBuilder))
                expected = "SELECT \"User\".\"UserId\", \"User\".\"Username\", \"User\".\"Password\", \"Permission\".\"PermissionId\", \"Permission\".\"UserId\", \"Permission\".\"Value\" ;";
            else if (type == typeof(SimplifyMySQLQueryBuilder))
                expected = "SELECT `User`.`UserId`, `User`.`Username`, `User`.`Password`, `Permission`.`PermissionId`, `Permission`.`UserId`, `Permission`.`Value` ;";
            else
                throw new NotSupportedException("Not suppoted type");

            Assert.Equal(expected, query);
        }

        [Theory]
        [MemberData(nameof(Types))]
        public void SelectAllFieldsTest(Type type)
        {
            var tableName = "User";

            var queryBuilder = GetInstance(type).SelectAllFields(tableName);

            var query = queryBuilder.BuildQuery();

            string expected;
            if (type == typeof(SimplifySQLServerQueryBuilder))
                expected = "SELECT [User].* ;";
            else if (type == typeof(SimplifyPostgresSQLQueryBuilder))
                expected = "SELECT \"User\".* ;";
            else if (type == typeof(SimplifyMySQLQueryBuilder))
                expected = "SELECT `User`.* ;";
            else
                throw new NotSupportedException("Not suppoted type");

            Assert.Equal(expected, query);
        }

        [Theory]
        [MemberData(nameof(Types))]
        public void SelectTopTest(Type type)
        {
            var tableName = "User";
            var columns = new List<string> { "UserId", "Username", "Password" };

            var queryBuilder = GetInstance(type).Top(10)
                .SelectFields(tableName, columns);
            var query = queryBuilder.BuildQuery();

            string expected;
            if (type == typeof(SimplifySQLServerQueryBuilder))
                expected = "SELECT TOP 10 [User].[UserId], [User].[Username], [User].[Password] ;";
            else if (type == typeof(SimplifyPostgresSQLQueryBuilder))
                expected = "SELECT \"User\".\"UserId\", \"User\".\"Username\", \"User\".\"Password\" ;";// LIMIT 10 ;";
            else if (type == typeof(SimplifyMySQLQueryBuilder))
                expected = "SELECT `User`.`UserId`, `User`.`Username`, `User`.`Password` ;"; //LIMIT 10 ;";
            else
                throw new NotSupportedException("Not suppoted type");

            Assert.Equal(expected, query);
        }

        [Theory]
        [MemberData(nameof(Types))]
        public void FromTest(Type type)
        {
            var tableName = "User";

            var queryBuilder = GetInstance(type)
                .From(tableName);
            var query = queryBuilder.BuildQuery();

            string expected;
            if (type == typeof(SimplifySQLServerQueryBuilder))
                expected = "FROM [User] ;";
            else if (type == typeof(SimplifyPostgresSQLQueryBuilder))
                expected = "FROM \"User\" ;";
            else if (type == typeof(SimplifyMySQLQueryBuilder))
                expected = "FROM `User` ;";
            else
                throw new NotSupportedException("Not supported type");

            Assert.Equal(expected, query);
        }

        [Theory]
        [MemberData(nameof(Types))]
        public void FromWithAliasTest(Type type)
        {
            var tableName = "User";

            var queryBuilder = GetInstance(type)
                .From(tableName, "u");
            var query = queryBuilder.BuildQuery();

            string expected;
            if (type == typeof(SimplifySQLServerQueryBuilder))
                expected = "FROM [User] u ;";
            else if (type == typeof(SimplifyPostgresSQLQueryBuilder))
                expected = "FROM \"User\" u ;";
            else if (type == typeof(SimplifyMySQLQueryBuilder))
                expected = "FROM `User` u ;";
            else
                throw new NotSupportedException("Not supported type");

            Assert.Equal(expected, query);
        }

        #region Join

        [Theory]
        [MemberData(nameof(Types))]
        public void JoinTest(Type type)
        {
            var queryBuilder = GetInstance(type)
                .Join("Permission", "UserId", "User", "UserId");
            var query = queryBuilder.BuildQuery();

            string expected;
            if (type == typeof(SimplifySQLServerQueryBuilder))
                expected = "JOIN [Permission] ON [Permission].[UserId] = [User].[UserId] ;";
            else if (type == typeof(SimplifyPostgresSQLQueryBuilder))
                expected = "JOIN \"Permission\" ON \"Permission\".\"UserId\" = \"User\".\"UserId\" ;";
            else if (type == typeof(SimplifyMySQLQueryBuilder))
                expected = "JOIN `Permission` ON `Permission`.`UserId` = `User`.`UserId` ;";
            else
                throw new NotSupportedException("Not supported type");

            Assert.Equal(expected, query);
        }

        [Theory]
        [MemberData(nameof(Types))]
        public void InnerJoinTest(Type type)
        {
            var queryBuilder = GetInstance(type)
                .InnerJoin("Permission", "UserId", "User", "UserId");
            var query = queryBuilder.BuildQuery();

            string expected;
            if (type == typeof(SimplifySQLServerQueryBuilder))
                expected = "INNER JOIN [Permission] ON [Permission].[UserId] = [User].[UserId] ;";
            else if (type == typeof(SimplifyPostgresSQLQueryBuilder))
                expected = "INNER JOIN \"Permission\" ON \"Permission\".\"UserId\" = \"User\".\"UserId\" ;";
            else if (type == typeof(SimplifyMySQLQueryBuilder))
                expected = "INNER JOIN `Permission` ON `Permission`.`UserId` = `User`.`UserId` ;";
            else
                throw new NotSupportedException("Not suppoted type");

            Assert.Equal(expected, query);
        }

        [Theory]
        [MemberData(nameof(Types))]
        public void LeftJoinTest(Type type)
        {
            var queryBuilder = GetInstance(type)
                .LeftJoin("Permission", "UserId", "User", "UserId");
            var query = queryBuilder.BuildQuery();

            string expected;
            if (type == typeof(SimplifySQLServerQueryBuilder))
                expected = "LEFT JOIN [Permission] ON [Permission].[UserId] = [User].[UserId] ;";
            else if (type == typeof(SimplifyPostgresSQLQueryBuilder))
                expected = "LEFT JOIN \"Permission\" ON \"Permission\".\"UserId\" = \"User\".\"UserId\" ;";
            else if (type == typeof(SimplifyMySQLQueryBuilder))
                expected = "LEFT JOIN `Permission` ON `Permission`.`UserId` = `User`.`UserId` ;";
            else
                throw new NotSupportedException("Not suppoted type");

            Assert.Equal(expected, query);
        }

        [Theory]
        [MemberData(nameof(Types))]
        public void RightJoinTest(Type type)
        {
            var queryBuilder = GetInstance(type)
                .RightJoin("Permission", "UserId", "User", "UserId");
            var query = queryBuilder.BuildQuery();

            string expected;
            if (type == typeof(SimplifySQLServerQueryBuilder))
                expected = "RIGHT JOIN [Permission] ON [Permission].[UserId] = [User].[UserId] ;";
            else if (type == typeof(SimplifyPostgresSQLQueryBuilder))
                expected = "RIGHT JOIN \"Permission\" ON \"Permission\".\"UserId\" = \"User\".\"UserId\" ;";
            else if (type == typeof(SimplifyMySQLQueryBuilder))
                expected = "RIGHT JOIN `Permission` ON `Permission`.`UserId` = `User`.`UserId` ;";
            else
                throw new NotSupportedException("Not suppoted type");

            Assert.Equal(expected, query);
        }

        #endregion

        #region Where

        [Theory]
        [MemberData(nameof(Types))]
        public void WhereEqualsTest(Type type)
        {
            var tableName = "User";
            var column = "UserId";

            var queryBuilder = GetInstance(type)
                .WhereEquals(tableName, column, 1);
            var query = queryBuilder.BuildQuery();

            string expected;
            if (type == typeof(SimplifySQLServerQueryBuilder))
                expected = "WHERE [User].[UserId] = @UserId0 ;";
            else if (type == typeof(SimplifyPostgresSQLQueryBuilder))
                expected = "WHERE \"User\".\"UserId\" = @UserId0 ;";
            else if (type == typeof(SimplifyMySQLQueryBuilder))
                expected = "WHERE `User`.`UserId` = @UserId0 ;";
            else
                throw new NotSupportedException("Not suppoted type");

            Assert.Equal(expected, query);
            SimplifyQueryBuilderAsserts.AssertParameter(queryBuilder, "UserId0", 1);
        }

        [Theory]
        [MemberData(nameof(Types))]
        public void WhereInTest(Type type)
        {
            var tableName = "User";
            var column = "UserId";

            var queryBuilder = GetInstance(type)
                .WhereIn(tableName, column, [1,2,3]);
            var query = queryBuilder.BuildQuery();

            string expected;
            if (type == typeof(SimplifySQLServerQueryBuilder))
                expected = "WHERE [User].[UserId] IN (@UserId00,@UserId01,@UserId02) ;";
            else if (type == typeof(SimplifyPostgresSQLQueryBuilder))
                expected = "WHERE \"User\".\"UserId\" IN (@UserId00,@UserId01,@UserId02) ;";
            else if (type == typeof(SimplifyMySQLQueryBuilder))
                expected = "WHERE `User`.`UserId` IN (@UserId00,@UserId01,@UserId02) ;";
            else
                throw new NotSupportedException("Not suppoted type");

            Assert.Equal(expected, query);
            SimplifyQueryBuilderAsserts.AssertParameter(queryBuilder, "UserId00", 1);
            SimplifyQueryBuilderAsserts.AssertParameter(queryBuilder, "UserId01", 2);
            SimplifyQueryBuilderAsserts.AssertParameter(queryBuilder, "UserId02", 3);
        }

        [Theory]
        [MemberData(nameof(Types))]
        public void WhereNotEqualsTest(Type type)
        {
            var tableName = "User";
            var column = "UserId";

            var queryBuilder = GetInstance(type)
                .WhereNotEquals(tableName, column, 1);
            var query = queryBuilder.BuildQuery();

            string expected;
            if (type == typeof(SimplifySQLServerQueryBuilder))
                expected = "WHERE [User].[UserId] <> @UserId0 ;";
            else if (type == typeof(SimplifyPostgresSQLQueryBuilder))
                expected = "WHERE \"User\".\"UserId\" <> @UserId0 ;";
            else if (type == typeof(SimplifyMySQLQueryBuilder))
                expected = "WHERE `User`.`UserId` <> @UserId0 ;";
            else
                throw new NotSupportedException("Not suppoted type");

            Assert.Equal(expected, query);
            SimplifyQueryBuilderAsserts.AssertParameter(queryBuilder, "UserId0", 1);
        }

        [Theory]
        [MemberData(nameof(Types))]
        public void WhereGreaterTest(Type type)
        {
            var tableName = "User";
            var column = "UserId";

            var queryBuilder = GetInstance(type)
                .WhereGreater(tableName, column, 1);
            var query = queryBuilder.BuildQuery();

            string expected;
            if (type == typeof(SimplifySQLServerQueryBuilder))
                expected = "WHERE [User].[UserId] > @UserId0 ;";
            else if (type == typeof(SimplifyPostgresSQLQueryBuilder))
                expected = "WHERE \"User\".\"UserId\" > @UserId0 ;";
            else if (type == typeof(SimplifyMySQLQueryBuilder))
                expected = "WHERE `User`.`UserId` > @UserId0 ;";
            else
                throw new NotSupportedException("Not supported type");

            Assert.Equal(expected, query);
            SimplifyQueryBuilderAsserts.AssertParameter(queryBuilder, "UserId0", 1);
        }

        [Theory]
        [MemberData(nameof(Types))]
        public void WhereGreaterOrEqualTest(Type type)
        {
            var tableName = "User";
            var column = "UserId";

            var queryBuilder = GetInstance(type)
                .WhereGreaterOrEqual(tableName, column, 1);
            var query = queryBuilder.BuildQuery();

            string expected;
            if (type == typeof(SimplifySQLServerQueryBuilder))
                expected = "WHERE [User].[UserId] >= @UserId0 ;";
            else if (type == typeof(SimplifyPostgresSQLQueryBuilder))
                expected = "WHERE \"User\".\"UserId\" >= @UserId0 ;";
            else if (type == typeof(SimplifyMySQLQueryBuilder))
                expected = "WHERE `User`.`UserId` >= @UserId0 ;";
            else
                throw new NotSupportedException("Not supported type");

            Assert.Equal(expected, query);
            SimplifyQueryBuilderAsserts.AssertParameter(queryBuilder, "UserId0", 1);
        }

        [Theory]
        [MemberData(nameof(Types))]
        public void WhereLowerTest(Type type)
        {
            var tableName = "User";
            var column = "UserId";

            var queryBuilder = GetInstance(type)
                .WhereLower(tableName, column, 1);
            var query = queryBuilder.BuildQuery();

            string expected;
            if (type == typeof(SimplifySQLServerQueryBuilder))
                expected = "WHERE [User].[UserId] < @UserId0 ;";
            else if (type == typeof(SimplifyPostgresSQLQueryBuilder))
                expected = "WHERE \"User\".\"UserId\" < @UserId0 ;";
            else if (type == typeof(SimplifyMySQLQueryBuilder))
                expected = "WHERE `User`.`UserId` < @UserId0 ;";
            else
                throw new NotSupportedException("Not supported type");

            Assert.Equal(expected, query);
            SimplifyQueryBuilderAsserts.AssertParameter(queryBuilder, "UserId0", 1);
        }

        [Theory]
        [MemberData(nameof(Types))]
        public void WhereLowerOrEqualTest(Type type)
        {
            var tableName = "User";
            var column = "UserId";

            var queryBuilder = GetInstance(type)
                .WhereLowerOrEqual(tableName, column, 1);
            var query = queryBuilder.BuildQuery();

            string expected;
            if (type == typeof(SimplifySQLServerQueryBuilder))
                expected = "WHERE [User].[UserId] <= @UserId0 ;";
            else if (type == typeof(SimplifyPostgresSQLQueryBuilder))
                expected = "WHERE \"User\".\"UserId\" <= @UserId0 ;";
            else if (type == typeof(SimplifyMySQLQueryBuilder))
                expected = "WHERE `User`.`UserId` <= @UserId0 ;";
            else
                throw new NotSupportedException("Not supported type");

            Assert.Equal(expected, query);
            SimplifyQueryBuilderAsserts.AssertParameter(queryBuilder, "UserId0", 1);
        }

        [Theory]
        [MemberData(nameof(Types))]
        public void WhereBetweenTest(Type type)
        {
            var tableName = "User";
            var column = "UserId";

            var queryBuilder = GetInstance(type)
                .WhereBetween(tableName, column, 1, 10);
            var query = queryBuilder.BuildQuery();

            string expected;
            if (type == typeof(SimplifySQLServerQueryBuilder))
                expected = "WHERE [User].[UserId] BETWEEN @UserId0 AND @UserId1 ;";
            else if (type == typeof(SimplifyPostgresSQLQueryBuilder))
                expected = "WHERE \"User\".\"UserId\" BETWEEN @UserId0 AND @UserId1 ;";
            else if (type == typeof(SimplifyMySQLQueryBuilder))
                expected = "WHERE `User`.`UserId` BETWEEN @UserId0 AND @UserId1 ;";
            else
                throw new NotSupportedException("Not supported type");

            Assert.Equal(expected, query);
            SimplifyQueryBuilderAsserts.AssertParameter(queryBuilder, "UserId0", 1);
            SimplifyQueryBuilderAsserts.AssertParameter(queryBuilder, "UserId1", 10);
        }

        #endregion

        #region And

        [Theory]
        [MemberData(nameof(Types))]
        public void AndEqualsTest(Type type)
        {
            var queryBuilder = GetInstance(type)
                .WhereEquals("User", "UserId", 1)
                .AndEquals("User", "Permission", 2);
            var query = queryBuilder.BuildQuery();

            string expected;
            if (type == typeof(SimplifySQLServerQueryBuilder))
                expected = "WHERE [User].[UserId] = @UserId0 AND [User].[Permission] = @Permission0 ;";
            else if (type == typeof(SimplifyPostgresSQLQueryBuilder))
                expected = "WHERE \"User\".\"UserId\" = @UserId0 AND \"User\".\"Permission\" = @Permission0 ;";
            else if (type == typeof(SimplifyMySQLQueryBuilder))
                expected = "WHERE `User`.`UserId` = @UserId0 AND `User`.`Permission` = @Permission0 ;";
            else
                throw new NotSupportedException("Not supported type");

            Assert.Equal(expected, query);

            SimplifyQueryBuilderAsserts.AssertParameter(queryBuilder, "UserId0", 1);
            SimplifyQueryBuilderAsserts.AssertParameter(queryBuilder, "Permission0", 2);
        }

        [Theory]
        [MemberData(nameof(Types))]
        public void AndNotEqualsTest(Type type)
        {
            var queryBuilder = GetInstance(type)
                .WhereEquals("User", "UserId", 1)
                .AndNotEquals("User", "Permission", 2);
            var query = queryBuilder.BuildQuery();

            string expected;
            if (type == typeof(SimplifySQLServerQueryBuilder))
                expected = "WHERE [User].[UserId] = @UserId0 AND [User].[Permission] <> @Permission0 ;";
            else if (type == typeof(SimplifyPostgresSQLQueryBuilder))
                expected = "WHERE \"User\".\"UserId\" = @UserId0 AND \"User\".\"Permission\" <> @Permission0 ;";
            else if (type == typeof(SimplifyMySQLQueryBuilder))
                expected = "WHERE `User`.`UserId` = @UserId0 AND `User`.`Permission` <> @Permission0 ;";
            else
                throw new NotSupportedException("Not supported type");

            Assert.Equal(expected, query);

            SimplifyQueryBuilderAsserts.AssertParameter(queryBuilder, "UserId0", 1);
            SimplifyQueryBuilderAsserts.AssertParameter(queryBuilder, "Permission0", 2);
        }

        [Theory]
        [MemberData(nameof(Types))]
        public void AndGreaterTest(Type type)
        {
            var queryBuilder = GetInstance(type)
                .WhereEquals("User", "UserId", 1)
                .AndGreater("User", "Permission", 2);
            var query = queryBuilder.BuildQuery();

            string expected;
            if (type == typeof(SimplifySQLServerQueryBuilder))
                expected = "WHERE [User].[UserId] = @UserId0 AND [User].[Permission] > @Permission0 ;";
            else if (type == typeof(SimplifyPostgresSQLQueryBuilder))
                expected = "WHERE \"User\".\"UserId\" = @UserId0 AND \"User\".\"Permission\" > @Permission0 ;";
            else if (type == typeof(SimplifyMySQLQueryBuilder))
                expected = "WHERE `User`.`UserId` = @UserId0 AND `User`.`Permission` > @Permission0 ;";
            else
                throw new NotSupportedException("Not supported type");

            Assert.Equal(expected, query);

            SimplifyQueryBuilderAsserts.AssertParameter(queryBuilder, "UserId0", 1);
            SimplifyQueryBuilderAsserts.AssertParameter(queryBuilder, "Permission0", 2);
        }


        [Theory]
        [MemberData(nameof(Types))]
        public void AndGreaterOrEqualsTest(Type type)
        {
            var queryBuilder = GetInstance(type)
                .WhereEquals("User", "UserId", 1)
                .AndGreaterOrEqual("User", "Permission", 2);
            var query = queryBuilder.BuildQuery();

            string expected;
            if (type == typeof(SimplifySQLServerQueryBuilder))
                expected = "WHERE [User].[UserId] = @UserId0 AND [User].[Permission] >= @Permission0 ;";
            else if (type == typeof(SimplifyPostgresSQLQueryBuilder))
                expected = "WHERE \"User\".\"UserId\" = @UserId0 AND \"User\".\"Permission\" >= @Permission0 ;";
            else if (type == typeof(SimplifyMySQLQueryBuilder))
                expected = "WHERE `User`.`UserId` = @UserId0 AND `User`.`Permission` >= @Permission0 ;";
            else
                throw new NotSupportedException("Not supported type");

            Assert.Equal(expected, query);

            SimplifyQueryBuilderAsserts.AssertParameter(queryBuilder, "UserId0", 1);
            SimplifyQueryBuilderAsserts.AssertParameter(queryBuilder, "Permission0", 2);
        }

        [Theory]
        [MemberData(nameof(Types))]
        public void AndLowerTest(Type type)
        {
            var queryBuilder = GetInstance(type)
                .WhereEquals("User", "UserId", 1)
                .AndLower("User", "Permission", 2);
            var query = queryBuilder.BuildQuery();

            string expected;
            if (type == typeof(SimplifySQLServerQueryBuilder))
                expected = "WHERE [User].[UserId] = @UserId0 AND [User].[Permission] < @Permission0 ;";
            else if (type == typeof(SimplifyPostgresSQLQueryBuilder))
                expected = "WHERE \"User\".\"UserId\" = @UserId0 AND \"User\".\"Permission\" < @Permission0 ;";
            else if (type == typeof(SimplifyMySQLQueryBuilder))
                expected = "WHERE `User`.`UserId` = @UserId0 AND `User`.`Permission` < @Permission0 ;";
            else
                throw new NotSupportedException("Not supported type");

            Assert.Equal(expected, query);

            SimplifyQueryBuilderAsserts.AssertParameter(queryBuilder, "UserId0", 1);
            SimplifyQueryBuilderAsserts.AssertParameter(queryBuilder, "Permission0", 2);
        }

        [Theory]
        [MemberData(nameof(Types))]
        public void AndLowerOrEqualsTest(Type type)
        {
            var queryBuilder = GetInstance(type)
                .WhereEquals("User", "UserId", 1)
                .AndLowerOrEqual("User", "Permission", 2);
            var query = queryBuilder.BuildQuery();

            string expected;
            if (type == typeof(SimplifySQLServerQueryBuilder))
                expected = "WHERE [User].[UserId] = @UserId0 AND [User].[Permission] <= @Permission0 ;";
            else if (type == typeof(SimplifyPostgresSQLQueryBuilder))
                expected = "WHERE \"User\".\"UserId\" = @UserId0 AND \"User\".\"Permission\" <= @Permission0 ;";
            else if (type == typeof(SimplifyMySQLQueryBuilder))
                expected = "WHERE `User`.`UserId` = @UserId0 AND `User`.`Permission` <= @Permission0 ;";
            else
                throw new NotSupportedException("Not supported type");

            Assert.Equal(expected, query);

            SimplifyQueryBuilderAsserts.AssertParameter(queryBuilder, "UserId0", 1);
            SimplifyQueryBuilderAsserts.AssertParameter(queryBuilder, "Permission0", 2);
        }

        [Theory]
        [MemberData(nameof(Types))]
        public void AndBetweenTest(Type type)
        {
            var queryBuilder = GetInstance(type)
                .WhereEquals("User", "UserId", 1)
                .AndBetween("User", "Permission", 10, 20);
            var query = queryBuilder.BuildQuery();

            string expected;
            if (type == typeof(SimplifySQLServerQueryBuilder))
                expected = "WHERE [User].[UserId] = @UserId0 AND [User].[Permission] BETWEEN @Permission0 AND @Permission1 ;";
            else if (type == typeof(SimplifyPostgresSQLQueryBuilder))
                expected = "WHERE \"User\".\"UserId\" = @UserId0 AND \"User\".\"Permission\" BETWEEN @Permission0 AND @Permission1 ;";
            else if (type == typeof(SimplifyMySQLQueryBuilder))
                expected = "WHERE `User`.`UserId` = @UserId0 AND `User`.`Permission` BETWEEN @Permission0 AND @Permission1 ;";
            else
                throw new NotSupportedException("Not supported type");

            Assert.Equal(expected, query);

            SimplifyQueryBuilderAsserts.AssertParameter(queryBuilder, "UserId0", 1);
            SimplifyQueryBuilderAsserts.AssertParameter(queryBuilder, "Permission0", 10);
            SimplifyQueryBuilderAsserts.AssertParameter(queryBuilder, "Permission1", 20);
        }

        #endregion

        #region Or

        [Theory]
        [MemberData(nameof(Types))]
        public void OrEqualsTest(Type type)
        {
            var queryBuilder = GetInstance(type)
                .WhereEquals("User", "UserId", 1)
                .OrEquals("User", "Permission", 2);
            var query = queryBuilder.BuildQuery();

            string expected;
            if (type == typeof(SimplifySQLServerQueryBuilder))
                expected = "WHERE [User].[UserId] = @UserId0 OR [User].[Permission] = @Permission0 ;";
            else if (type == typeof(SimplifyPostgresSQLQueryBuilder))
                expected = "WHERE \"User\".\"UserId\" = @UserId0 OR \"User\".\"Permission\" = @Permission0 ;";
            else if (type == typeof(SimplifyMySQLQueryBuilder))
                expected = "WHERE `User`.`UserId` = @UserId0 OR `User`.`Permission` = @Permission0 ;";
            else
                throw new NotSupportedException("Not supported type");

            Assert.Equal(expected, query);
            SimplifyQueryBuilderAsserts.AssertParameter(queryBuilder, "UserId0", 1);
            SimplifyQueryBuilderAsserts.AssertParameter(queryBuilder, "Permission0", 2);
        }

        [Theory]
        [MemberData(nameof(Types))]
        public void OrNotEqualsTest(Type type)
        {
            var queryBuilder = GetInstance(type)
                .WhereEquals("User", "UserId", 1)
                .OrNotEquals("User", "Permission", 2);
            var query = queryBuilder.BuildQuery();

            string expected;
            if (type == typeof(SimplifySQLServerQueryBuilder))
                expected = "WHERE [User].[UserId] = @UserId0 OR [User].[Permission] <> @Permission0 ;";
            else if (type == typeof(SimplifyPostgresSQLQueryBuilder))
                expected = "WHERE \"User\".\"UserId\" = @UserId0 OR \"User\".\"Permission\" <> @Permission0 ;";
            else if (type == typeof(SimplifyMySQLQueryBuilder))
                expected = "WHERE `User`.`UserId` = @UserId0 OR `User`.`Permission` <> @Permission0 ;";
            else
                throw new NotSupportedException("Not supported type");

            Assert.Equal(expected, query);
            SimplifyQueryBuilderAsserts.AssertParameter(queryBuilder, "UserId0", 1);
            SimplifyQueryBuilderAsserts.AssertParameter(queryBuilder, "Permission0", 2);
        }

        [Theory]
        [MemberData(nameof(Types))]
        public void OrGreaterTest(Type type)
        {
            var queryBuilder = GetInstance(type)
                .WhereEquals("User", "UserId", 1)
                .OrGreater("User", "Permission", 2);
            var query = queryBuilder.BuildQuery();

            string expected;
            if (type == typeof(SimplifySQLServerQueryBuilder))
                expected = "WHERE [User].[UserId] = @UserId0 OR [User].[Permission] > @Permission0 ;";
            else if (type == typeof(SimplifyPostgresSQLQueryBuilder))
                expected = "WHERE \"User\".\"UserId\" = @UserId0 OR \"User\".\"Permission\" > @Permission0 ;";
            else if (type == typeof(SimplifyMySQLQueryBuilder))
                expected = "WHERE `User`.`UserId` = @UserId0 OR `User`.`Permission` > @Permission0 ;";
            else
                throw new NotSupportedException("Not supported type");

            Assert.Equal(expected, query);
            SimplifyQueryBuilderAsserts.AssertParameter(queryBuilder, "UserId0", 1);
            SimplifyQueryBuilderAsserts.AssertParameter(queryBuilder, "Permission0", 2);
        }


        [Theory]
        [MemberData(nameof(Types))]
        public void OrGreaterOrEqualsTest(Type type)
        {
            var queryBuilder = GetInstance(type)
                .WhereEquals("User", "UserId", 1)
                .OrGreaterEqual("User", "Permission", 2);
            var query = queryBuilder.BuildQuery();

            string expected;
            if (type == typeof(SimplifySQLServerQueryBuilder))
                expected = "WHERE [User].[UserId] = @UserId0 OR [User].[Permission] >= @Permission0 ;";
            else if (type == typeof(SimplifyPostgresSQLQueryBuilder))
                expected = "WHERE \"User\".\"UserId\" = @UserId0 OR \"User\".\"Permission\" >= @Permission0 ;";
            else if (type == typeof(SimplifyMySQLQueryBuilder))
                expected = "WHERE `User`.`UserId` = @UserId0 OR `User`.`Permission` >= @Permission0 ;";
            else
                throw new NotSupportedException("Not supported type");

            var actual = query;

            Assert.Equal(expected, actual);
            SimplifyQueryBuilderAsserts.AssertParameter(queryBuilder, "UserId0", 1);
            SimplifyQueryBuilderAsserts.AssertParameter(queryBuilder, "Permission0", 2);
        }

        [Theory]
        [MemberData(nameof(Types))]
        public void OrLowerTest(Type type)
        {
            var queryBuilder = GetInstance(type)
                .WhereEquals("User", "UserId", 1)
                .OrLower("User", "Permission", 2);
            var query = queryBuilder.BuildQuery();

            string expected;
            if (type == typeof(SimplifySQLServerQueryBuilder))
                expected = "WHERE [User].[UserId] = @UserId0 OR [User].[Permission] < @Permission0 ;";
            else if (type == typeof(SimplifyPostgresSQLQueryBuilder))
                expected = "WHERE \"User\".\"UserId\" = @UserId0 OR \"User\".\"Permission\" < @Permission0 ;";
            else if (type == typeof(SimplifyMySQLQueryBuilder))
                expected = "WHERE `User`.`UserId` = @UserId0 OR `User`.`Permission` < @Permission0 ;";
            else
                throw new NotSupportedException("Not supported type");

            var actual = query;

            Assert.Equal(expected, actual);
            SimplifyQueryBuilderAsserts.AssertParameter(queryBuilder, "UserId0", 1);
            SimplifyQueryBuilderAsserts.AssertParameter(queryBuilder, "Permission0", 2);
        }

        [Theory]
        [MemberData(nameof(Types))]
        public void OrLowerOrEqualsTest(Type type)
        {
            var queryBuilder = GetInstance(type)
                .WhereEquals("User", "UserId", 1)
                .OrLowerEqual("User", "Permission", 2);
            var query = queryBuilder.BuildQuery();

            string expected;
            if (type == typeof(SimplifySQLServerQueryBuilder))
                expected = "WHERE [User].[UserId] = @UserId0 OR [User].[Permission] <= @Permission0 ;";
            else if (type == typeof(SimplifyPostgresSQLQueryBuilder))
                expected = "WHERE \"User\".\"UserId\" = @UserId0 OR \"User\".\"Permission\" <= @Permission0 ;";
            else if (type == typeof(SimplifyMySQLQueryBuilder))
                expected = "WHERE `User`.`UserId` = @UserId0 OR `User`.`Permission` <= @Permission0 ;";
            else
                throw new NotSupportedException("Not supported type");

            var actual = query;

            Assert.Equal(expected, actual);
            SimplifyQueryBuilderAsserts.AssertParameter(queryBuilder, "UserId0", 1);
            SimplifyQueryBuilderAsserts.AssertParameter(queryBuilder, "Permission0", 2);
        }

        [Theory]
        [MemberData(nameof(Types))]
        public void OrBetweenTest(Type type)
        {
            var queryBuilder = GetInstance(type)
                .WhereEquals("User", "UserId", 1)
                .OrBetween("User", "Permission", 10, 20);
            var query = queryBuilder.BuildQuery();

            string expected;
            if (type == typeof(SimplifySQLServerQueryBuilder))
                expected = "WHERE [User].[UserId] = @UserId0 OR [User].[Permission] BETWEEN @Permission0 AND @Permission1 ;";
            else if (type == typeof(SimplifyPostgresSQLQueryBuilder))
                expected = "WHERE \"User\".\"UserId\" = @UserId0 OR \"User\".\"Permission\" BETWEEN @Permission0 AND @Permission1 ;";
            else if (type == typeof(SimplifyMySQLQueryBuilder))
                expected = "WHERE `User`.`UserId` = @UserId0 OR `User`.`Permission` BETWEEN @Permission0 AND @Permission1 ;";
            else
                throw new NotSupportedException("Not supported type");

            var actual = query;

            Assert.Equal(expected, actual);
            SimplifyQueryBuilderAsserts.AssertParameter(queryBuilder, "UserId0", 1);
            SimplifyQueryBuilderAsserts.AssertParameter(queryBuilder, "Permission0", 10);
            SimplifyQueryBuilderAsserts.AssertParameter(queryBuilder, "Permission1", 20);
        }


        [Theory]
        [MemberData(nameof(Types))]
        public void ComplexOrTest(Type type)
        {
            var queryBuilder = GetInstance(type)
                .WhereEquals("User", "UserId", 1)
                .OrEquals("User", "Permission", 10)
                .OrNotEquals("User", "Permission", 11)
                .OrGreater("User", "Permission", 12)
                .OrGreaterEqual("User", "Permission", 13)
                .OrLower("User", "Permission", 14)
                .OrLowerEqual("User", "Permission", 15);
            var query = queryBuilder.BuildQuery();

            string expected;
            if (type == typeof(SimplifySQLServerQueryBuilder))
                expected = "WHERE [User].[UserId] = @UserId0 OR [User].[Permission] = @Permission0 OR [User].[Permission] <> @Permission1 OR [User].[Permission] > @Permission2 OR [User].[Permission] >= @Permission3 OR [User].[Permission] < @Permission4 OR [User].[Permission] <= @Permission5 ;";
            else if (type == typeof(SimplifyPostgresSQLQueryBuilder))
                expected = "WHERE \"User\".\"UserId\" = @UserId0 OR \"User\".\"Permission\" = @Permission0 OR \"User\".\"Permission\" <> @Permission1 OR \"User\".\"Permission\" > @Permission2 OR \"User\".\"Permission\" >= @Permission3 OR \"User\".\"Permission\" < @Permission4 OR \"User\".\"Permission\" <= @Permission5 ;";
            else if (type == typeof(SimplifyMySQLQueryBuilder))
                expected = "WHERE `User`.`UserId` = @UserId0 OR `User`.`Permission` = @Permission0 OR `User`.`Permission` <> @Permission1 OR `User`.`Permission` > @Permission2 OR `User`.`Permission` >= @Permission3 OR `User`.`Permission` < @Permission4 OR `User`.`Permission` <= @Permission5 ;";
            else
                throw new NotSupportedException("Not supported type");

            Assert.Equal(expected, query);
            SimplifyQueryBuilderAsserts.AssertParameter(queryBuilder, "UserId0", 1);
            SimplifyQueryBuilderAsserts.AssertParameter(queryBuilder, "Permission0", 10);
            SimplifyQueryBuilderAsserts.AssertParameter(queryBuilder, "Permission1", 11);
            SimplifyQueryBuilderAsserts.AssertParameter(queryBuilder, "Permission2", 12);
            SimplifyQueryBuilderAsserts.AssertParameter(queryBuilder, "Permission3", 13);
            SimplifyQueryBuilderAsserts.AssertParameter(queryBuilder, "Permission4", 14);
            SimplifyQueryBuilderAsserts.AssertParameter(queryBuilder, "Permission5", 15);
        }

        #endregion

        #region Order By

        [Theory]
        [MemberData(nameof(Types))]
        public void OrderByTest(Type type)
        {
            var queryBuilder = GetInstance(type)
                .OrderBy("User", "Username", SimplifyOrderOperation.Asc)
                .OrderBy("User", "Name", SimplifyOrderOperation.Desc);
            var query = queryBuilder.BuildQuery();

            string expected;
            if (type == typeof(SimplifySQLServerQueryBuilder))
                expected = "ORDER BY [User].[Username] ASC , [User].[Name] DESC ;";
            else if (type == typeof(SimplifyPostgresSQLQueryBuilder))
                expected = "ORDER BY \"User\".\"Username\" ASC , \"User\".\"Name\" DESC ;";
            else if (type == typeof(SimplifyMySQLQueryBuilder))
                expected = "ORDER BY `User`.`Username` ASC , `User`.`Name` DESC ;";
            else
                throw new NotSupportedException("Not supported type");

            Assert.Equal(expected, query);
        }

        #endregion

        [Theory]
        [MemberData(nameof(Types))]
        public void QueryTest(Type type)
        {
            var queryBuilder = GetInstance(type)
                .SelectFields("User", ["UserId", "Username", "Password"])
                .From("User")
                .InnerJoin("Permission", "UserId", "User", "UserId")
                .WhereEquals("User", "UserId", 1)
                .AndEquals("Permission", "Permission", 2)
                .OrGreater("Permission", "Permission", 10)
                .AndLower("Permission", "Permission", 50);
            var query = queryBuilder.BuildQuery();

            string expected;
            if (type == typeof(SimplifySQLServerQueryBuilder))
                expected = "SELECT [User].[UserId], [User].[Username], [User].[Password] FROM [User] INNER JOIN [Permission] ON [Permission].[UserId] = [User].[UserId] WHERE [User].[UserId] = @UserId0 AND [Permission].[Permission] = @Permission0 OR [Permission].[Permission] > @Permission1 AND [Permission].[Permission] < @Permission2 ;";
            else if (type == typeof(SimplifyPostgresSQLQueryBuilder))
                expected = "SELECT \"User\".\"UserId\", \"User\".\"Username\", \"User\".\"Password\" FROM \"User\" INNER JOIN \"Permission\" ON \"Permission\".\"UserId\" = \"User\".\"UserId\" WHERE \"User\".\"UserId\" = @UserId0 AND \"Permission\".\"Permission\" = @Permission0 OR \"Permission\".\"Permission\" > @Permission1 AND \"Permission\".\"Permission\" < @Permission2 ;";
            else if (type == typeof(SimplifyMySQLQueryBuilder))
                expected = "SELECT `User`.`UserId`, `User`.`Username`, `User`.`Password` FROM `User` INNER JOIN `Permission` ON `Permission`.`UserId` = `User`.`UserId` WHERE `User`.`UserId` = @UserId0 AND `Permission`.`Permission` = @Permission0 OR `Permission`.`Permission` > @Permission1 AND `Permission`.`Permission` < @Permission2 ;";
            else
                throw new NotSupportedException("Not supported type");
            Assert.Equal(expected, query);
        }
    }
}