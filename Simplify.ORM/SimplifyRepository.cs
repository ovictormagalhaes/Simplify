﻿using Simplify.ORM.Interfaces;
using Simplify.ORM.Utils;

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

        public string ColumnName<O>(string property) where O : SimplifyEntity => SimplifyEntityHelper.ColumnName<O>(property);

        public string TableName<O>() where O : SimplifyEntity => SimplifyEntityHelper.TableName<O>();

        public async Task<T> BaseSelectByIdAsync<O>(T obj, string column, object value)
        {
            var table = obj.GetTableName();
            
            var query = QueryBuilder.SelectAllFieldsFrom(table).WhereEquals(table, column, value);

            return await Executor.FirstOrDefaultAsync<T>(query);
        }

        public async Task BaseInsertAsync(T obj)
        {
            await Executor.ExecuteAsync(CommandBuilder.AddInsert(obj));
        }

        public async Task BaseUpdateWhereColumnEqualsAsync(T obj, string column, object value)
        {
            var table = obj.GetTableName();
            var columnValues = obj.GetColumnValues();

            await Executor.ExecuteAsync(CommandBuilder.AddUpdateWhereEquals(table, columnValues, column, value));
        }
    }
}
