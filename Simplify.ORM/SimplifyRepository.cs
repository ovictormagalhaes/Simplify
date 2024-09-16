using Simplify.ORM.Interfaces;

namespace Simplify.ORM
{
    public class SimplifyRepository<T> : ISimplifyRepository<T> where T : SimplifyEntity
    {
        protected ISimplifyQueryBuilder QueryBuilder;
        protected ISimplifyCommandBuilder CommandBuilder;
        protected ISimplifyExecutor Executor;

        public SimplifyRepository(ISimplifyQueryBuilder queryBuilder, ISimplifyCommandBuilder commandBuilder, ISimplifyExecutor executor)
        {
            QueryBuilder = queryBuilder;
            CommandBuilder = commandBuilder;
            Executor = executor;
        }

        public async Task<T> GetByIdAsync<O>(T obj, string column, object value)
        {
            var table = obj.GetTableName();
            
            var query = QueryBuilder.SelectAllFieldsFrom(table).WhereEquals(table, column, value);

            return await Executor.FirstOrDefaultAsync<T>(query);
        }

        public async Task InsertAsync(T obj)
        {
            await Executor.ExecuteAsync(CommandBuilder.AddInsert(obj));
        }

        public async Task UpdateByIdAsync(T obj, string column, object value)
        {
            var table = obj.GetTableName();
            var columnValues = obj.GetColumnValues();

            await Executor.ExecuteAsync(CommandBuilder.AddUpdateWhereEquals(table, columnValues, column, value));
        }

        public string ColumnName<T>(string property) where T : SimplifyEntity
        {
            return QueryBuilder.ColumnName<T>(property);
        }
    }
}
