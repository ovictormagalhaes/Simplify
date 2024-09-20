using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Simplify.ORM.Builders
{
    // Interface para o construtor de resposta
    public interface ISimplifyResponseBuilder
    {
        dynamic Build(object entity);
    }

    // Implementação da classe genérica SimplifyResponseBuilder
    public class SimplifyResponseBuilder<T> : ISimplifyResponseBuilder
    {
        private readonly List<(Func<T, object> Selector, string FieldName)> _fields = new List<(Func<T, object>, string)>();
        private readonly List<(Func<T, IEnumerable<object>> Selector, string FieldName, Func<object, IDictionary<string, object>> ItemMapper)> _listFields = new List<(Func<T, IEnumerable<object>>, string, Func<object, IDictionary<string, object>>)>();
        private readonly List<(Func<T, object> Selector, string FieldName, Type EntityType, bool IncludeAllProperties, ISimplifyResponseBuilder ItemBuilder)> _entityFields = new List<(Func<T, object>, string, Type, bool, ISimplifyResponseBuilder)>();

        public SimplifyResponseBuilder<T> Field(Expression<Func<T, object>> selector)
        {
            var memberExpression = GetMemberExpression(selector);
            if (memberExpression == null)
                throw new ArgumentException("Invalid expression");

            var fieldName = memberExpression.Member.Name;
            _fields.Add((selector.Compile(), fieldName));
            return this;
        }

        public SimplifyResponseBuilder<T> List<TItem>(Expression<Func<T, IEnumerable<TItem>>> selector, Action<SimplifyResponseBuilder<TItem>> itemConfig = null)
        {
            var memberExpression = GetMemberExpression(selector);
            if (memberExpression == null)
                throw new ArgumentException("Invalid expression");

            var fieldName = memberExpression.Member.Name;

            Func<T, IEnumerable<object>> compiledSelector = entity =>
            {
                var items = selector.Compile()(entity);
                return items?.Cast<object>() ?? Enumerable.Empty<object>();
            };

            Func<object, IDictionary<string, object>> itemMapper = item =>
            {
                if (itemConfig == null)
                {
                    // Map all properties of the item by default
                    var localItemBuilder = new SimplifyResponseBuilder<TItem>();
                    foreach (var prop in typeof(TItem).GetProperties())
                    {
                        var propertyExpression = GetPropertyExpression<TItem>(prop);
                        localItemBuilder.Field(propertyExpression);
                    }
                    var itemResponse = localItemBuilder.Build((TItem)item);
                    return (IDictionary<string, object>)itemResponse;
                }

                var itemBuilder = new SimplifyResponseBuilder<TItem>();
                itemConfig(itemBuilder);

                var itemResponseDict = itemBuilder.Build((TItem)item);
                return (IDictionary<string, object>)itemResponseDict;
            };

            _listFields.Add((compiledSelector, fieldName, itemMapper));
            return this;
        }

        public SimplifyResponseBuilder<T> Entity<TProperty>(Expression<Func<T, TProperty>> selector, bool includeAllProperties = true)
        {
            var memberExpression = GetMemberExpression(selector);
            if (memberExpression == null)
                throw new ArgumentException("Invalid expression");

            var fieldName = memberExpression.Member.Name;
            Func<T, object> compiledSelector = entity => selector.Compile()(entity);

            ISimplifyResponseBuilder itemBuilder = includeAllProperties ? null : new SimplifyResponseBuilder<TProperty>();

            _entityFields.Add((compiledSelector, fieldName, typeof(TProperty), includeAllProperties, itemBuilder));
            return this;
        }

        public SimplifyResponseBuilder<T> Entity<TProperty>(Expression<Func<T, TProperty>> selector, Action<SimplifyResponseBuilder<TProperty>> itemConfig)
        {
            var memberExpression = GetMemberExpression(selector);
            if (memberExpression == null)
                throw new ArgumentException("Invalid expression");

            var fieldName = memberExpression.Member.Name;
            Func<T, object> compiledSelector = entity => selector.Compile()(entity);

            var itemBuilder = new SimplifyResponseBuilder<TProperty>();
            itemConfig(itemBuilder);

            _entityFields.Add((compiledSelector, fieldName, typeof(TProperty), false, itemBuilder));
            return this;
        }

        public dynamic Build(object entity)
        {
            if (entity is not T typedEntity)
                throw new ArgumentException($"Expected entity of type {typeof(T).Name}, but got {entity.GetType().Name}.");

            dynamic response = new ExpandoObject();
            var responseDict = (IDictionary<string, object>)response;

            foreach (var (selector, fieldName) in _fields)
                responseDict.Add(fieldName, selector(typedEntity));

            foreach (var (selector, fieldName, itemMapper) in _listFields)
            {
                var list = selector(typedEntity);
                if (list != null)
                {
                    var mappedList = list.Select(item => itemMapper(item)).ToList();
                    responseDict.Add(fieldName, mappedList);
                }
            }

            foreach (var (selector, fieldName, entityType, includeAllProperties, itemBuilder) in _entityFields)
            {
                var entityValue = selector(typedEntity);
                if (entityValue != null)
                {
                    var entityResponse = new ExpandoObject();
                    var entityDict = (IDictionary<string, object>)entityResponse;

                    if (includeAllProperties)
                    {
                        foreach (var prop in entityType.GetProperties())
                        {
                            var propValue = prop.GetValue(entityValue);
                            entityDict[prop.Name] = propValue;
                        }
                    }
                    else
                    {
                        if (itemBuilder != null)
                        {
                            var itemResponseDict = itemBuilder.Build(entityValue);
                            foreach (var kvp in (IDictionary<string, object>)itemResponseDict)
                            {
                                entityDict[kvp.Key] = kvp.Value;
                            }
                        }
                    }

                    responseDict.Add(fieldName, entityResponse);
                }
            }

            return response;
        }

        private MemberExpression GetMemberExpression(LambdaExpression lambda)
        {
            if (lambda.Body is UnaryExpression unaryExpression)
            {
                return unaryExpression.Operand as MemberExpression;
            }
            return lambda.Body as MemberExpression;
        }

        private Expression<Func<TItem, object>> GetPropertyExpression<TItem>(PropertyInfo property)
        {
            var param = Expression.Parameter(typeof(TItem), "x");
            var propertyAccess = Expression.MakeMemberAccess(param, property);
            var lambda = Expression.Lambda<Func<TItem, object>>(
                Expression.Convert(propertyAccess, typeof(object)), param);
            return lambda;
        }
    }
}
