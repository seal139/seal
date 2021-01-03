using seal.Attribtues;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace seal
{
    

    public interface IMappingi
    {
        List<T> ObjectBuilder<T>(List<List<string>> rawSqlData) where T : ModelBase;
        string QueryBuilderSelect<T>(bool join) where T : Model;
        string QueryBuilderSelect<T>() where T : ModelView;
        string QueryBuilderNonSelect<T> (T entity, Operation opr) where T : Model;
    }

    public interface IDbTrasaction
    {
        void Begin();
        void Close();
        void InitDb(string server, string database, string username, string password);
        void InitDb(string server, string database);
        List<List<string>> TransactSelect(string query);
        void TransactModify(string query);
    }


    public abstract class ICore : IMapping, IDbTrasaction
    {
        public abstract void Begin();
        public abstract void Close();
        public abstract void InitMapping<T>();
        public abstract void InitDb(string server, string database, string username, string password);
        public abstract void InitDb(string server, string database);
        public abstract List<T> ObjectBuilder<T>(List<List<string>> rawSqlData) where T : ModelBase;
        public abstract string QueryBuilderNonSelect<T>(T entity, Operation opr) where T : Model;
        public abstract string QueryBuilderSelect<T>(bool join) where T : Model;
        public abstract string QueryBuilderSelect<T>() where T : ModelView;
        public abstract void TransactModify(string query);
        public abstract List<List<string>> TransactSelect(string query);
    }

    class Core : ICore
    { 
        private static bool initiated = false;
        private static Core core;
        public static Core GetInstance()
        {
            if (!initiated)
            {
                core = new Core();
                initiated = true;
            }
            return core;
        }
     
        private Core()
        {

        }

        //=========== DATABASE TRANSACTION ===========

        private SqlConnection con;

        /// <summary>
        /// Open database connection
        /// </summary>
        public sealed override void Begin()
        {
            con.Open();
        }

        /// <summary>
        /// Close database connection
        /// </summary>
        public sealed override void Close()
        {
            con.Close();
        }

        public sealed override void InitMapping<T>()
        {
            string tableName = typeof(T).Name;
            DataFactory.AddDataType(typeof(T).Name, typeof(T));

            Dictionary<string, Tuple<string, PropertyInfo>> fieldList = new Dictionary<string, Tuple<string, PropertyInfo>>();
            Dictionary<string, KeyValuePair<string, bool>> isAnotherEntity = new Dictionary<string, KeyValuePair<string, bool>>();

            PropertyInfo[] property_infos = (typeof(T)).GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public);
            foreach (PropertyInfo info in property_infos)
            {
                object[] attrs = info.GetCustomAttributes(true);
                string alias = info.Name;
                foreach (object attr in attrs)
                {
                    ColumnAttribute columnName = attr as ColumnAttribute;
                    if (columnName != null)
                    {
                        alias = columnName.ColumnName;
                    }
                }
                string attributes = info.PropertyType.Attributes.ToString();

                if (!info.PropertyType.IsArray)
                {
                    if (info.CanRead && info.CanWrite)
                    {
                        fieldList.Add(info.Name, new Tuple<string, PropertyInfo>(alias, info));
                        if (info.PropertyType.IsSubclassOf(typeof(Model)))
                        {
                            isAnotherEntity.Add(info.Name, new KeyValuePair<string, bool>(info.PropertyType.Name, true));
                        }
                        else
                        {
                            isAnotherEntity.Add(info.Name, new KeyValuePair<string, bool>(info.Name, false));
                        }
                    }
                }
            }

            object[] entityAttr = typeof(T).GetCustomAttributes(true);
            string entityAlias = tableName;
            foreach (object attr in entityAttr)
            {
                Entity alias = attr as Entity;
                if (alias != null)
                {
                    entityAlias = alias.TableName;
                }
            }


            Tuple<string, Dictionary<string, Tuple<string, PropertyInfo>>> columnInfo = new Tuple<string, Dictionary<string, Tuple<string, PropertyInfo>>>("", fieldList);
            DataFactory.AddAlias(tableName, entityAlias);
            DataFactory.AddMap(tableName, fieldList, isAnotherEntity);
        }

        /// <summary>
        /// Initialize database connection
        /// </summary>
        /// <param name="server">Server name. Eg: SEPTIAN-WST\SQLSERVER</param>
        /// <param name="database">Database name. Eg: tableExample</param>
        /// <param name="username">Login username. Eg: septian</param>
        /// <param name="password">Login password. Eg: Pw@12345</param>
        public sealed override void InitDb(string server, string database, string username, string password)
        {
            con.ConnectionString = "Data Source= " + server + "; Initial Catalog=" + database + ";user=" + username + "; Password=" + password;          
        }

        /// <summary>
        /// Initialize database connection
        /// </summary>
        /// <param name="server">Server name. Eg: SEPTIAN-WST\SQLSERVER</param>
        /// <param name="database">Database name. Eg: tableExample</param>
        public sealed override void InitDb(string server, string database)
        {
            con.ConnectionString = "Data Source= " + server + "; Initial Catalog=" + database + ";Integrated Security=True";
        }

        /// <summary>
        /// Execute non select query (eg: INSERT, UPDATE, DELETE)
        /// </summary>
        /// <param name="query">SQL Query</param>
        public override void TransactModify(string query)
        {
            try
            {
                using SqlCommand command = new SqlCommand(query, con);
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                throw new ApiException(e.Message);
            }
        }

        /// <summary>
        /// Execute select query
        /// </summary>
        /// <param name="query">SQL Query</param>
        /// <returns>List of row (List of column value in string representation)</returns>
        public sealed override List<List<string>> TransactSelect(string query)
        {
            List<List<string>> valueRead = new List<List<String>>();
            try
            {
                using (SqlCommand command = new SqlCommand(query, con))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        List<string> row = new List<string>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row.Add(reader[i].ToString());
                        }

                        valueRead.Add(row);
                    }
                }
                return valueRead;
            }
            catch (Exception e)
            {
                throw new ApiException(e.Message);
            }
        }

        /// <summary>
        /// Create Model object from raw List
        /// </summary>
        /// <typeparam name="T">Intherited Model class</typeparam>
        /// <param name="rawSqlData">List of row(List of column in string) </param>
        /// <returns>List of Model</returns>
        public override List<T> ObjectBuilder<T>(List<List<string>> rawSqlData)
        {
            Dictionary<string, int> selectedTable = new Dictionary<string, int>();
            Dictionary<string, Dictionary<string, int>> columnMapping = new Dictionary<string, Dictionary<string, int>>();

            int initialIndex = -1;
            GetFieldRcrv(typeof(T).Name, ref selectedTable, 0, ref columnMapping, ref initialIndex);

            return objectBuilder_<T>(rawSqlData, selectedTable, columnMapping);
        }

        public override string QueryBuilderNonSelect<T>(T entity, Operation opr)
        {
            string tableName = typeof(T).Name; 
            string query;
            bool first = true;
            switch (opr)
            {
                case Operation.INSERT:
                    query = "INSERT INTO " + typeof(T).Name + " ";
                    string column = "";
                    string value = "";
                    column = "(";
                    value = ") VALUES (";
                   

                    foreach (KeyValuePair<string, Tuple<string, PropertyInfo>> field in DataFactory.GetClassInfo(tableName))
                    {
                        if (!first)
                        {
                            column += ", ";
                            value += ", '";
                        }
                        first = false;
                        column += field.Value.Item1;
                        value += GetPropertyValue(entity, field.Value.Item2) + "'";
                    }

                    query += column + value + ")";
                    break;

                case Operation.UPDATE:
                    query = "UPDATE " + typeof(T).Name + " SET ";

                    foreach (KeyValuePair<string, Tuple<string, PropertyInfo>> field in DataFactory.GetClassInfo(tableName))
                    {
                        if (!first)
                        {
                            query += ", ";
                        }
                        first = true;
                        query += ", " + field.Value.Item1 + " = '" + GetPropertyValue(entity, field.Value.Item2) + "'";
                    }
                    query += " WHERE id = '" + ((Model)entity).Id.ToString() + "'";
                    break;

                case Operation.DELETE:
                    query = "DELETE FROM " + typeof(T).Name + " WHERE id = '" + ((Model)entity).Id.ToString() + "'";
                    break;

                default:
                    throw new ApiException("Invalid operation");
            }
            return query;
        }

        public override string QueryBuilderSelect<T>(bool join)
        {
            throw new NotImplementedException();
        }

        public override string QueryBuilderSelect<T>()
        {
            throw new NotImplementedException();
        }

        //=========== PRIVATE HELPER ===========

        private void SetPropertyValue <T> (T source, PropertyInfo p, object value, Dictionary<String, Dictionary<int, T>> rawList)
        {
            if (p.PropertyType.IsSubclassOf(typeof(short)) || p.PropertyType.GetTypeInfo() == typeof(short))
            {
                p.SetValue(source, Convert.ToInt16(value));
            }
            else if (p.PropertyType.IsSubclassOf(typeof(int)) || p.PropertyType.GetTypeInfo() == typeof(int))
            {
                p.SetValue(source, Convert.ToInt32(value));
            }
            else if (p.PropertyType.IsSubclassOf(typeof(long)) || p.PropertyType.GetTypeInfo() == typeof(long))
            {
                p.SetValue(source, Convert.ToInt64(value));
            }
            else if (p.PropertyType.IsSubclassOf(typeof(string)) || p.PropertyType.GetTypeInfo() == typeof(string))
            {
                p.SetValue(source, value);
            }
            else if (p.PropertyType.IsSubclassOf(typeof(bool)) || p.PropertyType.GetTypeInfo() == typeof(bool))
            {
                if ((string)value == "1")
                {
                    p.SetValue(source, true);
                }
                else
                {
                    p.SetValue(source, false);
                }
            }
            else if(p.PropertyType.IsSubclassOf(typeof(Enum)))
            {
                p.SetValue(source, (T)Enum.Parse(typeof(T), value.ToString(), true));
            }

            else if (p.PropertyType.IsSubclassOf(typeof(DateTime)) || p.PropertyType.GetTypeInfo() == typeof(DateTime))
            {
                DateTime dt = DateTime.Parse((string)value);
                p.SetValue(source, dt);
            }
            else if (p.PropertyType.IsSubclassOf(typeof(Model)) || p.PropertyType.GetTypeInfo() == typeof(Model))
            {
                p.SetValue(source, (rawList[p.PropertyType.Name])[Convert.ToInt32(value)]);
            }
        }

        private string GetPropertyValue<T> (T source, PropertyInfo p)
        {
            object obj = p.GetValue(source, null);
            if (obj is null)
            {
                return "";
            }

            if (obj.GetType().IsSubclassOf(typeof(short)) || obj is short ||
                obj.GetType().IsSubclassOf(typeof(int)) || obj is int ||
                obj.GetType().IsSubclassOf(typeof(long)) || obj is long ||
                obj.GetType().IsSubclassOf(typeof(string)) || obj is string)
            {
                return obj.ToString();
            }

            if (obj.GetType().IsSubclassOf(typeof(bool)) || obj is bool)
            {
                if ((bool)obj == true)
                {
                    return "1";
                }
                return "0";
            }

            if (obj.GetType().IsSubclassOf(typeof(DateTime)) || obj is DateTime)
            {
                DateTime dt = (DateTime)obj;
                return dt.ToString("yyyy-MM-dd HH:mm:ss");
            }

            if (obj.GetType().IsSubclassOf(typeof(Enum)))
            {
                return ((int)obj).ToString();
            }

            if (obj.GetType().IsSubclassOf(typeof(Model)))
            {
                return ((Model)obj).Id.ToString();
            }

            throw new ApiException("Unable to cast " + obj.GetType().Name);
        }

        private List<T> objectBuilder_<T>(List<List<string>> rawSqlData, Dictionary<string, int> entityObjects, Dictionary<string, Dictionary<string, int>> columnMapping) where T : ModelBase
        {
            Dictionary<String, Dictionary<int, T>> rawList = new Dictionary<string, Dictionary<int, T>>();

            Dictionary<int, List<String>> rawSortEntity = new Dictionary<int, List<string>>();
            int maxLevel = 0;
            LinkedList<string> sortedList = new LinkedList<string>();
            foreach (KeyValuePair<string, int> sortEntity in entityObjects)
            {
                if (!rawSortEntity.ContainsKey(sortEntity.Value))
                {
                    rawSortEntity.Add(sortEntity.Value, new List<string>());
                    maxLevel = Math.Max(maxLevel, sortEntity.Value);
                }
                rawSortEntity[sortEntity.Value].Add(sortEntity.Key);
            }

            for (int i = maxLevel; i >= 0; i--)
            {
                foreach (string str in rawSortEntity[i])
                {
                    sortedList.AddLast(str);
                }
            }

            // Split per row
            foreach (List<String> row in rawSqlData)
            {
                //Split per table
                foreach (string subTable in sortedList)
                {
                    Dictionary<string, int> subTableInfo = columnMapping[subTable];
                    T entityData = (T)Activator.CreateInstance(DataFactory.GetDataType(subTable));
                    Dictionary<string, Tuple<string, PropertyInfo>> propertyData = DataFactory.GetClassInfo(subTable);

                    //Loop through column
                    foreach (KeyValuePair<string, int> columnArray in subTableInfo)
                    {
                        object dataCol = row[columnArray.Value];
                        SetPropertyValue(entityData, propertyData[columnArray.Key].Item2, dataCol, rawList);
                    }

                    if (!rawList.ContainsKey(subTable))
                    {
                        rawList.Add(subTable, new Dictionary<int, T>());
                    }

                    if (!rawList[subTable].ContainsKey(entityData.Id))
                    {
                        rawList[subTable].Add(entityData.Id, entityData);
                    }

                }
            }

            List<T> ret = new List<T>();
            foreach (KeyValuePair<int, T> vl in rawList[typeof(T).Name])
            {
                ret.Add((T)vl.Value);
            }
            return ret;
        }

        private void GetFieldRcrv(string t, ref Dictionary<string, int> selectedTable, int index, ref Dictionary<string, Dictionary<string, int>> columnMapping, ref int colIndex)
        {
            selectedTable.Add(t, index);
            Dictionary<string, Tuple<string, PropertyInfo>> fieldList = DataFactory.GetClassInfo(t);
            Dictionary<string, int> columnValMapping = new Dictionary<string, int>();
            foreach (KeyValuePair<string, Tuple<string, PropertyInfo>> column in fieldList)
            {
                columnValMapping.Add(column.Key, ++colIndex);
            }
            columnMapping.Add(t, columnValMapping);
        }
    }
}
