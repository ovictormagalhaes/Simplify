using System.Linq.Expressions;

namespace Simplify.ORM.Interfaces
{
    public interface ISimplifyCommand
    {
        Task Execute(ISimplifyCommandBuilder command);
        Task Execute(IEnumerable<ISimplifyCommandBuilder> commands);

        Task ExecuteAsync(ISimplifyCommandBuilder command);
        Task ExecuteAsync(IEnumerable<ISimplifyCommandBuilder> commands);

        IEnumerable<T> Query<T>(ISimplifyQueryBuilder queryBuilder) where T : ISimplifyEntity;
        IEnumerable<T> Query<T>(ISimplifyQuery query) where T : ISimplifyEntity;

        Task<IEnumerable<T>> QueryAsync<T>(ISimplifyQueryBuilder queryBuilder) where T : ISimplifyEntity;
        Task<IEnumerable<T>> QueryAsync<T>(ISimplifyQuery query) where T : ISimplifyEntity;

        T FirstOrDefault<T>(ISimplifyQueryBuilder queryBuilder) where T : ISimplifyEntity;
        T FirstOrDefault<T>(ISimplifyQuery query) where T : ISimplifyEntity;
        Task<T> FirstOrDefaultAsync<T>(ISimplifyQueryBuilder queryBuilder) where T : ISimplifyEntity;
        Task<T> FirstOrDefaultAsync<T>(ISimplifyQuery query) where T : ISimplifyEntity;

        Task HydrateAsync<T, U>(T entity, Expression<Func<T, object?>> objectFKExpression, Expression<Func<T, object?>> objectMemberToHydrateExpression, Expression<Func<U, object?>> newObjectFKExpression)
            where T : ISimplifyEntity
            where U : ISimplifyEntity;

        Task HydrateAsync<T, U>(IEnumerable<T> entity, Expression<Func<T, object?>> objectFKExpression, Expression<Func<T, object?>> objectMemberToHydrateExpression, Expression<Func<U, object?>> newObjectFKExpression)
            where T : ISimplifyEntity
            where U : ISimplifyEntity;
    }
}
