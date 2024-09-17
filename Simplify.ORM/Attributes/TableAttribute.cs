using Simplify.ORM.Enumerations;

namespace Simplify.ORM.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class TableAttribute : Attribute
    {
        public string Name { get; set; }
        public NamingConvention ColumnsNamingConvention { get; set; } = NamingConvention.None;

        public TableAttribute()
        {

        }

        public TableAttribute(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Table name cannot be null or whitespace.", nameof(name));

            Name = name;
        }

        public TableAttribute(string name, NamingConvention columnsNamingConvention)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Table name cannot be null or whitespace.", nameof(name));

            Name = name;
            ColumnsNamingConvention = columnsNamingConvention;
        }

        public TableAttribute(NamingConvention columnsNamingConvention)
        {
            ColumnsNamingConvention = columnsNamingConvention;
        }
    }
}
