using System.Text;
using Simplify.ORM.Enumerations;
using Simplify.ORM.Interfaces;
using Simplify.ORM.Utils;

namespace Simplify.ORM.Builders
{
    public abstract class AbstractSimplifyQueryBuilder : ISimplifyQueryBuilder
    {
        protected Dictionary<string, object> Parameters { get; set; } = [];
        protected List<string> Selects { get; set; } = [];
        protected List<Tuple<SimplifyJoinOperation, string>> Joins { get; set; } = [];
        protected List<WhereOperation> Wheres { get; set; } = [];
        protected List<Tuple<SimplifyOrderOperation, string>> OrdersBy { get; set; } = [];
        protected int? TopValue { get; set; }
        protected int? LimitValue { get; set; }

        protected string InsertTable { get; set; }

        public virtual string BuildQuery()
        {
            var sb = new StringBuilder();

            if (Selects.Any())
            {
                sb.Append($"SELECT ");
                if (TopValue.HasValue)
                    sb.Append($"TOP {TopValue} ");

                sb.Append(string.Join(", ", Selects));
                sb.Append(" ");
            }

            foreach (var join in Joins)
            {
                var operationSymbol = GetJoinOperationSymbol(join.Item1);
                var value = join.Item2;

                sb.Append($"{operationSymbol} {value} ");
            }

            if (Wheres.Any())
                sb.Append($"{GetWhereOperationSymbol(SimplifyWhereOperation.Where)} ");

            foreach (var where in Wheres)
            {
                var operationSymbol = GetWhereOperationSymbol(where.Operation);
                var parameterName = !string.IsNullOrEmpty(where.ParameterName) ? FormatParameterName(where.ParameterName) : null;

                if (!string.IsNullOrEmpty(where.LeftTable) && !string.IsNullOrEmpty(where.LeftColumn))
                {
                    var table = FormatTable(where.LeftTable);
                    var column = FormatColumn(where.LeftColumn);

                    if (where.ParameterValue is IEnumerable<object>)
                    {
                        sb.Append($"{table}.{column} {operationSymbol} ({SplitListParameters(where.ParameterName, where.ParameterValue)}) ");
                    }
                    else 
                        sb.Append($"{table}.{column} {operationSymbol} {parameterName} ");
                }
                else
                    sb.Append($"{operationSymbol} {parameterName} ");
            }

            if (OrdersBy.Any())
            {
                sb.Append($"ORDER BY ");

                var orderByText = OrdersBy.Select(x =>
                {
                    var operationSymbol = GetOrderByOperationSymbol(x.Item1);
                    var value = x.Item2;

                    return $"{value} {operationSymbol} ";
                });

                sb.Append(string.Join(", ", orderByText));

            }

            if (LimitValue.HasValue)
                sb.Append($"LIMIT {LimitValue} ");

            return sb.Append(";").ToString().Replace("  ", " ").TrimEnd();
        }

        private string SplitListParameters(string key, object value)
        {
            if (value is IEnumerable<object> list)
            {
                var sb = new StringBuilder();
                foreach(var v in list)
                {
                    var newParameter = GetParameterName(key);
                    Parameters.Add(newParameter, v);
                    sb.Append(FormatParameterName(newParameter)).Append(",");
                }
                if(sb.Length > 0)
                    sb.Remove(sb.Length - 1, 1);
                Parameters.Remove(key);
                return sb.ToString();
            }
            return key;
        }

        public Dictionary<string, object> GetParameters() => Parameters;

        public string TableName<T>() where T : SimplifyEntity
                    => SimplifyEntityHelper.TableName<T>();

        public string ColumnName<T>(string property) where T : SimplifyEntity
            => SimplifyEntityHelper.ColumnName<T>(property);


        #region Format

        public virtual string FormatTable(string table) => table;

        public virtual string FormatColumn(string column) => column;

        public virtual string FormatTableColumn(string table, string column) => $"{FormatTable(table)}.{FormatColumn(column)}";

        public virtual string FormatParameterName(string parameter) => $"@{parameter}";

        #endregion

        #region Add

        public virtual ISimplifyQueryBuilder AddParameter(string name, object value)
        {
            Parameters.Add(name, value);
            return this;
        }

