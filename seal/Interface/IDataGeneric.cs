using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using seal.Enumeration;

namespace seal.Interface
{
    /// <summary>
    /// Provide basic method needed to mapping between data object model and raw data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal interface IDataGenericOperation<T> where T : Model
    {
        T objectBuilder<T>(Dictionary<string, string> row, string tableName, Dictionary<string, PropertyInfo> fieldList) where T : Model, new();
        string queryBuilder<T> (Enumeration.Operation operation, T entity, Dictionary<string, PropertyInfo> fieldList) where T : Model, new();
    }
}
