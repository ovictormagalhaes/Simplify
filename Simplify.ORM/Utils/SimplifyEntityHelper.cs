using Simplify.ORM.Interfaces;

namespace Simplify.ORM.Utils
{
    public static class SimplifyEntityHelper
    {
        public static string TableName(this SimplifyEntity entity)
        {
            return entity?.GetTableName() ?? throw new InvalidOperationException("Entity cannot be null.");
        }

        public static string TableName<T>() where T : SimplifyEntity
        {
            var entity = Activator.CreateInstance<T>();

            return entity?.GetTableName() ?? throw new InvalidOperationException("Entity cannot be null.");
        }

        public static string TableName(this Type type)
        {
            if (typeof(ISimplifyEntity).IsAssignableFrom(type))
            {
                var instance = Activator.CreateInstance(type);
                return type.GetMethod("GetTableName")?.Invoke(instance, null) as string;
            }

            return null;
        }

        public static string ColumnName(this SimplifyEntity entity, string property)
        {
            return entity?.GetColumnName(property) ?? throw new InvalidOperationException("Entity or property cannot be null.");
        }

        public static string ColumnName<T>(string property) where T : SimplifyEntity
        {
            var entity = Activator.CreateInstance<T>();

            return entity?.GetColumnName(property) ?? throw new InvalidOperationException("Entity or property cannot be null.");
        }

        public static string ColumnName(this Type type, string property)
        {
            if (typeof(ISimplifyEntity).IsAssignableFrom(type))
            {
                var instance = Activator.CreateInstance(type);

                var method = type.GetMethod("GetColumnName");

                return method?.Invoke(instance, [property]) as string;
            }

            return null;
        }
    }
}
