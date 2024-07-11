using Dapper;
using System.Data;
using System.Data.Common;

namespace Simplify.ORM
{
    public interface SimplifyEntity
    {

    }

    public interface ISimplifyCommand
    {
        IEnumerable<T> Query<T>(ISimplifyQuery query);
        Task<IEnumerable<T>> QueryAsync<T>(ISimplifyQuery query);

        ISimplifyCommand Save(SimplifyEntity entity);
        Task Execute();
        Task ExecuteAsync();
    }

    public class SimplifyCommand : ISimplifyCommand
    {
        private IDbConnection _connection { get; }
        private List<SimplifyEntity> ToSave { get; } = new List<SimplifyEntity> ();

        public SimplifyCommand(IDbConnection connection)
        {
            _connection = connection;
        }

        public ISimplifyCommand Save(SimplifyEntity entity)
        {
            ToSave.Add(entity);
            return this;
        }

        public Task Execute()
        {
            using (var transaction = _connection.BeginTransaction())
            {
                ToSave.ForEach(entity => _connection.Execute("", entity, transaction));

                transaction.Commit();
            }

            return Task.CompletedTask;
        }

        public async Task ExecuteAsync()
        {
            using (var transaction = _connection.BeginTransaction())
            {
                ToSave.ForEach(async entity 
                    => await _connection.ExecuteAsync("", entity, transaction));

                transaction.Commit();
            }

        }

        public IEnumerable<T> Query<T>(ISimplifyQuery query)
            => _connection.Query<T>(query.BuildQuery(), query.GetParameters());

        public async Task<IEnumerable<T>> QueryAsync<T>(ISimplifyQuery query)
            => await _connection.QueryAsync<T>(query.BuildQuery(), query.GetParameters());
    }
}
