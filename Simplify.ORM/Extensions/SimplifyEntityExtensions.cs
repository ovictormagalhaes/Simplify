using Simplify.ORM.Interfaces;

namespace Simplify.ORM.Extensions
{
    public static class SimplifyEntityExtensions
    {
        public static T SetIsPersisted<T>(this T entity) where T : ISimplifyEntity
        {
            entity.IsPersisted = true;
            return entity;
        }

        public static IEnumerable<T> SetIsPersisted<T>(this IEnumerable<T> entities) where T : ISimplifyEntity
        {
            var list = entities.ToList();

            list.ForEach(e => SetIsPersisted(e));

            return list;
        }
    }
}
