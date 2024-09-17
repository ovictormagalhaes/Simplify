using Simplify.ORM.Interfaces;

namespace Simplify.ORM.Extensions
{
    public static class ClassExtensions
    {
        public static string TableName(this Type type)
        {
            if (typeof(ISimplifyEntity).IsAssignableFrom(type))
                return type.GetMethod("GetTableName")?.Invoke(null, null) as string;

            return null;
        }

        public static string ColumnName(this Type type, string property)
        {
            if (typeof(ISimplifyEntity).IsAssignableFrom(type))
                return type.GetMethod("GetColumnName")?.Invoke(null, [property]) as string;

            return null;
        }


    }
}