        public virtual ISimplifyQueryBuilder AddSelect(string table, string column)
        {
            if (string.IsNullOrEmpty(column))
                Selects.Add($"{FormatTable(table)}.*");
            else
                Selects.Add(FormatTableColumn(table, column));
            return this;
        }

        public virtual ISimplifyQueryBuilder AddSelect(Tuple<string, string> tableColumn)
        {
            return AddSelect(tableColumn.Item1, tableColumn.Item2);
        }

        public virtual ISimplifyQueryBuilder AddSelect(List<Tuple<string, string>> tableColumns)
        {
            foreach (var tableColumn in tableColumns)
                AddSelect(tableColumn);
            return this;
        }

        public virtual ISimplifyQueryBuilder AddTop(int top)
        {
            TopValue = top;
            return this;
        }

        public ISimplifyQueryBuilder AddFrom(string table, string alias = null)
        {
            if (!string.IsNullOrEmpty(alias))
                Joins.Add(new Tuple<SimplifyJoinOperation, string>(SimplifyJoinOperation.From, $"{FormatTable(table)} {alias}"));
            else
                Joins.Add(new Tuple<SimplifyJoinOperation, string>(SimplifyJoinOperation.From, FormatTable(table)));
            return this;
        }

        public ISimplifyQueryBuilder AddJoin(SimplifyJoinOperation operation, string table)
        {
            Joins.Add(new Tuple<SimplifyJoinOperation, string>(operation, FormatTable(table)));
            return this;
        }

        public ISimplifyQueryBuilder AddJoin(SimplifyJoinOperation operation, string table, string column)
        {
            Joins.Add(new Tuple<SimplifyJoinOperation, string>(operation, $"{FormatTable(table)}.{FormatColumn(column)}"));
            return this;
        }

        public ISimplifyQueryBuilder AddWhere(SimplifyWhereOperation operation)
        {
            Wheres.Add(new WhereOperation(operation));
            return this;
        }

        public ISimplifyQueryBuilder AddWhere(SimplifyWhereOperation operation, string table, string column, object value)
        {
            var parameterName = GetParameterName(column);

            AddParameter(parameterName, value);

            Wheres.Add(new WhereOperation(operation, table, column, parameterName, value));
            return this;
        }

        public ISimplifyQueryBuilder AddWhere(SimplifyWhereOperation operation, string parameter, object value)
        {
            var parameterName = GetParameterName(parameter);

            AddParameter(parameterName, value);

            Wheres.Add(new WhereOperation(operation, null, null, parameterName, value));
            return this;
        }

        public virtual ISimplifyQueryBuilder AddLimit(int limit)
        {
            LimitValue = limit;
            return this;
        }

        public ISimplifyQueryBuilder AddOrderBy(string table, string column, SimplifyOrderOperation operation = SimplifyOrderOperation.Asc)
        {
            OrdersBy.Add(new Tuple<SimplifyOrderOperation, string>(operation, $"{FormatTable(table)}.{FormatColumn(column)}"));
            return this;
        }

        #endregion

        public virtual string GetJoinOperationSymbol(SimplifyJoinOperation operation) => operation switch
        {
            SimplifyJoinOperation.From => "FROM",
            SimplifyJoinOperation.On => "ON",
            SimplifyJoinOperation.Equals => "=",
            SimplifyJoinOperation.Join => "JOIN",
            SimplifyJoinOperation.InnerJoin => "INNER JOIN",
            SimplifyJoinOperation.LeftJoin => "LEFT JOIN",
            SimplifyJoinOperation.RightJoin => "RIGHT JOIN",
            _ => throw new ArgumentException("Invalid operation for this method", nameof(operation))
        };

        public virtual string GetWhereOperationSymbol(SimplifyWhereOperation operation) => operation switch
        {
            SimplifyWhereOperation.Where => "WHERE",
            SimplifyWhereOperation.Or => "OR",
            SimplifyWhereOperation.And => "AND",
            SimplifyWhereOperation.Equals => "=",
            SimplifyWhereOperation.NotEquals => "<>",
            SimplifyWhereOperation.Greater => ">",
            SimplifyWhereOperation.GreaterOrEqual => ">=",
            SimplifyWhereOperation.Lower => "<",
            SimplifyWhereOperation.LowerOrEqual => "<=",
            SimplifyWhereOperation.Between => "BETWEEN",
            SimplifyWhereOperation.In => "IN",
            _ => throw new ArgumentException("Invalid operation for this method", nameof(operation))
        };

