using seal.Enumeration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace seal.Helper
{
    public class ExpressionBuilder
    {
        private string buffer = "";
        public static explicit operator string(ExpressionBuilder eb)
        {
            return eb.buffer;
        }

        public static implicit operator ExpressionBuilder(string str)
        {
            return new ExpressionBuilder(str);
        }

        public static ExpressionBuilder Group(ExpressionBuilder eb)
        {
            string buffer = "(";
            buffer += eb.buffer;
            buffer += ")";
            return new ExpressionBuilder(buffer);
        }
        public ExpressionBuilder(string expression) { buffer = expression; }
        public ExpressionBuilder(ExpressionBuilder left, LogicalOperator opr, params ExpressionBuilder[] right)
        {
            buffer = left.buffer + " " + opr.ToString();
            if(opr == LogicalOperator.BETWEEN)
            {
                buffer += " " + right[0].buffer + " AND " + right[1].buffer;
                return;
            }

            if(opr == LogicalOperator.In)
            {
                buffer += " {";
                bool first = true;
                foreach(ExpressionBuilder eb in right)
                {
                    if (!first) buffer += ", ";
                    buffer += eb.buffer;
                }
                buffer += "}";
                return;
            }

            buffer += " " + right[0];
        }

        public ExpressionBuilder(LinkOperator link, ExpressionBuilder left, LogicalOperator opr, params ExpressionBuilder[] right)
        {
            buffer = link.ToString() + " " + left.buffer + " " + opr.ToString();
            if (opr == LogicalOperator.BETWEEN)
            {
                buffer += " " + right[0].buffer + " AND " + right[1].buffer;
                return;
            }

            if (opr == LogicalOperator.In)
            {
                buffer += " {";
                bool first = true;
                foreach (ExpressionBuilder eb in right)
                {
                    if (!first) buffer += ", ";
                    buffer += eb.buffer;
                }
                buffer += "}";
                return;
            }

            buffer += " " + right[0];
        }
    }
}
