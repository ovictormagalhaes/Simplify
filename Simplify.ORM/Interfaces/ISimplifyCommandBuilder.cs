using Simplify.ORM.Enumerations;

namespace Simplify.ORM.Interfaces
{
    public interface ISimplifyCommandBuilder
    {
        string BuildQuery();
        Dictionary<string, object> GetParameters();

        ISimplifyCommandBuilder AddInsert(ISimplifyEntity entity);
        ISimplifyCommandBuilder AddInsert(string table, Dictionary<string, object> columnValues);

        ISimplifyCommandBuilder AddUpdate(ISimplifyEntity entity, List<WhereOperation> whereOperations);
        ISimplifyCommandBuilder AddUpdate(string table, Dictionary<string, object> columnValues, List<WhereOperation> whereOperations);

        string GetWhereOperationSymbol(SimplifyWhereOperation operation);
    }
}
