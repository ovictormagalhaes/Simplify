using Simplify.ORM.Enumerations;
using Simplify.ORM.Interfaces;
using System.Text;

namespace Simplify.ORM.Builders
{
    public class SimplifyCommandBuilder : ISimplifyCommandBuilder
    {
        public string BuildInsertQuery(string table, Dictionary<string, object> columnValues)
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(table) && columnValues.Any())
            {
                sb.Append($"INSERT INTO {table} (");
                sb.Append(string.Join(", ", columnValues.Select(x => x.Key)));
                sb.Append(") VALUES (");
                sb.Append(string.Join(", ", columnValues.Select(x => $"@{x.Key}")));
                sb.Append(");");
                return sb.ToString();
            }

            return string.Empty;
        }

        public string BuildUpdateQuery(string table, Dictionary<string, object> columnValues, List<WhereOperation> whereOperations)
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(table) && columnValues.Any())
            {
                sb.Append($"UPDATE {table} SET ");
                sb.Append(string.Join(", ", columnValues.Select(x => $"{x.Key} = @{x.Key}")));
                sb.Append(" ");

                if (whereOperations.Any())
                    sb.Append($"{GetWhereOperationSymbol(SimplifyWhereOperation.Where)} ");

                foreach (var where in whereOperations)
                {
                    var operationSymbol = GetWhereOperationSymbol(where.Operation);
                    var parameterName = !string.IsNullOrEmpty(where.ParameterName) ? (where.ParameterName!) : null;

                    if (!string.IsNullOrEmpty(where.LeftTable) && !string.IsNullOrEmpty(where.LeftColumn))
                    {
                        var whereTable = where.LeftTable!;
                        var column = where.LeftColumn!;
                        sb.Append($"{table}.{column} {operationSymbol} {parameterName} ");
                    }
                    else
                        sb.Append($"{operationSymbol} {parameterName} ");
                }

                return sb.Append(";").ToString().Replace("  ", " ").TrimEnd();
            }

            return string.Empty;
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
