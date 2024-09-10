namespace Simplify.ORM.Utils
{
    public static class SimplifyEntityHelper
    {
        public static string TableName<T>() where T : SimplifyEntity
        {
            var entity = default(T);
            return entity?.GetTableName() ?? throw new InvalidOperationException("Entity cannot be null.");
        }

        public static string ColumnName<T>(string property) where T : SimplifyEntity
        {
            var entity = default(T);
            return entity?.GetColumnName(property) ?? throw new InvalidOperationException("Entity or property cannot be null.");
        }

    }
}
