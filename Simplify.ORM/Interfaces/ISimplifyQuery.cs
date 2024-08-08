using System;
using System.Collections.Generic;
using System.Text;

namespace Simplify.ORM.Interfaces
{
    public interface ISimplifyQuery
    {
        IEnumerable<T> Query<T>(ISimplifyQueryBuilder query);
        Task<IEnumerable<T>> QueryAsync<T>(ISimplifyQueryBuilder query);
        Task<IEnumerable<T>> FirstOrDefault<T>(string table, string column) where T : ISimplifyEntity;
    }
}