        public virtual string GetOrderByOperationSymbol(SimplifyOrderOperation operation) => operation switch
        {
            SimplifyOrderOperation.Asc => "ASC",
            SimplifyOrderOperation.Desc => "DESC",
            _ => throw new ArgumentException("Invalid operation for this method", nameof(operation))
        };

        private string GetParameterName(string parameter)
        {
            var parameterNumber = 0;
            var parameterName = $"{parameter}{parameterNumber}";
            while (Parameters.ContainsKey(parameterName))
            {
                parameterNumber++;
                parameterName = $"{parameter}{parameterNumber}";
            }

            return parameterName;
        }

        #region Select

        public ISimplifyQueryBuilder SelectFields(string table, List<string> columns)
        {
            var selects = columns.Select(c => new Tuple<string, string>(table, c)).ToList();

            return AddSelect(selects);
        }

        public ISimplifyQueryBuilder SelectAllFields(string table)
            => AddSelect(table, string.Empty);

        public ISimplifyQueryBuilder SelectAllFieldsFrom(string table)
        {
            AddSelect(table, string.Empty);
            AddFrom(table);
            return this;     
        }

        public ISimplifyQueryBuilder Top(int top) => AddTop(top);

        public ISimplifyQueryBuilder Limit(int limit) => AddLimit(limit);

        #endregion

        #region Join

        private ISimplifyQueryBuilder JoinBase(SimplifyJoinOperation operation, string rightTable, string rightColumn, string leftTable, string leftColumn)
        {
            return AddJoin(operation, rightTable)
                .AddJoin(SimplifyJoinOperation.On, rightTable, rightColumn)
                .AddJoin(SimplifyJoinOperation.Equals, leftTable, leftColumn);
        }

        public ISimplifyQueryBuilder From(string tableName, string alias = null)
            => AddFrom(tableName, alias);

        public ISimplifyQueryBuilder Join(string rightTable, string rightColumn, string leftTable, string leftColumn)
            => JoinBase(SimplifyJoinOperation.Join, rightTable, rightColumn, leftTable, leftColumn);

        public ISimplifyQueryBuilder InnerJoin(string rightTable, string rightColumn, string leftTable, string leftColumn)
            => JoinBase(SimplifyJoinOperation.InnerJoin, rightTable, rightColumn, leftTable, leftColumn);

        public ISimplifyQueryBuilder LeftJoin(string rightTable, string rightColumn, string leftTable, string leftColumn)
            => JoinBase(SimplifyJoinOperation.LeftJoin, rightTable, rightColumn, leftTable, leftColumn);

        public ISimplifyQueryBuilder RightJoin(string rightTable, string rightColumn, string leftTable, string leftColumn)
            => JoinBase(SimplifyJoinOperation.RightJoin, rightTable, rightColumn, leftTable, leftColumn);

        #endregion

        #region Where

        private ISimplifyQueryBuilder WhereBase(SimplifyWhereOperation operation, string tableName, string column, object value, bool conditional)
        {
            if (!conditional) 
                return this; 

            return AddWhere(operation, tableName, column, value);
        }

        public ISimplifyQueryBuilder WhereEquals(string table, string column, object value, bool conditional = true)
            => WhereBase(SimplifyWhereOperation.Equals, table, column, value, conditional);

        public ISimplifyQueryBuilder WhereNotEquals(string table, string column, object value, bool conditional = true)
            => WhereBase(SimplifyWhereOperation.NotEquals, table, column, value, conditional);

        public ISimplifyQueryBuilder WhereGreater(string table, string column, object value, bool conditional = true)
            => WhereBase(SimplifyWhereOperation.Greater, table, column, value, conditional);

        public ISimplifyQueryBuilder WhereGreaterOrEqual(string table, string column, object value, bool conditional = true)
            => WhereBase(SimplifyWhereOperation.GreaterOrEqual, table, column, value, conditional);

        public ISimplifyQueryBuilder WhereLower(string table, string column, object value, bool conditional = true)
            => WhereBase(SimplifyWhereOperation.Lower, table, column, value, conditional);

        public ISimplifyQueryBuilder WhereLowerOrEqual(string table, string column, object value, bool conditional = true)
            => WhereBase(SimplifyWhereOperation.LowerOrEqual, table, column, value, conditional);

