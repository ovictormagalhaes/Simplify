namespace Simplify.ORM.Interfaces
{
    public interface ISimplifyQuery
    {
        IEnumerable<T> Query<T>(ISimplifyQueryBuilder query) where T : ISimplifyEntity;
        Task<IEnumerable<T>> QueryAsync<T>(ISimplifyQueryBuilder query) where T : ISimplifyEntity;
        T FirstOrDefault<T>(ISimplifyQueryBuilder query) where T : ISimplifyEntity;
        Task<T> FirstOrDefaultAsync<T>(ISimplifyQueryBuilder query) where T : ISimplifyEntity;
    }
}
