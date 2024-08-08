using Simplify.ORM.Interfaces;

namespace Simplify.ORM.Builders
{
    public class SimplifySQLServerQueryBuilder : SimplifyQueryBuilder
    {
        override
        public string FormatTable(string table)
        {
            return $"[{table}]";
        }

        override
        public string FormatColumn(string column)
        {
            return $"[{column}]";
        }

        override
        public ISimplifyQueryBuilder AddLimit(int limit)
        {
            return this;
        }
    }
}
