/*namespace Simplify.ORM
{
    public static class SimplifyQueryExtensions
    {
        #region Select

        public static T SelectFields<T>(this T query, string tableName, List<string> columns) where T : ISimplifyQuery
        {
            var selects = columns.Select(c => new Tuple<string, string>(tableName, c)).ToList();

            return (T)query.AddSelect(selects);
        }

        public static T SelectAllFields<T>(this T query, string tableName) where T : ISimplifyQuery
            => (T)query.AddSelect(tableName, string.Empty);

        public static T Top<T>(this T query, int top) where T : ISimplifyQuery
        {
            return (T)query.AddTop(top);
        }

        public static T Limit<T>(this T query, int limit) where T : ISimplifyQuery
        {
            return (T)query.AddLimit(limit);
        }

        #endregion

        #region Join

        private static T JoinBase<T>(this T query, SimplifyJoinOperation operation, string rightTable, string rightColumn, string leftTable, string leftColumn) where T : ISimplifyQuery
        {
            return (T)query
                .AddJoin(operation, rightTable)
                .AddJoin(SimplifyJoinOperation.On, rightTable, rightColumn)
                .AddJoin(SimplifyJoinOperation.Equals, leftTable, leftColumn);
        }

        public static T From<T>(this T query, string tableName, string? alias = null) where T : ISimplifyQuery
            => (T)query.AddFrom(tableName, alias);

        public static T Join<T>(this T query, string rightTable, string rightColumn, string leftTable, string leftColumn) where T : ISimplifyQuery
            => query.JoinBase(SimplifyJoinOperation.Join, rightTable, rightColumn, leftTable, leftColumn);

        public static T InnerJoin<T>(this T query, string rightTable, string rightColumn, string leftTable, string leftColumn) where T : ISimplifyQuery
            => query.JoinBase(SimplifyJoinOperation.InnerJoin, rightTable, rightColumn, leftTable, leftColumn);

        public static T LeftJoin<T>(this T query, string rightTable, string rightColumn, string leftTable, string leftColumn) where T : ISimplifyQuery
            => query.JoinBase(SimplifyJoinOperation.LeftJoin, rightTable, rightColumn, leftTable, leftColumn);

        public static T RightJoin<T>(this T query, string rightTable, string rightColumn, string leftTable, string leftColumn) where T : ISimplifyQuery
            => query.JoinBase(SimplifyJoinOperation.RightJoin, rightTable, rightColumn, leftTable, leftColumn);

        #endregion

        #region Where

        private static T WhereBase<T>(this T query, SimplifyWhereOperation operation, string tableName, string column, object value, bool conditional) where T : ISimplifyQuery
        {
            if (!conditional) { return query; }

            return (T)query.AddWhere(operation, tableName, column, value);
        }

        public static T WhereEquals<T>(this T query, string tableName, string column, object value, bool conditional = true) where T : ISimplifyQuery
            => query.WhereBase(SimplifyWhereOperation.Equals, tableName, column, value, conditional);

        public static T WhereNotEquals<T>(this T query, string tableName, string column, object value, bool conditional = true) where T : ISimplifyQuery
            => query.WhereBase(SimplifyWhereOperation.NotEquals, tableName, column, value, conditional);

        public static T WhereGreater<T>(this T query, string tableName, string column, object value, bool conditional = true) where T : ISimplifyQuery
            => query.WhereBase(SimplifyWhereOperation.Greater, tableName, column, value, conditional);

        public static T WhereGreaterOrEqual<T>(this T query, string tableName, string column, object value, bool conditional = true) where T : ISimplifyQuery
            => query.WhereBase(SimplifyWhereOperation.GreaterOrEqual, tableName, column, value, conditional);

        public static T WhereLower<T>(this T query, string tableName, string column, object value, bool conditional = true) where T : ISimplifyQuery
            => query.WhereBase(SimplifyWhereOperation.Lower, tableName, column, value, conditional);

        public static T WhereLowerOrEqual<T>(this T query, string tableName, string column, object value, bool conditional = true) where T : ISimplifyQuery
            => query.WhereBase(SimplifyWhereOperation.LowerOrEqual, tableName, column, value, conditional);

        public static T WhereBetween<T>(this T query, string tableName, string column, object from, object to, bool conditional = true) where T : ISimplifyQuery
        {
            if (!conditional) { return query; }

            return (T)query.AddWhere(SimplifyWhereOperation.Between, tableName, column, from).AddWhere(SimplifyWhereOperation.And, column, to);
        }

        #endregion

        #region And

        private static T AndBase<T>(this T query, SimplifyWhereOperation operation, string tableName, string column, object value, bool conditional) where T : ISimplifyQuery
        {
            if (!conditional) { return query; }

            return (T)query.AddWhere(SimplifyWhereOperation.And).AddWhere(operation, tableName, column, value);
        }

        public static T AndEquals<T>(this T query, string tableName, string column, object value, bool conditional = true) where T : ISimplifyQuery
            => query.AndBase(SimplifyWhereOperation.Equals, tableName, column, value, conditional);

        public static T AndNotEquals<T>(this T query, string tableName, string column, object value, bool conditional = true) where T : ISimplifyQuery
            => query.AndBase(SimplifyWhereOperation.NotEquals, tableName, column, value, conditional);

        public static T AndGreater<T>(this T query, string tableName, string column, object value, bool conditional = true) where T : ISimplifyQuery
            => query.AndBase(SimplifyWhereOperation.Greater, tableName, column, value, conditional);

        public static T AndGreaterOrEqual<T>(this T query, string tableName, string column, object value, bool conditional = true) where T : ISimplifyQuery
            => query.AndBase(SimplifyWhereOperation.GreaterOrEqual, tableName, column, value, conditional);

        public static T AndLower<T>(this T query, string tableName, string column, object value, bool conditional = true) where T : ISimplifyQuery
            => query.AndBase(SimplifyWhereOperation.Lower, tableName, column, value, conditional);

        public static T AndLowerOrEqual<T>(this T query, string tableName, string column, object value, bool conditional = true) where T : ISimplifyQuery
            => query.AndBase(SimplifyWhereOperation.LowerOrEqual, tableName, column, value, conditional);

        public static T AndBetween<T>(this T query, string tableName, string column, object from, object to, bool conditional = true) where T : ISimplifyQuery
            => (T)query.AndBase(SimplifyWhereOperation.Between, tableName, column, from, conditional).AddWhere(SimplifyWhereOperation.And, column, to);

        #endregion

        #region Or
        private static T OrBase<T>(this T query, SimplifyWhereOperation operation, string tableName, string column, object value, bool conditional) where T : ISimplifyQuery
        {
            if (!conditional) { return query; }

            return (T)query.AddWhere(SimplifyWhereOperation.Or).AddWhere(operation, tableName, column, value);
        }

        public static T OrEquals<T>(this T query, string tableName, string column, object value, bool conditional = true) where T : ISimplifyQuery
            => query.OrBase(SimplifyWhereOperation.Equals, tableName, column, value, conditional);

        public static T OrNotEquals<T>(this T query, string tableName, string column, object value, bool conditional = true) where T : ISimplifyQuery
            => query.OrBase(SimplifyWhereOperation.NotEquals, tableName, column, value, conditional);

        public static T OrGreater<T>(this T query, string tableName, string column, object value, bool conditional = true) where T : ISimplifyQuery
            => query.OrBase(SimplifyWhereOperation.Greater, tableName, column, value, conditional);

        public static T OrGreaterEqual<T>(this T query, string tableName, string column, object value, bool conditional = true) where T : ISimplifyQuery
            => query.OrBase(SimplifyWhereOperation.GreaterOrEqual, tableName, column, value, conditional);

        public static T OrLower<T>(this T query, string tableName, string column, object value, bool conditional = true) where T : ISimplifyQuery
            => query.OrBase(SimplifyWhereOperation.Lower, tableName, column, value, conditional);

        public static T OrLowerEqual<T>(this T query, string tableName, string column, object value, bool conditional = true) where T : ISimplifyQuery
            => query.OrBase(SimplifyWhereOperation.LowerOrEqual, tableName, column, value, conditional);

        public static T OrBetween<T>(this T query, string tableName, string column, object from, object to, bool conditional = true) where T : ISimplifyQuery
            => (T)query.OrBase(SimplifyWhereOperation.Between, tableName, column, from, conditional).AddWhere(SimplifyWhereOperation.And, column, to);

        #endregion

        #region Order By
        public static T OrderBy<T>(this T query, string table, string column, SimplifyOrderOperation operation = SimplifyOrderOperation.Desc) where T : ISimplifyQuery
        {
            return (T)query.AddOrderBy(table, column, operation);
        }

        #endregion
    }
}
*/