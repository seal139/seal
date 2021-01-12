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

        public ExpressionBuilder And(ExpressionBuilder eb)
        {
            buffer += " AND " + eb.buffer;
            return this;
        }

        public ExpressionBuilder Or (ExpressionBuilder eb)
        {
            buffer += " OR " + eb.buffer;
            return this;
        }

        public ExpressionBuilder Like(ExpressionBuilder eb)
        {
            buffer += " LIKE " + eb.buffer;
            return this;
        }

        public ExpressionBuilder Between(ExpressionBuilder eb1, ExpressionBuilder eb2)
        {
            buffer += " BETWEEN " + eb1.buffer + " AND " + eb2;
            return this;
        }

        public ExpressionBuilder In(IList<ExpressionBuilder> str)
        {
            buffer += " IN (";
            bool first = true;
            foreach (ExpressionBuilder s in str)
            {
                if (!first)
                {
                    buffer += ",";
                }
                buffer += s.buffer;
            }
            buffer += ")";
            return this;
        }

        public ExpressionBuilder In(params ExpressionBuilder[] str)
        {
            buffer += " IN (";
            bool first = true;
            foreach (ExpressionBuilder s in str)
            {
                if (!first)
                {
                    buffer += ",";
                }
                buffer += s.buffer;
            }
            buffer += ")";
            return this;
        }

        public ExpressionBuilder In(params string[] str)
        {
            buffer += " IN (";
            bool first = true;
            foreach (string s in str)
            {
                if (!first)
                {
                    buffer += ",";
                }
                buffer += s;
            }
            buffer += ")";
            return this;
        }

        public ExpressionBuilder In(IList<string> str)
        {
            buffer += " IN (";
            bool first = true;
            foreach (string s in str)
            {
                if (!first)
                {
                    buffer += ",";
                }
                buffer += s;
            }
            buffer += ")";
            return this;
        }

        public ExpressionBuilder Equal(ExpressionBuilder eb)
        {
            buffer += " = " + eb.buffer;
            return this;
        }

        public ExpressionBuilder NotEqual(ExpressionBuilder eb)
        {
            buffer += " <> " + eb.buffer;
            return this;
        }

        public ExpressionBuilder GreaterThan(ExpressionBuilder eb)
        {
            buffer += " > " + eb.buffer;
            return this;
        }

        public ExpressionBuilder SmallerThan(ExpressionBuilder eb)
        {
            buffer += " < " + eb.buffer;
            return this;
        }

        public ExpressionBuilder GreaterThanOrEqual(ExpressionBuilder eb)
        {
            buffer += " >= " + eb.buffer;
            return this;
        }

        public ExpressionBuilder SmallerThanOrEqual(ExpressionBuilder eb)
        {
            buffer += " <= " + eb.buffer;
            return this;
        }

        public static ExpressionBuilder Group(ExpressionBuilder eb)
        {
            string buffer = "(";
            buffer += (string)eb;
            buffer += ")";
            return new ExpressionBuilder(buffer);
        }
        public ExpressionBuilder(string expression) { buffer = expression; }
        public ExpressionBuilder() {}
    }
}
