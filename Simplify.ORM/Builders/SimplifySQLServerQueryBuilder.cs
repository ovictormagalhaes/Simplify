using Simplify.ORM.Interfaces;

namespace Simplify.ORM.Builders
{
    public class SimplifySQLServerQueryBuilder : SimplifyQueryBuilder
    {        
        public override string FormatTable(string table)
        {
            return $"[{table}]";
        }

        public override string FormatColumn(string column)
        {
            return $"[{column}]";
        }

        public override ISimplifyQueryBuilder AddLimit(int limit)
        {
            return this;
        }
    }
}
