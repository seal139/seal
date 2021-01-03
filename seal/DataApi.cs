using seal.Attribtues;
using seal.Enumeration;
using seal.Interface;
using seal.IntfImpl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace seal
{
   

    public enum Sort
    {
        ASCENDING,
        DESCENDING
    }

    public class DaoMaster<T> where T : Model, new()
    {
        private string tableName;
        public DaoMaster()
        {
            tableName = typeof(T).Name;
            DataFactory.AddDataType(typeof(T).Name, typeof(T));

            Dictionary<string, Tuple<string, PropertyInfo>> fieldList = new Dictionary<string, Tuple<string, PropertyInfo>>();
            Dictionary<string, KeyValuePair<string, bool>> isAnotherEntity = new Dictionary<string, KeyValuePair<string, bool>>();

            PropertyInfo[]  property_infos = (typeof(T)).GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public);
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
                    if(info.CanRead && info.CanWrite)
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

        List<T> objectBuilder(List<List<string>> rawSqlData, Dictionary<string, int> entityObjects, Dictionary<string, Dictionary<string, int>> columnMapping)
        {
            Dictionary<String, Dictionary<int, Model>> rawList = new Dictionary<string, Dictionary<int, Model>>();

            Dictionary<int, List<String>> rawSortEntity = new Dictionary<int, List<string>>();
            int maxLevel = 0;
            LinkedList<string> sortedList = new LinkedList<string>();
            foreach(KeyValuePair<string, int> sortEntity in entityObjects)
            {
                if (!rawSortEntity.ContainsKey(sortEntity.Value))
                {
                    rawSortEntity.Add(sortEntity.Value, new List<string>());
                    maxLevel = Math.Max(maxLevel, sortEntity.Value);
                }
                rawSortEntity[sortEntity.Value].Add(sortEntity.Key);
            }

            for(int i = maxLevel; i>= 0; i--)
            {
                foreach(string str in rawSortEntity[i])
                {
                    sortedList.AddLast(str);
                }
            }

            // Split per row
            foreach(List<String> row in rawSqlData)
            {
                //Split per table
                foreach (string subTable in sortedList)
                {
                    Dictionary<string, int>  subTableInfo = columnMapping[subTable];
                    Model entityData = (Model)Activator.CreateInstance(DataFactory.GetDataType(subTable));
                    Dictionary<string, Tuple<string, PropertyInfo>> propertyData =  DataFactory.GetClassInfo(subTable);

                    //Loop through column
                    foreach(KeyValuePair<string, int> columnArray in subTableInfo)
                    {
                        object dataCol = row[columnArray.Value];
                        SetPropertyValue(entityData, propertyData[columnArray.Key].Item2, dataCol, rawList);
                    }

                    if (!rawList.ContainsKey(subTable))
                    {
                        rawList.Add(subTable, new Dictionary<int, Model>());
                    }

                    if (!rawList[subTable].ContainsKey(entityData.Id))
                    {
                        rawList[subTable].Add(entityData.Id, entityData);
                    }
                      
                }
            }

            List<T> ret = new List<T>();
            foreach(KeyValuePair<int, Model> vl in rawList[typeof(T).Name])
            {
                ret.Add((T)vl.Value);
            }
            return ret;
        }

        public List<T> QueryBuilderSelect()
        {
            Dictionary<string, int> selectedTable = new Dictionary<string, int>();
            Dictionary<string, Dictionary<string, int>> columnMapping = new Dictionary<string, Dictionary<string, int>>();
            string join = "";
            int initialIndex = -1;
            string sel = GetFieldRcrv(typeof(T).Name, ref join, ref selectedTable, true, 0, ref columnMapping, ref initialIndex);
            string frm = "FROM " + DataFactory.GetAlias(typeof(T).Name);
           
            var qq = seal.Transactor.TransactSelect(sel + " " + frm + " " + join);
            return objectBuilder(qq, selectedTable, columnMapping);

        }

        private string GetFieldRcrv(string t, ref string join, ref Dictionary<string, int> selectedTable, bool first, int index, ref Dictionary<string, Dictionary<string, int>> columnMapping, ref int colIndex)
        {
            String query ="";
            bool isFirst = first;
            selectedTable.Add(t, index);
            Dictionary<string, Tuple<string, PropertyInfo>> fieldList = DataFactory.GetClassInfo(t);
            Dictionary<string, KeyValuePair<string, bool>> relationList = DataFactory.GetRelationInfo(t);

            Dictionary<string, int> columnValMapping = new Dictionary<string, int>();
            foreach (KeyValuePair< string, Tuple<string, PropertyInfo>> column in fieldList)
            {
                if (!isFirst)
                {
                    query += ", ";
                }
                else
                {
                    query = "SELECT ";
                }
                query += DataFactory.GetAlias(t) + "." + column.Value.Item1 + "\n\r";
                columnValMapping.Add(column.Key, ++colIndex);

                bool relation = relationList[column.Key].Value;
                if (relation && !selectedTable.ContainsKey(relationList[column.Key].Key))
                {
                    string joinTableNameAlias = DataFactory.GetAlias(relationList[column.Key].Key);
                    string joinTablePK = DataFactory.GetClassInfo(relationList[column.Key].Key)["Id"].Item1;
                    join += "JOIN " + DataFactory.GetAlias(relationList[column.Key].Key) + " ON " + joinTableNameAlias + "." + joinTablePK + " = " + DataFactory.GetAlias(t) + "." + column.Value.Item1 + " ";
                    query += GetFieldRcrv(relationList[column.Key].Key, ref join, ref selectedTable, isFirst, index + 1, ref columnMapping, ref colIndex);
                }
                isFirst = false;
            }
            columnMapping.Add(t, columnValMapping);
            return query;
        }

        public string QueryBuilder (Enumeration.Operation operation, T entity)
        {

            string query;
            switch (operation)
            {
                case Enumeration.Operation.INSERT:
                    query = "INSERT INTO " + typeof(T).Name + " ";
                    string column = "";
                    string value = "";
                    column = "(Created, LastModified";
                    value = ") VALUES ('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";

                    foreach (KeyValuePair<string, Tuple<string, PropertyInfo>> field in DataFactory.GetClassInfo(tableName))
                    {
                        column += ", " + field.Value.Item1;
                        value += ", '" + GetPropertyValue(entity, field.Value.Item2) + "'";
                    }

                    query += column + value + ")";
                    break;

                case Enumeration.Operation.UPDATE:
                    query = "UPDATE " + typeof(T).Name + " SET LastModified = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                    foreach (KeyValuePair<string, Tuple<string, PropertyInfo>> field in DataFactory.GetClassInfo(tableName))
                    {
                        query += ", " + field.Value.Item1 + " = '" + GetPropertyValue(entity, field.Value.Item2) + "'";
                    }
                    query += " WHERE id = '" + ((Model)entity).Id.ToString() + "'";
                    break;

                case Enumeration.Operation.DELETE:
                    query = "DELETE FROM " + typeof(T).Name + " WHERE id = '" + ((Model)entity).Id.ToString() + "'";
                    break;

              
                default:
                    throw new ApiException("Invalid operation");
            }
            return query;
        }

        private void SetPropertyValue(Model source, PropertyInfo p, object value, Dictionary<String, Dictionary<int, Model>> rawList)
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

        private string GetPropertyValue(T source, PropertyInfo p)
        {
            object obj = p.GetValue(source, null);
            if (obj is null)
            {
                return "";
            }

            if (obj.GetType().IsSubclassOf(typeof(short))   || obj is short     ||
                obj.GetType().IsSubclassOf(typeof(int))     || obj is int       ||
                obj.GetType().IsSubclassOf(typeof(long))    || obj is long      ||
                obj.GetType().IsSubclassOf(typeof(string))  || obj is string)
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
    }
}
