using Simplify.ORM.Interfaces;

namespace Simplify.ORM.Builders
{
    public class SimplifyMySQLQueryBuilder : SimplifyQueryBuilder
    {
        override
        public string FormatTable(string table)
        {
            return $"`{table}`";
        }

        override
        public string FormatColumn(string column)
        {
            return $"`{column}`";
        }

        override
        public ISimplifyQueryBuilder AddTop(int top)
        {
            return this;
        }
    }
}
