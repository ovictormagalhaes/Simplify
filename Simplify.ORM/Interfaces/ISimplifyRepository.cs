using System.Data.Common;

namespace Simplify.ORM.Interfaces
{
    public interface ISimplifyRepository<T> where T : SimplifyEntity
    {
        public Task<T> BaseSelectByIdAsync<O>(T obj, string column, object value);
        public Task BaseInsertAsync(T obj);
        public Task BaseUpdateWhereColumnEqualsAsync(T obj, string column, object value);
    }
}
