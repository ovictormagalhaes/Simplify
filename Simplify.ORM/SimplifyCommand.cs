using Dapper;
using Simplify.ORM.Interfaces;
using System.Data;

namespace Simplify.ORM
{
    public class SimplifyCommand(IDbConnection connection) : ISimplifyCommand
    {
        private readonly IDbConnection _connection = connection;

        public Task Execute(ISimplifyCommandBuilder command)
        {
            using (var transaction = _connection.BeginTransaction())
            {
                _connection.Execute(command.BuildQuery(), command.GetParameters());

                transaction.Commit();
            }

            return Task.CompletedTask;
        }

        public Task Execute(IEnumerable<ISimplifyCommandBuilder> commands)
        {
            using (var transaction = _connection.BeginTransaction())
            {
                foreach (var command in commands)
                    _connection.Execute(command.BuildQuery(), command.GetParameters());

                transaction.Commit();
            }

            return Task.CompletedTask;
        }

        public async Task ExecuteAsync(ISimplifyCommandBuilder command)
        {
            using var transaction = _connection.BeginTransaction();
                await _connection.ExecuteAsync(command.BuildQuery(), command.GetParameters());

            transaction.Commit();
        }

        public async Task ExecuteAsync(IEnumerable<ISimplifyCommandBuilder> commands)
        {
            using var transaction = _connection.BeginTransaction();
            foreach (var command in commands)
                await _connection.ExecuteAsync(command.BuildQuery(), command.GetParameters());

            transaction.Commit();
        }

        public IEnumerable<T> Query<T>(ISimplifyQueryBuilder query)
            => _connection.Query<T>(query.BuildQuery(), query.GetParameters());

        public async Task<IEnumerable<T>> QueryAsync<T>(ISimplifyQueryBuilder query)
            => await _connection.QueryAsync<T>(query.BuildQuery(), query.GetParameters());
    }
}
