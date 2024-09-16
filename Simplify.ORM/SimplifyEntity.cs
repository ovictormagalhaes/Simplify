using Simplify.ORM.Interfaces;

namespace Simplify.ORM
{
    public record SimplifyEntityProperty
    {
        public SimplifyEntityProperty(string? propertyName, string? columnName, object? value)
        {
            PropertyName = propertyName;
            ColumnName = columnName;
            Value = value;
        }

        public string? PropertyName { get; set; }
        public string? ColumnName { get; set; }
        public object? Value { get; set; }
    }

    public abstract class SimplifyEntity : ISimplifyEntity
    {
        public virtual string GetTableName() => GetType().Name;
        public string? GetColumnName(string property) => GetProperties().FirstOrDefault(x => x.PropertyName == property)?.PropertyName;
        public virtual IEnumerable<SimplifyEntityProperty> GetProperties() => [];
        public virtual Dictionary<string, object> GetColumnValues() 
        {
            var properties = GetProperties();

            if(properties is null)
                return [];

            return properties.ToDictionary(prop => prop.ColumnName, prop => prop.Value);
        }
    }
}
