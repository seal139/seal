using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace seal.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class Column : System.Attribute
    {
        public string ColumnName { set; get; }
        public Column(string columnName)
        {
            this.ColumnName = columnName;
        }
    }
}
