namespace Simplify.ORM
{
    public interface ISimplifyEntity
    {
        string GetTableName();
        Dictionary<string, object> GetColumnValues();
    }

    public abstract partial class SimplifyEntity : ISimplifyEntity
    {
        public virtual string GetTableName() => throw new NotImplementedException();
        public virtual Dictionary<string, object> GetColumnValues() => throw new NotImplementedException();
    }
}
