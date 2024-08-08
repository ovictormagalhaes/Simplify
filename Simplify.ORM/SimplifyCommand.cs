using Dapper;
using Simplify.ORM.Interfaces;
using System.Data;

namespace Simplify.ORM
{
    public interface ISimplifyCommand
    {
        ISimplifyCommand Save(ISimplifyEntity entity);
        Task Execute();
        Task ExecuteAsync();
    }

    public class SimplifyCommand : ISimplifyCommand
    {
        private readonly IDbConnection _connection;
        private readonly ISimplifyQueryBuilder _queryBuilder;
        private readonly ISimplifyCommandBuilder _commandBuilder;
        private List<ISimplifyEntity> ToSave { get; } = new List<ISimplifyEntity>();

        public SimplifyCommand(IDbConnection connection, ISimplifyQueryBuilder queryBuilder, ISimplifyCommandBuilder commandBuilder)
        {
            _connection = connection;
            _queryBuilder = queryBuilder;
            _commandBuilder = commandBuilder;
        }

        public ISimplifyCommand Save(ISimplifyEntity entity)
        {
            ToSave.Add(entity);
            return this;
        }

        public Task Execute()
        {
            using (var transaction = _connection.BeginTransaction())
            {
                var query = _queryBuilder.BuildQuery();
                ToSave.ForEach(entity => _connection.Execute("", entity, transaction));

                transaction.Commit();
            }

            return Task.CompletedTask;
        }

        public Task ExecuteAsync()
        {
            using (var transaction = _connection.BeginTransaction())
            {
                ToSave.ForEach(async entity
                    => await _connection.ExecuteAsync("", entity, transaction));

                transaction.Commit();
            }
            return Task.CompletedTask;
        }

        public IEnumerable<T> Query<T>(ISimplifyQueryBuilder query)
            => _connection.Query<T>(query.BuildQuery(), query.GetParameters());

        public async Task<IEnumerable<T>> QueryAsync<T>(ISimplifyQueryBuilder query)
            => await _connection.QueryAsync<T>(query.BuildQuery(), query.GetParameters());
    }
}
