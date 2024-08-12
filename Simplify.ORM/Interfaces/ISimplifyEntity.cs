namespace Simplify.ORM.Interfaces
{
    public interface ISimplifyEntity
    {
        string GetTableName();
        Dictionary<string, object> GetColumnValues();
        IEnumerable<string> GetColumns();
    }
}
