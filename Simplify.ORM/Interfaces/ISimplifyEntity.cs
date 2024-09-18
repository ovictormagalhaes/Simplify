namespace Simplify.ORM.Interfaces
{
    public interface ISimplifyEntity
    {
        bool IsPersisted { get; set; }

        string GetTableName();
        public string GetColumnName(string property);
        List<SimplifyEntityProperty> GetProperties();
        Dictionary<string, object> GetColumnValues();
    }
}
