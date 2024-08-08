namespace Simplify.ORM.Interfaces
{
    public interface ISimplifyCommandBuilder
    {
        string BuildInsertQuery(string table, Dictionary<string, object> columnValues);
    }
}
