using Simplify.ORM.Enumerations;

namespace Simplify.ORM.Interfaces
{
    public interface ISimplifyQueryBuilder
    {
        string BuildQuery();
        Dictionary<string, object?> GetParameters();

        string FormatTable(string table);
        string FormatColumn(string column);
        string FormatTableColumn(string table, string column);
        string FormatParameterName(string parameter);

        string TableName<T>() where T : SimplifyEntity;
        string ColumnName<T>(string property) where T : SimplifyEntity;

        ISimplifyQueryBuilder AddParameter(string name, object value);
        ISimplifyQueryBuilder AddSelect(string table, string column);
        ISimplifyQueryBuilder AddSelect(Tuple<string, string> tableColumn);
        ISimplifyQueryBuilder AddSelect(List<Tuple<string, string>> tableColumns);
        ISimplifyQueryBuilder AddTop(int top);
        ISimplifyQueryBuilder AddFrom(string table, string? alias = null);
        ISimplifyQueryBuilder AddJoin(SimplifyJoinOperation operation, string table);
        ISimplifyQueryBuilder AddJoin(SimplifyJoinOperation operation, string table, string column);
        ISimplifyQueryBuilder AddWhere(SimplifyWhereOperation operation);
        ISimplifyQueryBuilder AddWhere(SimplifyWhereOperation operation, string table, string column, object value);
        ISimplifyQueryBuilder AddWhere(SimplifyWhereOperation operation, string parameter, object value);
        ISimplifyQueryBuilder AddOrderBy(string table, string column, SimplifyOrderOperation operation = SimplifyOrderOperation.Asc);
        ISimplifyQueryBuilder AddLimit(int limit);


        ISimplifyQueryBuilder SelectFields(string table, List<string> columns);
        ISimplifyQueryBuilder SelectAllFields(string table);
        ISimplifyQueryBuilder Top(int top);
        ISimplifyQueryBuilder Limit(int limit);
        ISimplifyQueryBuilder From(string table, string? alias = null);
        ISimplifyQueryBuilder Join(string rightTable, string rightColumn, string leftTable, string leftColumn);
        ISimplifyQueryBuilder InnerJoin(string rightTable, string rightColumn, string leftTable, string leftColumn);
        ISimplifyQueryBuilder LeftJoin(string rightTable, string rightColumn, string leftTable, string leftColumn);
        ISimplifyQueryBuilder RightJoin(string rightTable, string rightColumn, string leftTable, string leftColumn);
        ISimplifyQueryBuilder WhereEquals(string table, string column, object? value, bool conditional = true);
        ISimplifyQueryBuilder WhereNotEquals(string table, string column, object value, bool conditional = true);
        ISimplifyQueryBuilder WhereGreater(string table, string column, object value, bool conditional = true);
        ISimplifyQueryBuilder WhereGreaterOrEqual(string table, string column, object value, bool conditional = true);
        ISimplifyQueryBuilder WhereLower(string table, string column, object value, bool conditional = true);
        ISimplifyQueryBuilder WhereLowerOrEqual(string table, string column, object value, bool conditional = true);
        ISimplifyQueryBuilder WhereBetween(string table, string column, object from, object to, bool conditional = true);
        ISimplifyQueryBuilder AndEquals(string table, string column, object value, bool conditional = true);
        ISimplifyQueryBuilder AndNotEquals(string table, string column, object value, bool conditional = true);
        ISimplifyQueryBuilder AndGreater(string table, string column, object value, bool conditional = true);
        ISimplifyQueryBuilder AndGreaterOrEqual(string table, string column, object value, bool conditional = true);
        ISimplifyQueryBuilder AndLower(string table, string column, object value, bool conditional = true);
        ISimplifyQueryBuilder AndLowerOrEqual(string table, string column, object value, bool conditional = true);
        ISimplifyQueryBuilder AndBetween(string table, string column, object from, object to, bool conditional = true);
        ISimplifyQueryBuilder WhereIn(string table, string column, IEnumerable<object> value, bool conditional = true);
        ISimplifyQueryBuilder OrEquals(string table, string column, object value, bool conditional = true);
        ISimplifyQueryBuilder OrNotEquals(string table, string column, object value, bool conditional = true);
        ISimplifyQueryBuilder OrGreater(string table, string column, object value, bool conditional = true);
        ISimplifyQueryBuilder OrGreaterEqual(string table, string column, object value, bool conditional = true);
        ISimplifyQueryBuilder OrLower(string table, string column, object value, bool conditional = true);
        ISimplifyQueryBuilder OrLowerEqual(string table, string column, object value, bool conditional = true);
        ISimplifyQueryBuilder OrBetween(string table, string column, object from, object to, bool conditional = true);
        ISimplifyQueryBuilder OrderBy(string table, string column, SimplifyOrderOperation operation = SimplifyOrderOperation.Desc);
    }
}
