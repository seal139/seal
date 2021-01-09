using seal.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace seal.Interface
{
    public interface IQueryBuilder
    {
        string AutoQuery(TableInfo tableInfo, string primaryField, IList<object> rawField, bool isInsert); // insert / update 
        string AutoQuery(TableInfo tableInfo, string primaryField, object primaryKeyValue); // delete
        string AutoQuery(TableInfo tableInfo, string primaryField); // select
        string AutoQuery(TableInfo tableInfo, string[] whereClauses); // select
        string BuildQuery(string table, string[] columns, string[] whereClauses);
    }
}
