using Simplify.ORM.Interfaces;

namespace Simplify.ORM
{
    public abstract partial class SimplifyEntity : ISimplifyEntity
    {
        public virtual string GetTableName() => GetType().Name;
        public virtual Dictionary<string, object> GetColumnValues() => [];
        public virtual IEnumerable<string> GetColumns() => GetColumnValues().Select(x => x.Key);
        public static string TableName => nameof(SimplifyEntity);
    }
}
