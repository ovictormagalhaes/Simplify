using Simplify.ORM.Enumerations;
using Simplify.ORM.Interfaces;
using System.Text;

namespace Simplify.ORM.Builders
{
    public class SimplifyCommandBuilder : ISimplifyCommandBuilder
    {
        protected Dictionary<string, object> Parameters { get; set; } = [];

        protected string Table { get; set; } = string.Empty;
        protected Dictionary<string, object> InsertValues { get; set; } = [];
        protected Dictionary<string, object> UpdateValues { get; set; } = [];

        protected List<WhereOperation> UpdateWheres = [];

        public Dictionary<string, object> GetParameters() => Parameters;

        public virtual string BuildQuery()
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(Table) && InsertValues.Any())
            {
                sb.Append($"INSERT INTO {Table} (");
                sb.Append(string.Join(", ", InsertValues.Select(x => x.Key)));
                sb.Append(") VALUES (");
                sb.Append(string.Join(", ", InsertValues.Select(x => $"@{x.Key}")));
                sb.Append(")");
                return sb.Append(";").ToString().Replace("  ", " ").TrimEnd();
            }

            if (!string.IsNullOrEmpty(Table) && UpdateValues.Any())
            {
                sb.Append($"UPDATE {Table} SET ");
                sb.Append(string.Join(", ", UpdateValues.Select(x => $"{x.Key} = @{x.Key}")));
                sb.Append(" ");

                if (UpdateWheres.Any())
                    sb.Append($"{GetWhereOperationSymbol(SimplifyWhereOperation.Where)} ");

                foreach (var where in UpdateWheres)
                {
                    var operationSymbol = GetWhereOperationSymbol(where.Operation);
                    var parameterName = !string.IsNullOrEmpty(where.ParameterName) ? (where.ParameterName!) : null;

                    if (!string.IsNullOrEmpty(where.LeftTable) && !string.IsNullOrEmpty(where.LeftColumn))
                    {
                        var whereTable = where.LeftTable!;
                        var column = where.LeftColumn!;
                        sb.Append($"{Table}.{column} {operationSymbol} {parameterName} ");
                    }
                    else
                        sb.Append($"{operationSymbol} {parameterName} ");
                }
                return sb.Append(";").ToString().Replace("  ", " ").TrimEnd();
            }
            
            return string.Empty;
        }

        public ISimplifyCommandBuilder AddInsert(ISimplifyEntity entity) 
            => AddInsert(entity.GetTableName(), entity.GetColumnValues());

        public ISimplifyCommandBuilder AddInsert(string table, Dictionary<string, object> columnValues)
        {
            Table = table;
            Parameters = columnValues;
            InsertValues = columnValues;
            return this;
        }

        public ISimplifyCommandBuilder AddUpdate(ISimplifyEntity entity, List<WhereOperation> whereOperations)
            => AddUpdate(entity.GetTableName(), entity.GetColumnValues(), whereOperations);

        public ISimplifyCommandBuilder AddUpdate(string table, Dictionary<string, object> columnValues, List<WhereOperation> whereOperations)
        {
            Table = table;
            Parameters = columnValues;
            UpdateValues = columnValues;
            UpdateWheres = whereOperations;
            return this;
        }

        public virtual string GetWhereOperationSymbol(SimplifyWhereOperation operation) => operation switch
        {
            SimplifyWhereOperation.Where => "WHERE",
            SimplifyWhereOperation.Or => "OR",
            SimplifyWhereOperation.And => "AND",
            SimplifyWhereOperation.Equals => "=",
            SimplifyWhereOperation.NotEquals => "<>",
            SimplifyWhereOperation.Greater => ">",
            SimplifyWhereOperation.GreaterOrEqual => ">=",
            SimplifyWhereOperation.Lower => "<",
            SimplifyWhereOperation.LowerOrEqual => "<=",
            SimplifyWhereOperation.Between => "BETWEEN",
            _ => throw new ArgumentException("Invalid operation for this method", nameof(operation))
        };
    }
}
