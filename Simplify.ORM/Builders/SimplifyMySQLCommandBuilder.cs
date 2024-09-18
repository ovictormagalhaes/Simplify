namespace Simplify.ORM.Builders
{
    public sealed class SimplifyMySQLCommandBuilder : AbstractSimplifyCommandBuilder
    {
        public override string FormatTable(string table)
        {
            return $"`{table}`";
        }

        public override string FormatColumn(string column)
        {
            return $"`{column}`";
        }

    }
}
