using Simplify.ORM.Interfaces;
using System.Text;

namespace Simplify.ORM.Builders
{
    public class SimplifyCommandBuilder : ISimplifyCommandBuilder
    {
        protected string? Table { get; set; }
        protected Dictionary<string, object> Inserts { get; set; } = new Dictionary<string, object>();
        protected Dictionary<string, object> Updates { get; set; } = new Dictionary<string, object>();
        protected List<WhereOperation> UpdatesWheres { get; set; } = new List<WhereOperation>();

        public string BuildInsertQuery(string table, Dictionary<string, object> columnValues)
        {
            var sb = new StringBuilder();
            if (Inserts.Any() && !string.IsNullOrEmpty(table))
            {
                sb.Append($"INSERT INTO {table} (");
                sb.Append(string.Join(", ", columnValues.Select(x => x.Key)));
                sb.Append(") VALUES(");
                sb.Append(string.Join(", ", columnValues.Select(x => $"@{x.Key}")));
                sb.Append(");");
                return sb.ToString();
            }

            return string.Empty;
        }

        public string BuildQuery()
        {
            var sb = new StringBuilder();
            if (Inserts.Any() && !string.IsNullOrEmpty(Table))
            {
                sb.Append($"INSERT INTO {Table} (");
                sb.Append(string.Join(", ", Inserts.Select(x => x.Key)));
                sb.Append(") VALUES(");
                sb.Append(string.Join(", ", Inserts.Select(x => $"@{x.Key}")));
                sb.Append(");");
                return sb.ToString();
            }

            if (Updates.Any() && !string.IsNullOrEmpty(Table))
            {
                sb.Append($"UPDATE {Table} SET ");
                sb.Append(string.Join(", ", Inserts.Select(x => $"SET {x.Key} = @{x.Key}")));
                return sb.ToString();

            }

            return string.Empty;
        }

        public Dictionary<string, object> GetParameters()
        {
            throw new NotImplementedException();
        }
    }
}
