using seal.Enumeration;
using seal.Helper;
using seal.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace seal.IntfImpl
{
    /// <summary>
    /// Provide serialization mechanism between raw data and object model <br/>
    /// This singleton class cannot be inherited.
    /// </summary>
    public sealed class Serializer : ISerialization
    {
        public static Serializer Instance { get; } = new Serializer();

        private Serializer() { }

        /// <summary>
        /// Serialize class to Raw format
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <returns></returns>
        public IDictionary<string, object> Serialize<T>(T table) where T : IModel
        {
            ModelFactory factory = ModelFactory.GetInstance();
            TableInfo tInfo = factory[typeof(T).Name];
            IDictionary<string, object> rawData = table.Unpack();
            IDictionary<string, object> convertedData = new Dictionary<string, object>();
            foreach (KeyValuePair<string, object> pair in rawData)
            {
                convertedData.Add(tInfo[pair.Key].FieldName, pair.Value);
            }
            return convertedData;
        }

        /// <summary>
        /// Deserialize raw format to class <br/>
        /// Please note that you cannot directly deserialize raw data from serialize method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="raw"></param>
        /// <returns></returns>
        public T Deserialize<T>(IDictionary<string, object> raw) where T : IModel, new()
        {
            T obj = new T();
            obj.Pack(raw);
            return obj;
        }

        public override string CompileQuery(Operation operation, string tableName, Dictionary<string, object> raw)
        {
            bool first = true;
            string query = "";

            switch (operation)
            {

                case Operation.Insert:
                    string column = "(";
                    string value = ") VALUES (";

                    foreach (KeyValuePair<string, object> keyVal in raw)
                    {
                        if (!first)
                        {
                            column += ", ";
                            value += ", ";
                        }

                        first = false;
                        column += keyVal.Key;
                        value += ValueConverter(keyVal.Value);
                    }

                    query += column + value + ")";
                    break;

                case Operation.Update:
                    query = " SET ";

                    foreach (KeyValuePair<string, object> keyVal in raw)
                    {
                        if (!first)
                        {
                            query += ", ";
                        }
                        first = false;
                        query += keyVal.Key + " = " + ValueConverter(keyVal.Value);

                    }
                    break;

                case Operation.Delete:
                    query = "DELETE FROM " + typeof(T).Name + " WHERE id = '" + ((Model)entity).Id.ToString() + "'";
                    break;

                default:
                    throw new ApiException("Invalid SQL operation");

            }
            return query;
        }

        private string ValueConverter(object value)
        {
            if (value.GetType().IsSubclassOf(typeof(DateTime)) || value is DateTime)
            {
                return "'" + DateTimeUtility.GetSqlFormatDate((DateTime)value) + "'";
            }

            if ((value.GetType().IsSubclassOf(typeof(long)) || value is long) ||
                (value.GetType().IsSubclassOf(typeof(int)) || value is int) ||
                (value.GetType().IsSubclassOf(typeof(short)) || value is short) ||
                (value.GetType().IsSubclassOf(typeof(double)) || value is double) ||
                (value.GetType().IsSubclassOf(typeof(float)) || value is float))
            {
                return value.ToString();
            }

            if (value.GetType().IsSubclassOf(typeof(string)) || value is string)
            {
                return "'" + value.ToString() + "'";
            }

            if (value.GetType().IsSubclassOf(typeof(bool)) || value is bool)
            {
                if ((bool)value == true)
                {
                    return "1";
                }
                return "0";
            }

            if (value.GetType().IsSubclassOf(typeof(ModelTable)) || value is ModelTable)
            {
                return ((ModelTable)value).Id.ToString();
            }

            if (value.GetType().IsSubclassOf(typeof(Enum)))
            {
                return ((int)value).ToString();
            }

            throw new ApiException("Invalid value for invoking to database");
        }
    }
}
