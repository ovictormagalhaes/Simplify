using Dapper;
using Simplify.ORM.Interfaces;
using Simplify.ORM.Utils;

namespace Simplify.ORM
{
    public class SimplifyQuery(ISimplifyQueryBuilder queryBuilder) : ISimplifyQuery
    {
        public readonly ISimplifyQueryBuilder QueryBuilder = queryBuilder ?? throw new ArgumentNullException(nameof(queryBuilder));

        public string TableName<T>() where T : SimplifyEntity
            => SimplifyEntityHelper.TableName<T>();

        public string ColumnName<T>(string property) where T : SimplifyEntity 
            => SimplifyEntityHelper.ColumnName<T>(property);

        public ISimplifyQueryBuilder GetBuilder() => QueryBuilder;
    }
}
