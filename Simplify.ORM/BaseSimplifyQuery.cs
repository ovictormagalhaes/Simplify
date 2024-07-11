﻿using System.Text;

namespace Simplify.ORM
{
    public abstract partial class BaseSimplifyQuery : ISimplifyQuery
    {
        public string Query { get => BuildQuery(); }
        protected Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();
        protected List<string> Selects { get; set; } = new List<string>();
        protected List<Tuple<SimplifyJoinOperation, string>> Joins { get; set; } = new List<Tuple<SimplifyJoinOperation, string>>();
        protected List<WhereOperation> Wheres { get; set; } = new List<WhereOperation>();
        protected List<Tuple<SimplifyOrderOperation, string>> OrdersBy { get; set; } = new List<Tuple<SimplifyOrderOperation, string>>();
        protected int? TopValue { get; set; }
        protected int? LimitValue { get; set; }

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

                    return $"{value} {operationSymbol}";
                });

                sb.Append(string.Join(", ", orderByText));

            }


            if (LimitValue.HasValue)
                sb.Append($"LIMIT {LimitValue}");

            sb.Append(";");

            return sb.ToString().Replace("  ", " ").TrimEnd();
        }

        public Dictionary<string, object> GetParameters() => Parameters;

        #region Format

        public virtual string FormatTable(string table)
        {
            return table;
        }

        public virtual string FormatColumn(string column)
        {
            return column;
        }

        public virtual string FormatTableColumn(string table, string column)
        {
            return $"{FormatTable(table)}.{FormatColumn(column)}";
        }

        public virtual string FormatParameterName(string parameter)
        {
            return $"@{parameter}";
        }

        #endregion

        #region Add

        public virtual ISimplifyQuery AddParameter(string name, object value)
        {
            Parameters.Add(name, value);
            return this;
        }

        public virtual ISimplifyQuery AddSelect(string table, string column)
        {
            if (string.IsNullOrEmpty(column))
                Selects.Add($"{FormatTable(table)}.*");
            else
                Selects.Add(FormatTableColumn(table, column));
            return this;
        }

        public virtual ISimplifyQuery AddSelect(Tuple<string, string> tableColumn)
        {
            return AddSelect(tableColumn.Item1, tableColumn.Item2);
        }

        public virtual ISimplifyQuery AddSelect(List<Tuple<string, string>> tableColumns)
        {
            foreach (var tableColumn in tableColumns)
                AddSelect(tableColumn);
            return this;
        }

        public virtual ISimplifyQuery AddTop(int top)
        {
            TopValue = top;
            return this;
        }

        public ISimplifyQuery AddFrom(string table, string? alias = null)
        {
            if (!string.IsNullOrEmpty(alias))
                Joins.Add(new Tuple<SimplifyJoinOperation, string>(SimplifyJoinOperation.From, $"{FormatTable(table)} {alias}"));
            else
                Joins.Add(new Tuple<SimplifyJoinOperation, string>(SimplifyJoinOperation.From, FormatTable(table)));
            return this;
        }

        public ISimplifyQuery AddJoin(SimplifyJoinOperation operation, string table)
        {
            Joins.Add(new Tuple<SimplifyJoinOperation, string>(operation, FormatTable(table)));
            return this;
        }

        public ISimplifyQuery AddJoin(SimplifyJoinOperation operation, string table, string column)
        {
            Joins.Add(new Tuple<SimplifyJoinOperation, string>(operation, $"{FormatTable(table)}.{FormatColumn(column)}"));
            return this;
        }

        public ISimplifyQuery AddWhere(SimplifyWhereOperation operation)
        {
            Wheres.Add(new WhereOperation(operation));
            return this;
        }

        public ISimplifyQuery AddWhere(SimplifyWhereOperation operation, string table, string column, object value)
        {
            var parameterName = GetParameterName(column);

            AddParameter(parameterName, value);

            Wheres.Add(new WhereOperation(operation, table, column, parameterName, value));
            return this;
        }

        public ISimplifyQuery AddWhere(SimplifyWhereOperation operation, string parameter, object value)
        {
            var parameterName = GetParameterName(parameter);

            AddParameter(parameterName, value);

            Wheres.Add(new WhereOperation(operation, null, null, parameterName, value));
            return this;
        }

        public virtual ISimplifyQuery AddLimit(int limit)
        {
            LimitValue = limit;
            return this;
        }

        public ISimplifyQuery AddOrderBy(string table, string column, SimplifyOrderOperation operation = SimplifyOrderOperation.Asc)
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

        public ISimplifyQuery SelectFields(string tableName, List<string> columns)
        {
            var selects = columns.Select(c => new Tuple<string, string>(tableName, c)).ToList();

            return AddSelect(selects);
        }

        public ISimplifyQuery SelectAllFields(string tableName)
            => AddSelect(tableName, string.Empty);

        public ISimplifyQuery Top(int top)
        {
            return AddTop(top);
        }

        public ISimplifyQuery Limit(int limit)
        {
            return AddLimit(limit);
        }

        #endregion

        #region Join

        private ISimplifyQuery JoinBase(SimplifyJoinOperation operation, string rightTable, string rightColumn, string leftTable, string leftColumn)
        {
            return AddJoin(operation, rightTable)
                .AddJoin(SimplifyJoinOperation.On, rightTable, rightColumn)
                .AddJoin(SimplifyJoinOperation.Equals, leftTable, leftColumn);
        }

        public ISimplifyQuery From(string tableName, string? alias = null)
            => AddFrom(tableName, alias);

        public ISimplifyQuery Join(string rightTable, string rightColumn, string leftTable, string leftColumn)
            => JoinBase(SimplifyJoinOperation.Join, rightTable, rightColumn, leftTable, leftColumn);

        public ISimplifyQuery InnerJoin(string rightTable, string rightColumn, string leftTable, string leftColumn)
            => JoinBase(SimplifyJoinOperation.InnerJoin, rightTable, rightColumn, leftTable, leftColumn);

        public ISimplifyQuery LeftJoin(string rightTable, string rightColumn, string leftTable, string leftColumn)
            => JoinBase(SimplifyJoinOperation.LeftJoin, rightTable, rightColumn, leftTable, leftColumn);

        public ISimplifyQuery RightJoin(string rightTable, string rightColumn, string leftTable, string leftColumn)
            => JoinBase(SimplifyJoinOperation.RightJoin, rightTable, rightColumn, leftTable, leftColumn);

        #endregion

        #region Where

        private ISimplifyQuery WhereBase(SimplifyWhereOperation operation, string tableName, string column, object value, bool conditional)
        {
            if (!conditional) { return this; }

            return AddWhere(operation, tableName, column, value);
        }

        public ISimplifyQuery WhereEquals(string tableName, string column, object value, bool conditional = true)
            => WhereBase(SimplifyWhereOperation.Equals, tableName, column, value, conditional);

        public ISimplifyQuery WhereNotEquals(string tableName, string column, object value, bool conditional = true)
            => WhereBase(SimplifyWhereOperation.NotEquals, tableName, column, value, conditional);

        public ISimplifyQuery WhereGreater(string tableName, string column, object value, bool conditional = true)
            => WhereBase(SimplifyWhereOperation.Greater, tableName, column, value, conditional);

        public ISimplifyQuery WhereGreaterOrEqual(string tableName, string column, object value, bool conditional = true)
            => WhereBase(SimplifyWhereOperation.GreaterOrEqual, tableName, column, value, conditional);

        public ISimplifyQuery WhereLower(string tableName, string column, object value, bool conditional = true)
            => WhereBase(SimplifyWhereOperation.Lower, tableName, column, value, conditional);

        public ISimplifyQuery WhereLowerOrEqual(string tableName, string column, object value, bool conditional = true)
            => WhereBase(SimplifyWhereOperation.LowerOrEqual, tableName, column, value, conditional);

        public ISimplifyQuery WhereBetween(string tableName, string column, object from, object to, bool conditional = true)
        {
            if (!conditional) { return this; }

            return AddWhere(SimplifyWhereOperation.Between, tableName, column, from).AddWhere(SimplifyWhereOperation.And, column, to);
        }

        #endregion

        #region And

        private ISimplifyQuery AndBase(SimplifyWhereOperation operation, string tableName, string column, object value, bool conditional)
        {
            if (!conditional) { return this; }

            return AddWhere(SimplifyWhereOperation.And).AddWhere(operation, tableName, column, value);
        }

        public ISimplifyQuery AndEquals(string tableName, string column, object value, bool conditional = true)
            => AndBase(SimplifyWhereOperation.Equals, tableName, column, value, conditional);

        public ISimplifyQuery AndNotEquals(string tableName, string column, object value, bool conditional = true)
            => AndBase(SimplifyWhereOperation.NotEquals, tableName, column, value, conditional);

        public ISimplifyQuery AndGreater(string tableName, string column, object value, bool conditional = true)
            => AndBase(SimplifyWhereOperation.Greater, tableName, column, value, conditional);

        public ISimplifyQuery AndGreaterOrEqual(string tableName, string column, object value, bool conditional = true)
            => AndBase(SimplifyWhereOperation.GreaterOrEqual, tableName, column, value, conditional);

        public ISimplifyQuery AndLower(string tableName, string column, object value, bool conditional = true)
            => AndBase(SimplifyWhereOperation.Lower, tableName, column, value, conditional);

        public ISimplifyQuery AndLowerOrEqual(string tableName, string column, object value, bool conditional = true)
            => AndBase(SimplifyWhereOperation.LowerOrEqual, tableName, column, value, conditional);

        public ISimplifyQuery AndBetween(string tableName, string column, object from, object to, bool conditional = true)
            => AndBase(SimplifyWhereOperation.Between, tableName, column, from, conditional).AddWhere(SimplifyWhereOperation.And, column, to);

        #endregion

        #region Or
        private ISimplifyQuery OrBase(SimplifyWhereOperation operation, string tableName, string column, object value, bool conditional)
        {
            if (!conditional) { return this; }

            return AddWhere(SimplifyWhereOperation.Or).AddWhere(operation, tableName, column, value);
        }

        public ISimplifyQuery OrEquals(string tableName, string column, object value, bool conditional = true)
            => OrBase(SimplifyWhereOperation.Equals, tableName, column, value, conditional);

        public ISimplifyQuery OrNotEquals(string tableName, string column, object value, bool conditional = true)
            => OrBase(SimplifyWhereOperation.NotEquals, tableName, column, value, conditional);

        public ISimplifyQuery OrGreater(string tableName, string column, object value, bool conditional = true)
            => OrBase(SimplifyWhereOperation.Greater, tableName, column, value, conditional);

        public ISimplifyQuery OrGreaterEqual(string tableName, string column, object value, bool conditional = true)
            => OrBase(SimplifyWhereOperation.GreaterOrEqual, tableName, column, value, conditional);

        public ISimplifyQuery OrLower(string tableName, string column, object value, bool conditional = true)
            => OrBase(SimplifyWhereOperation.Lower, tableName, column, value, conditional);

        public ISimplifyQuery OrLowerEqual(string tableName, string column, object value, bool conditional = true)
            => OrBase(SimplifyWhereOperation.LowerOrEqual, tableName, column, value, conditional);

        public ISimplifyQuery OrBetween(string tableName, string column, object from, object to, bool conditional = true)
            => OrBase(SimplifyWhereOperation.Between, tableName, column, from, conditional).AddWhere(SimplifyWhereOperation.And, column, to);

        #endregion

        #region Order By
        public ISimplifyQuery OrderBy(string table, string column, SimplifyOrderOperation operation = SimplifyOrderOperation.Desc)
        {
            return AddOrderBy(table, column, operation);
        }

        #endregion
    }
}
