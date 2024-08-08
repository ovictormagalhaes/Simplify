using Dapper;
using Simplify.ORM.Interfaces;
using System.Data;

namespace Simplify.ORM
{
    public class SimplifyQuery : ISimplifyQuery
    {
        private readonly IDbConnection _connection;
        private readonly ISimplifyQueryBuilder _queryBuilder;

        public SimplifyQuery(IDbConnection connection, ISimplifyQueryBuilder queryBuilder)
        {
            _connection = connection;
            _queryBuilder = queryBuilder;
        }

        public IEnumerable<T> Query<T>(ISimplifyQueryBuilder query)
            => _connection.Query<T>(query.BuildQuery(), query.GetParameters());

        public async Task<IEnumerable<T>> QueryAsync<T>(ISimplifyQueryBuilder query)
            => await _connection.QueryAsync<T>(query.BuildQuery(), query.GetParameters());

        public Task<IEnumerable<T>> FirstOrDefault<T>(string table, string column) where T : ISimplifyEntity
        {
            _queryBuilder.BuildQuery();

            throw new NotImplementedException();
        }

    }
}
