using Dapper;
using Simplify.ORM.Interfaces;
using System.Data;

namespace Simplify.ORM
{
    public class SimplifyQuery(IDbConnection connection) : ISimplifyQuery
    {
        private readonly IDbConnection _connection = connection;

        public IEnumerable<T> Query<T>(ISimplifyQueryBuilder query) where T : ISimplifyEntity
            => _connection.Query<T>(query.BuildQuery(), query.GetParameters());

        public async Task<IEnumerable<T>> QueryAsync<T>(ISimplifyQueryBuilder query) where T : ISimplifyEntity
            => await _connection.QueryAsync<T>(query.BuildQuery(), query.GetParameters());

        public T FirstOrDefault<T>(ISimplifyQueryBuilder query) where T : ISimplifyEntity
            => _connection.QueryFirstOrDefault<T>(query.BuildQuery(), query.GetParameters());
        
        public async Task<T> FirstOrDefaultAsync<T>(ISimplifyQueryBuilder query) where T : ISimplifyEntity
            => await _connection.QueryFirstOrDefaultAsync<T>(query.BuildQuery(), query.GetParameters());
    }
}
