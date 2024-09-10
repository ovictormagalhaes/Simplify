using Simplify.ORM.Interfaces;

namespace Simplify.ORM.Builders
{
    public sealed class SimplifyMySQLQueryBuilder : AbstractSimplifyQueryBuilder
    {
        public override string FormatTable(string table)
        {
            return $"`{table}`";
        }

        public override string FormatColumn(string column)
        {
            return $"`{column}`";
        }

        public override ISimplifyQueryBuilder AddTop(int top)
        {
            return this;
        }
    }
}
