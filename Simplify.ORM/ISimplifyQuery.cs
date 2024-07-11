namespace Simplify.ORM
{
    public interface ISimplifyQuery
    {
        string BuildQuery();
        Dictionary<string, object> GetParameters();

        string FormatTable(string table);
        string FormatColumn(string column);
        string FormatTableColumn(string table, string column);
        string FormatParameterName(string parameter);

        ISimplifyQuery AddParameter(string name, object value);
        ISimplifyQuery AddSelect(string table, string column);
        ISimplifyQuery AddSelect(Tuple<string, string> tableColumn);
        ISimplifyQuery AddSelect(List<Tuple<string, string>> tableColumns);
        ISimplifyQuery AddTop(int top);
        ISimplifyQuery AddFrom(string table, string? alias = null);
        ISimplifyQuery AddJoin(SimplifyJoinOperation operation, string table);
        ISimplifyQuery AddJoin(SimplifyJoinOperation operation, string table, string column);
        ISimplifyQuery AddWhere(SimplifyWhereOperation operation);
        ISimplifyQuery AddWhere(SimplifyWhereOperation operation, string table, string column, object value);
        ISimplifyQuery AddWhere(SimplifyWhereOperation operation, string parameter, object value);
        ISimplifyQuery AddOrderBy(string table, string column, SimplifyOrderOperation operation = SimplifyOrderOperation.Asc);
        ISimplifyQuery AddLimit(int limit);


        ISimplifyQuery SelectFields(string tableName, List<string> columns);
        ISimplifyQuery SelectAllFields(string tableName);
        ISimplifyQuery Top(int top);
        ISimplifyQuery Limit(int limit);
        ISimplifyQuery From(string tableName, string? alias = null);
        ISimplifyQuery Join(string rightTable, string rightColumn, string leftTable, string leftColumn);
        ISimplifyQuery InnerJoin(string rightTable, string rightColumn, string leftTable, string leftColumn);
        ISimplifyQuery LeftJoin(string rightTable, string rightColumn, string leftTable, string leftColumn);
        ISimplifyQuery RightJoin(string rightTable, string rightColumn, string leftTable, string leftColumn);
        ISimplifyQuery WhereEquals(string tableName, string column, object value, bool conditional = true);
        ISimplifyQuery WhereNotEquals(string tableName, string column, object value, bool conditional = true);
        ISimplifyQuery WhereGreater(string tableName, string column, object value, bool conditional = true);
        ISimplifyQuery WhereGreaterOrEqual(string tableName, string column, object value, bool conditional = true);
        ISimplifyQuery WhereLower(string tableName, string column, object value, bool conditional = true);
        ISimplifyQuery WhereLowerOrEqual(string tableName, string column, object value, bool conditional = true);
        ISimplifyQuery WhereBetween(string tableName, string column, object from, object to, bool conditional = true);
        ISimplifyQuery AndEquals(string tableName, string column, object value, bool conditional = true);
        ISimplifyQuery AndNotEquals(string tableName, string column, object value, bool conditional = true);
        ISimplifyQuery AndGreater(string tableName, string column, object value, bool conditional = true);
        ISimplifyQuery AndGreaterOrEqual(string tableName, string column, object value, bool conditional = true);
        ISimplifyQuery AndLower(string tableName, string column, object value, bool conditional = true);
        ISimplifyQuery AndLowerOrEqual(string tableName, string column, object value, bool conditional = true);
        ISimplifyQuery AndBetween(string tableName, string column, object from, object to, bool conditional = true);
        ISimplifyQuery OrEquals(string tableName, string column, object value, bool conditional = true);
        ISimplifyQuery OrNotEquals(string tableName, string column, object value, bool conditional = true);
        ISimplifyQuery OrGreater(string tableName, string column, object value, bool conditional = true);
        ISimplifyQuery OrGreaterEqual(string tableName, string column, object value, bool conditional = true);
        ISimplifyQuery OrLower(string tableName, string column, object value, bool conditional = true);
        ISimplifyQuery OrLowerEqual(string tableName, string column, object value, bool conditional = true);
        ISimplifyQuery OrBetween(string tableName, string column, object from, object to, bool conditional = true);
        ISimplifyQuery OrderBy(string table, string column, SimplifyOrderOperation operation = SimplifyOrderOperation.Desc);
    }

    public enum SimplifyWhereOperation
    {
        Where,
        And,
        Or,
        Equals,
        NotEquals,
        Greater,
        GreaterOrEqual,
        Lower,
        LowerOrEqual,
        Between
    }

    public enum SimplifyJoinOperation
    {
        From,
        On,
        Equals,
        Join,
        InnerJoin,
        LeftJoin,
        RightJoin,
        FullJoin,
        CrossJoin,
        LeftOuterJoin,
        RightOuterJoin,
        FullOuterJoin,
        NaturalJoin
    }

    public enum SimplifyOrderOperation
    {
        Asc,
        Desc,
    }
}
