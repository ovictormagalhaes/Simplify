namespace Simplify.ORM.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ColumnAttribute : Attribute
    {
        public string Name { get; }
        public bool Ignore = false;
        public ColumnAttribute(string name, bool ignore = false)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Table name cannot be null or whitespace.", nameof(name));

            Name = name;
            Ignore = ignore;
        }
    }
}