        public ISimplifyQueryBuilder WhereBetween(string tableName, string column, object from, object to, bool conditional = true)
        {
            if (!conditional) { return this; }

            return AddWhere(SimplifyWhereOperation.Between, tableName, column, from).AddWhere(SimplifyWhereOperation.And, column, to);
        }

        public ISimplifyQueryBuilder WhereIn(string table, string column, IEnumerable<object> value, bool conditional = true)
            => WhereBase(SimplifyWhereOperation.In, table, column, value, conditional);

        #endregion

        #region And

        private ISimplifyQueryBuilder AndBase(SimplifyWhereOperation operation, string tableName, string column, object value, bool conditional)
        {
            if (!conditional) { return this; }

            return AddWhere(SimplifyWhereOperation.And).AddWhere(operation, tableName, column, value);
        }

        public ISimplifyQueryBuilder AndEquals(string tableName, string column, object value, bool conditional = true)
            => AndBase(SimplifyWhereOperation.Equals, tableName, column, value, conditional);

        public ISimplifyQueryBuilder AndNotEquals(string tableName, string column, object value, bool conditional = true)
            => AndBase(SimplifyWhereOperation.NotEquals, tableName, column, value, conditional);

        public ISimplifyQueryBuilder AndGreater(string tableName, string column, object value, bool conditional = true)
            => AndBase(SimplifyWhereOperation.Greater, tableName, column, value, conditional);

        public ISimplifyQueryBuilder AndGreaterOrEqual(string tableName, string column, object value, bool conditional = true)
            => AndBase(SimplifyWhereOperation.GreaterOrEqual, tableName, column, value, conditional);

        public ISimplifyQueryBuilder AndLower(string tableName, string column, object value, bool conditional = true)
            => AndBase(SimplifyWhereOperation.Lower, tableName, column, value, conditional);

        public ISimplifyQueryBuilder AndLowerOrEqual(string tableName, string column, object value, bool conditional = true)
            => AndBase(SimplifyWhereOperation.LowerOrEqual, tableName, column, value, conditional);

        public ISimplifyQueryBuilder AndBetween(string tableName, string column, object from, object to, bool conditional = true)
            => AndBase(SimplifyWhereOperation.Between, tableName, column, from, conditional).AddWhere(SimplifyWhereOperation.And, column, to);

        #endregion

        #region Or
        private ISimplifyQueryBuilder OrBase(SimplifyWhereOperation operation, string tableName, string column, object value, bool conditional)
        {
            if (!conditional) { return this; }

            return AddWhere(SimplifyWhereOperation.Or).AddWhere(operation, tableName, column, value);
        }

        public ISimplifyQueryBuilder OrEquals(string tableName, string column, object value, bool conditional = true)
            => OrBase(SimplifyWhereOperation.Equals, tableName, column, value, conditional);

        public ISimplifyQueryBuilder OrNotEquals(string tableName, string column, object value, bool conditional = true)
            => OrBase(SimplifyWhereOperation.NotEquals, tableName, column, value, conditional);

        public ISimplifyQueryBuilder OrGreater(string tableName, string column, object value, bool conditional = true)
            => OrBase(SimplifyWhereOperation.Greater, tableName, column, value, conditional);

        public ISimplifyQueryBuilder OrGreaterEqual(string tableName, string column, object value, bool conditional = true)
            => OrBase(SimplifyWhereOperation.GreaterOrEqual, tableName, column, value, conditional);

        public ISimplifyQueryBuilder OrLower(string tableName, string column, object value, bool conditional = true)
            => OrBase(SimplifyWhereOperation.Lower, tableName, column, value, conditional);

        public ISimplifyQueryBuilder OrLowerEqual(string tableName, string column, object value, bool conditional = true)
            => OrBase(SimplifyWhereOperation.LowerOrEqual, tableName, column, value, conditional);

        public ISimplifyQueryBuilder OrBetween(string tableName, string column, object from, object to, bool conditional = true)
            => OrBase(SimplifyWhereOperation.Between, tableName, column, from, conditional).AddWhere(SimplifyWhereOperation.And, column, to);

        #endregion

        #region Order By
        public ISimplifyQueryBuilder OrderBy(string table, string column, SimplifyOrderOperation operation = SimplifyOrderOperation.Desc)
        {
            return AddOrderBy(table, column, operation);
        }

        #endregion
    }
}
