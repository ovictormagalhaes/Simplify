namespace Simplify.ORM.Interfaces
{
    public interface ISimplifyEntity
    {
        string GetTableName();
        public string? GetColumnName(string property);
        IEnumerable<SimplifyEntityProperty> GetProperties();
        Dictionary<string, object> GetColumnValues();
    }
}
