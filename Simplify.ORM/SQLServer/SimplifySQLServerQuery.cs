namespace Simplify.ORM.SQLServer
{
    public class SimplifySQLServerQuery : BaseSimplifyQuery
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
        public ISimplifyQuery AddLimit(int limit)
        {
            return this;
        }
    }
}
