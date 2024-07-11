namespace Simplify.ORM.MySQL
{
    public class SimplifyMySQLQuery : BaseSimplifyQuery
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
        public ISimplifyQuery AddTop(int top)
        {
            return this;
        }
    }
}
