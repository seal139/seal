using seal.Enumeration;
using seal.Helper;
using seal.Interface;
using seal.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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

        public Func<IModel, object> CreateGetter(PropertyInfo property)
        {
            var parameter = Expression.Parameter(typeof(IModel), "i");
            var cast = Expression.TypeAs(parameter, property.DeclaringType);

            var getterBody = Expression.Property(cast, property);
            var output = Expression.TypeAs(getterBody, typeof(object));

            var exp = Expression.Lambda<Func<IModel, object>>(
                output, parameter);

            return exp.Compile();
        }

        public Action<IModel, object> CreateSetter(PropertyInfo property)
        {
            var parameter = Expression.Parameter(typeof(IModel), "i");
            var cast = Expression.TypeAs(parameter, property.DeclaringType);

            var input = Expression.Parameter(typeof(object), "p");
            UnaryExpression conv;
            //if (property.PropertyType.IsSubclassOf(typeof(Enum))){
            conv = Expression.Convert(input, property.PropertyType);
            //}
            //else
            //{
            //    conv = Expression.TypeAs(parameter, property.PropertyType);               
            //}


            var prop = Expression.Property(cast, property);

            Action<IModel, object> result = Expression.Lambda<Action<IModel, object>>
              (
                  Expression.Assign(prop, conv), parameter, input
              ).Compile();

            return result;
        }

        /// <summary>
        /// Serialize class to Raw format
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <returns></returns>
        public IList<object> Serialize<T>(T table) where T : IModel
        {
            ModelFactory factory = ModelFactory.GetInstance();
            TableInfo tInfo = factory[typeof(T).Name];
           return table.Unpack();
           
        }

        /// <summary>
        /// Deserialize raw format to class <br/>
        /// Please note that you cannot directly deserialize raw data from serialize method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="raw"></param>
        /// <returns></returns>
        public T Deserialize<T>(IList<object> raw) where T : IModel, new()
        {
            T obj = new T();
            obj.Pack(raw);
            return obj;
        }

        private string ValueConverter(object value)
        {
            if(value == null)
            {
                return "NULL";
            }

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

            if (value.GetType().IsSubclassOf(typeof(IModel)) || value is IModel)
            {
                return ((IModel)value).UniqueIdentifierValue;
            }

            if (value.GetType().IsSubclassOf(typeof(Enum)))
            {
                return ((int)value).ToString();
            }

            throw new ApiException("Invalid value for invoking to database");
        }

        public string CompileQuery(Operation operation, String table, IList<object> raw, string uniqueIdentifierField)
        {
            bool first = true;
            string query = "";

            switch (operation)
            {

                case Operation.Insert:
                    query = "INSERT INTO " + table + " ";

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
                    query = "UPDATE " + table + " SET ";

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
                    //query = "DELETE FROM " + table + " WHERE " + uniqueIdentifierField + " = " + raw[uniqueIdentifierField];
                    break;

                case Operation.SELECT:
                    query = "SELECT ";
                    foreach (KeyValuePair<string, object> keyVal in raw)
                    {
                        if (!first)
                        {
                            query += ", ";
                        }
                        first = false;
                        query += keyVal.Key;
                    }

                    query += "FROM " + table;
                    break;

                default:
                    throw new ApiException("Invalid SQL operation");
            }
            return query;
        }
    }
}
