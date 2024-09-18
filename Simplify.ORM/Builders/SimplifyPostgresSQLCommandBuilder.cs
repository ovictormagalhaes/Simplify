namespace Simplify.ORM.Builders
{
    public sealed class SimplifyPostgresSQLCommandBuilder : AbstractSimplifyCommandBuilder
    {
        public override string FormatTable(string table)
        {
            return $"\"{table}\"";
        }

        public override string FormatColumn(string column)
        {
            return $"\"{column}\"";
        }
    }
}
