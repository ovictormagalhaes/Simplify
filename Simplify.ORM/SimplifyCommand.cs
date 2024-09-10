using Dapper;
using Simplify.ORM.Interfaces;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace Simplify.ORM
{
    public class SimplifyCommand(IDbConnection connection, ISimplifyQueryBuilder queryBuilder) : ISimplifyCommand
    {
        private readonly IDbConnection _connection = connection;
        private readonly ISimplifyQueryBuilder _queryBuilder = queryBuilder;

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

        public IEnumerable<T> Query<T>(ISimplifyQueryBuilder queryBuilder) where T : ISimplifyEntity
            => _connection.Query<T>(queryBuilder.BuildQuery(), queryBuilder.GetParameters());

        public IEnumerable<T> Query<T>(ISimplifyQuery query) where T : ISimplifyEntity
            => Query<T>(query.GetBuilder());

        public async Task<IEnumerable<T>> QueryAsync<T>(ISimplifyQueryBuilder queryBuilder) where T : ISimplifyEntity
            => await _connection.QueryAsync<T>(queryBuilder.BuildQuery(), queryBuilder.GetParameters());

        public async Task<IEnumerable<T>> QueryAsync<T>(ISimplifyQuery query) where T : ISimplifyEntity
            => await QueryAsync<T>(query.GetBuilder());
    
        public T FirstOrDefault<T>(ISimplifyQueryBuilder queryBuilder) where T : ISimplifyEntity
            => _connection.QueryFirstOrDefault<T>(queryBuilder.BuildQuery(), queryBuilder.GetParameters());

        public T FirstOrDefault<T>(ISimplifyQuery query) where T : ISimplifyEntity
            => FirstOrDefault<T>(query.GetBuilder());

        public async Task<T> FirstOrDefaultAsync<T>(ISimplifyQueryBuilder queryBuilder) where T : ISimplifyEntity
            => await _connection.QueryFirstOrDefaultAsync<T>(queryBuilder.BuildQuery(), queryBuilder.GetParameters());

        public async Task<T> FirstOrDefaultAsync<T>(ISimplifyQuery query) where T : ISimplifyEntity
            => await FirstOrDefaultAsync<T>(query.GetBuilder());

        public async Task HydrateAsync<T, U>(
        T entity,
        Expression<Func<T, object?>> objectFKExpression,
        Expression<Func<T, object?>> objectMemberToHydrateExpression,
        Expression<Func<U, object?>> newObjectFKExpression)
    where T : ISimplifyEntity
    where U : ISimplifyEntity
        {
            var objectFK = GetPropertyInfo(objectFKExpression);
            var objectMember = GetPropertyInfo(objectMemberToHydrateExpression);
            var newObjectFK = GetPropertyInfo(newObjectFKExpression);

            var fkValueT = objectFK.GetValue(entity);

            var tableNameU = default(U)!.GetTableName();
            var columnNameFKU = default(U)!.GetColumnName(newObjectFK.Name)!;

            var query = _queryBuilder
                .SelectAllFields(tableNameU)
                .From(tableNameU)
                .WhereEquals(tableNameU, columnNameFKU, fkValueT);

            var result = await QueryAsync<U>(query);

            SetObjectMemberValue(entity, objectMember, result);
        }

        public async Task HydrateAsync<T, U>(
            IEnumerable<T> entities,
            Expression<Func<T, object?>> objectFKExpression,
            Expression<Func<T, object?>> objectMemberToHydrateExpression,
            Expression<Func<U, object?>> newObjectFKExpression)
                where T : ISimplifyEntity
                where U : ISimplifyEntity
        {
            var objectFK = GetPropertyInfo(objectFKExpression);
            var objectMember = GetPropertyInfo(objectMemberToHydrateExpression);
            var newObjectFK = GetPropertyInfo(newObjectFKExpression);

            var fkValueT = objectFK.GetValue(entities);

            var tableNameU = default(U)!.GetTableName();
            var columnNameFKU = default(U)!.GetColumnName(newObjectFK.Name)!;

            var query = _queryBuilder
                .SelectAllFields(tableNameU)
                .From(tableNameU)
                .WhereEquals(tableNameU, columnNameFKU, fkValueT);
            var result = await QueryAsync<U>(query);

            SetObjectMemberValue(entities, objectMember, result, objectFK, newObjectFK);
        }

        private PropertyInfo GetPropertyInfo(LambdaExpression expression)
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

        private void SetObjectMemberValue<T, U>(T entity, PropertyInfo objectMember, IEnumerable<U> result)
            where T : ISimplifyEntity
            where U : ISimplifyEntity
        {
            if (objectMember.PropertyType == typeof(ICollection<U>))
            {
                var collection = (ICollection<U>)objectMember.GetValue(entity);
                foreach (var item in result)
                    collection.Add(item);
            }
            else if (objectMember.PropertyType == typeof(List<U>))
            {
                var list = (List<U>)objectMember.GetValue(entity);
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

        private void SetObjectMemberValue<T, U>(
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
                    foreach (var item in relatedItems)
                        collection.Add(item);
                }
                else if (objectMember.PropertyType == typeof(List<U>))
                {
                    var list = (List<U>)objectMember.GetValue(entity);
                    foreach (var item in relatedItems)
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
