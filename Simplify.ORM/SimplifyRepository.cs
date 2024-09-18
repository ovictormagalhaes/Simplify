using Simplify.ORM.Interfaces;
using Simplify.ORM.Utils;

namespace Simplify.ORM
{
    public class SimplifyRepository<T> : ISimplifyRepository<T> where T : SimplifyEntity
    {
        public ISimplifyQueryBuilder QueryBuilder { get; protected set; }
        public ISimplifyCommandBuilder CommandBuilder { get; protected set; }
        public ISimplifyExecutor Executor { get; protected set; }

        public SimplifyRepository(ISimplifyQueryBuilder queryBuilder, ISimplifyCommandBuilder commandBuilder, ISimplifyExecutor executor)
        {
            QueryBuilder = queryBuilder;
            CommandBuilder = commandBuilder;
            Executor = executor;
        }

        public ISimplifyQueryBuilder GetQueryBuilder() => QueryBuilder;
        public ISimplifyCommandBuilder GetCommandBuilder() => CommandBuilder;
        public ISimplifyExecutor GetExecutor() => Executor;


        public string TableName() => SimplifyEntityHelper.TableName<T>();
        public string TableName<O>() where O : SimplifyEntity => SimplifyEntityHelper.TableName<O>();

        public string ColumnName(string property) => SimplifyEntityHelper.ColumnName<T>(property);
        public string ColumnName<O>(string property) where O : SimplifyEntity => SimplifyEntityHelper.ColumnName<O>(property);


        public async Task<T> FirstOrDefaultByColumnEqualsAsync(string column, object value)
        {
            var entity = Activator.CreateInstance<T>();
            var table = entity.GetTableName();

            var query = QueryBuilder.SelectAllFieldsFrom(table).WhereEquals(table, column, value);

            return await Executor.FirstOrDefaultAsync<T>(query);
        }

        public async Task InsertAsync(T entity)
        {
            await Executor.ExecuteAsync(CommandBuilder.AddInsert(entity));
        }

        public async Task UpdateWhereColumnEqualsAsync(T entity, string column, object value)
        {
            var table = entity.GetTableName();
            var columnValues = entity.GetColumnValues();

            await Executor.ExecuteAsync(CommandBuilder.AddUpdateWhereEquals(table, columnValues, column, value));
        }

        public async Task<IEnumerable<T>> QueryByColumnEqualsAsync(string column, object value)
        {
            var entity = Activator.CreateInstance<T>();
            var table = entity.GetTableName();

            var query = QueryBuilder.SelectAllFieldsFrom(table).WhereEquals(table, column, value);

            return await Executor.QueryAsync<T>(query);
        }

        public async Task<IEnumerable<T>> QueryByColumnEqualsAsync(string column, List<object> value)
        {
            var entity = Activator.CreateInstance<T>();
            var table = entity.GetTableName();

            var query = QueryBuilder.SelectAllFieldsFrom(table).WhereIn(table, column, value);

            return await Executor.QueryAsync<T>(query);
        }
    }
}
