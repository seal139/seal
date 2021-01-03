using seal.Enumeration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace seal.Base
{
    public abstract class ExpressionBuilder
    {
        public ExpressionBuilder(string columnName, LogicalOperator operation, params string[] value)
        {
            this.ColumnName = columnName;
            this.Operation = operation;
            this.Value = value;
        }

        public string ColumnName { set; get; }
        public string[] Value { set; get; }
        public LogicalOperator Operation { set; get; }

        protected abstract string GetStringQuery();
    }
}
