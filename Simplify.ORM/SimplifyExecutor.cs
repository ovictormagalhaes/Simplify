using Dapper;
using Simplify.ORM.Extensions;
using Simplify.ORM.Interfaces;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace Simplify.ORM
{
    public class SimplifyExecutor : ISimplifyExecutor
    {
        private readonly IDbConnection _connection;
        private readonly ISimplifyQueryBuilder _queryBuilder;

        public SimplifyExecutor(IDbConnection connection, ISimplifyQueryBuilder queryBuilder)
        {
            _connection = connection;
            _queryBuilder = queryBuilder;

            if (_connection.State != ConnectionState.Open)
                _connection.Open();
        }

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

        public IEnumerable<T> Query<T>(ISimplifyQueryBuilder queryBuilder) where T : ISimplifyEntity {

            var result = _connection.Query<T>(queryBuilder.BuildQuery(), queryBuilder.GetParameters()).SetIsPersisted();
            queryBuilder.Clean();
            return result; 
        }

        public IEnumerable<T> Query<T>(ISimplifyQuery query) where T : ISimplifyEntity
            => Query<T>(query.GetBuilder());

        public async Task<IEnumerable<T>> QueryAsync<T>(ISimplifyQueryBuilder queryBuilder) where T : ISimplifyEntity
        {
            var result = (await _connection.QueryAsync<T>(queryBuilder.BuildQuery(), queryBuilder.GetParameters())).SetIsPersisted();
            queryBuilder.Clean();
            return result;
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(ISimplifyQuery query) where T : ISimplifyEntity
            => await QueryAsync<T>(query.GetBuilder());
    
        public T FirstOrDefault<T>(ISimplifyQueryBuilder queryBuilder) where T : ISimplifyEntity
        {
            T result = _connection.QueryFirstOrDefault<T>(queryBuilder.BuildQuery(), queryBuilder.GetParameters()).SetIsPersisted();
            queryBuilder.Clean();
            return result;
        }

        public T FirstOrDefault<T>(ISimplifyQuery query) where T : ISimplifyEntity
            => FirstOrDefault<T>(query.GetBuilder());

        public async Task<T> FirstOrDefaultAsync<T>(ISimplifyQueryBuilder queryBuilder) where T : ISimplifyEntity
        {
            T result = await _connection.QueryFirstOrDefaultAsync<T>(queryBuilder.BuildQuery(), queryBuilder.GetParameters());
            queryBuilder.Clean();
            return result;
        }

        public async Task<T> FirstOrDefaultAsync<T>(ISimplifyQuery query) where T : ISimplifyEntity
            => await FirstOrDefaultAsync<T>(query.GetBuilder());

        public async Task HydrateAsync<T, U>(
        T entity,
        Expression<Func<T, object>> objectFKExpression,
        Expression<Func<T, object>> objectMemberToHydrateExpression,
        Expression<Func<U, object>> newObjectFKExpression) where T : ISimplifyEntity where U : ISimplifyEntity
        {
            var objectFK = GetPropertyInfo(objectFKExpression);
            var objectMember = GetPropertyInfo(objectMemberToHydrateExpression);
            var newObjectFK = GetPropertyInfo(newObjectFKExpression);

            var fkValueT = objectFK.GetValue(entity);

            U entityU = Activator.CreateInstance<U>();
            var tableNameU = entityU?.GetTableName();
            var columnNameFKU = entityU?.GetColumnName(newObjectFK.Name);

            var query = _queryBuilder
                .SelectAllFieldsFrom(tableNameU)
                .WhereEquals(tableNameU, columnNameFKU, fkValueT);

            var result = await QueryAsync<U>(query);

            SetObjectMemberValue(entity, objectMember, result);
        }

        public async Task HydrateAsync<T, U>(
            IEnumerable<T> entities,
            Expression<Func<T, object>> objectFKExpression,
            Expression<Func<T, object>> objectMemberToHydrateExpression,
            Expression<Func<U, object>> newObjectFKExpression)
                where T : ISimplifyEntity
                where U : ISimplifyEntity
        {
            var objectFK = GetPropertyInfo(objectFKExpression);
            var objectMember = GetPropertyInfo(objectMemberToHydrateExpression);
            var newObjectFK = GetPropertyInfo(newObjectFKExpression);

            var fkValuesT = new List<object>();

            foreach (var entity in entities)
            {
                var fkValueT = objectFK.GetValue(entity);
                fkValuesT.Add(fkValueT);
            }

            U entityU = Activator.CreateInstance<U>();
            var tableNameU = entityU.GetTableName();
            var columnNameFKU = entityU.GetColumnName(newObjectFK.Name)!;

            var query = _queryBuilder
                .SelectAllFieldsFrom(tableNameU)
                .WhereIn(tableNameU, columnNameFKU, fkValuesT);

            var build = query.BuildQuery();
            var result = await QueryAsync<U>(query);

            SetObjectMemberValue(entities, objectMember, result, objectFK, newObjectFK);
        }

        public static PropertyInfo GetPropertyInfo(LambdaExpression expression)
        {
            if (expression.Body is MemberExpression memberExpression)
            {
                return (PropertyInfo)memberExpression.Member;
            }
            else if (expression.Body is UnaryExpression unaryExpression &&
                     unaryExpression.Operand is MemberExpression memberOperand)
            {
                return (PropertyInfo)memberOperand.Member;
            }

            throw new ArgumentException("Invalid expression", nameof(expression));
        }

        public static void SetObjectMemberValue<T, U>(T entity, PropertyInfo objectMember, IEnumerable<U> result)
            where T : ISimplifyEntity
            where U : ISimplifyEntity
        {
            if (objectMember.PropertyType == typeof(ICollection<U>))
            {
                var collection = (ICollection<U>)objectMember.GetValue(entity);
                if (collection == null)
                {
                    collection = new List<U>();
                    objectMember.SetValue(entity, collection);
                }

                foreach (var item in result)
                    collection.Add(item);
            }
            else if (objectMember.PropertyType == typeof(List<U>))
            {
                var list = (List<U>)objectMember.GetValue(entity);

                if (list == null)
                {
                    list = new List<U>();
                    objectMember.SetValue(entity, list);
                }

                foreach (var item in result)
                    list.Add(item);
            }
            else if (objectMember.PropertyType == typeof(U))
            {
                objectMember.SetValue(entity, result.FirstOrDefault());
            }
            else if (objectMember.PropertyType.IsGenericType &&
                     objectMember.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                     Nullable.GetUnderlyingType(objectMember.PropertyType) == typeof(U))
            {
                objectMember.SetValue(entity, result.FirstOrDefault());
            }
            else
                throw new InvalidOperationException($"Property type '{objectMember.PropertyType}' not supported.");
        }

        public static void SetObjectMemberValue<T, U>(
            IEnumerable<T> entities,
            PropertyInfo objectMember,
            IEnumerable<U> result,
            PropertyInfo objectFK,
            PropertyInfo newObjectFK)
                where T : ISimplifyEntity
                where U : ISimplifyEntity
        {
            foreach (var entity in entities)
            {
                var fkValueT = objectFK.GetValue(entity);

                var relatedItems = result.Where(item => newObjectFK.GetValue(item)?.Equals(fkValueT) == true).ToList();


                if (objectMember.PropertyType == typeof(ICollection<U>))
                {
                    var collection = (ICollection<U>)objectMember.GetValue(entity);
                    if (collection == null)
                    {
                        collection = new List<U>();
                        objectMember.SetValue(entity, collection);
                    }
                }
                else if (objectMember.PropertyType == typeof(List<U>))
                {
                    var list = (List<U>)objectMember.GetValue(entity);

                    if (list == null)
                    {
                        list = new List<U>();
                        objectMember.SetValue(entity, list);
                    }

                    foreach (var item in result)
                        list.Add(item);
                }
                else if (objectMember.PropertyType == typeof(U))
                {
                    objectMember.SetValue(entity, relatedItems.FirstOrDefault());
                }
                else if (objectMember.PropertyType.IsGenericType &&
                         objectMember.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                         Nullable.GetUnderlyingType(objectMember.PropertyType) == typeof(U))
                {
                    objectMember.SetValue(entity, relatedItems.FirstOrDefault());
                }
                else
                    throw new InvalidOperationException($"Property type '{objectMember.PropertyType}' not supported.");
            }
        }

    }
}
