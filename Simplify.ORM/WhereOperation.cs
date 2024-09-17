using Simplify.ORM.Enumerations;

namespace Simplify.ORM
{
    public class WhereOperation
    {
        public SimplifyWhereOperation Operation { get; set; }
        public string LeftTable { get; set; }
        public string LeftColumn { get; set; }
        public string ParameterName { get; set; }
        public object ParameterValue { get; set; }

        public WhereOperation(SimplifyWhereOperation operation, string leftTable, string leftColumn, string parameterName, object parameterValue)
        {
            Operation = operation;
            LeftTable = leftTable;
            LeftColumn = leftColumn;
            ParameterName = parameterName;
            ParameterValue = parameterValue;
        }

        public WhereOperation(SimplifyWhereOperation operation)
        {
            Operation = operation;
        }
    }
}
