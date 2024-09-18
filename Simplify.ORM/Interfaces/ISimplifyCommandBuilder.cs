using Simplify.ORM.Enumerations;

namespace Simplify.ORM.Interfaces
{
    public interface ISimplifyCommandBuilder
    {
        string BuildQuery();
        Dictionary<string, object> GetParameters();

        string FormatTable(string table);
        string FormatColumn(string column);

        ISimplifyCommandBuilder AddInsert(ISimplifyEntity entity);
        ISimplifyCommandBuilder AddInsert(string table, Dictionary<string, object> columnValues);

        ISimplifyCommandBuilder AddUpdate(ISimplifyEntity entity, List<WhereOperation> whereOperations);
        ISimplifyCommandBuilder AddUpdate(string table, Dictionary<string, object> columnValues, List<WhereOperation> whereOperations);
        ISimplifyCommandBuilder AddUpdateWhereEquals(string table, Dictionary<string, object> columnValues, string column, object value);

        string GetWhereOperationSymbol(SimplifyWhereOperation operation);
    }
}
