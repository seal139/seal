using seal.Base;
using seal.Enumeration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace seal.Helper
{
    class SqlExpressionBuilder : ExpressionBuilder
    {

        public SqlExpressionBuilder(string columnName, LogicalOperator operation, params string[] value) : base(columnName, operation, value) { }

        protected override string GetStringQuery()
        {
            switch (Operation)
            {
                case LogicalOperator.Equals:
                    return ColumnName + " = " + Value[0];
                case LogicalOperator.NotEquals:
                    return ColumnName + " != " + Value[0];
                case LogicalOperator.LessOrEquals:
                    return ColumnName + " <= " + Value[0];
                case LogicalOperator.MoreOrEquals:
                    return ColumnName + " >= " + Value[0];
                case LogicalOperator.MoreThan:
                    return ColumnName + " > " + Value[0];
                case LogicalOperator.LessThan:
                    return ColumnName + " < " + Value[0];

                case LogicalOperator.LIKE:
                    return ColumnName + " LIKE " + Value[0];

                case LogicalOperator.BETWEEN:
                    return ColumnName + " BETWEEN " + Value[0] + " AND " + Value[1];
                case LogicalOperator.In:
                    bool first = true;
                    string buffer = "";
                    foreach(string v in Value)
                    {
                        if (first)
                        {
                            buffer = v;
                            first = false;
                        }
                        else
                        {
                            buffer += ", " + v;
                        }
                    }
                    return ColumnName + " = (" + buffer + ")";

                default:
                    return "1 = 1";
            }
        }
    }
}
