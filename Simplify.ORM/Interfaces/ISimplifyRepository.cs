using Simplify.ORM.Utils;
using System.Data.Common;

namespace Simplify.ORM.Interfaces
{
    public interface ISimplifyRepository<T> where T : SimplifyEntity
    {
        public Task<T> FirstOrDefaultByColumnEqualsAsync(string column, object value);
        public Task<IEnumerable<T>> QueryByColumnEqualsAsync(string column, object value);
        public Task<IEnumerable<T>> QueryByColumnEqualsAsync(string column, List<object> value);
        public Task InsertAsync(T entity);
        public Task UpdateWhereColumnEqualsAsync(T entity, string column, object value);
    }
}
