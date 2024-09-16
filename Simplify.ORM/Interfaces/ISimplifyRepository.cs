using System.Data.Common;

namespace Simplify.ORM.Interfaces
{
    public interface ISimplifyRepository<T> where T : SimplifyEntity
    {
        public Task<T> GetByIdAsync<O>(T obj, string column, object value);
        public Task InsertAsync(T obj);
        public Task UpdateByIdAsync(T obj, string column, object value);
    }
}
