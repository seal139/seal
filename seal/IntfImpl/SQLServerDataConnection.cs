using seal.Base;
using seal.Enumeration;
using seal.Helper;
using seal.Utils;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace seal.IntfImpl
{
    /// <summary>
    /// Provide basic CRUD operation for SQL Server
    /// </summary>
    public class SQLServerDataConnection : Data
    {
        /// <summary>
        /// Create connection string
        /// </summary>
        /// <param name="server">Server name</param>
        /// <param name="databaseName">Database name</param>
        /// <param name="username">Username login info</param>
        /// <param name="password">Password login info</param>
        /// <returns>Connection string</returns>
        public static string ConnectionStringBuilder(string server, string databaseName, string username, string password)
        {
            return "Data Source= " + server + "; Initial Catalog=" + databaseName + ";user=" + username + "; Password=" + password;
        }

        /// <summary>
        /// Create connection string
        /// </summary>
        /// <param name="server">Server name</param>
        /// <param name="databaseName">Database name</param>
        /// <returns>Connection string</returns>
        public static string ConnectionStringBuilder(string server, string databaseName)
        {
            return "Data Source= " + server + "; Initial Catalog=" + databaseName + ";Integrated Security=True";
        }

        private SqlConnection con;
        private SqlCommand com;

        /// <summary>
        ///  Construct SQL Server Connection object
        /// </summary>
        /// <param name="connectionString">Connection string URL</param>
        public SQLServerDataConnection(string connectionString)
        {
            con = new SqlConnection(connectionString);
            com = new SqlCommand();
            com.Connection = con;
        }

        /// <summary>
        /// Construct SQL Server Connection object with no parameter
        /// </summary>
        public SQLServerDataConnection()
        {
            con = new SqlConnection();
            com = new SqlCommand();
            com.Connection = con;
        }

        /// <summary>
        /// Close connection
        /// </summary>
        public override void Close()
        {
            con.Close();
        }

        /// <summary>
        /// Open connection
        /// </summary>
        public override void Open()
        {
            con.Open();
        }

        /// <summary>
        /// Transact SQL query for SELECT operation
        /// </summary>
        /// <param name="query">Query string</param>
        /// <returns>List of row(List of value for each column)</returns>
        public override List<List<object>> TransactGet(string query)
        {       
            try
            {
                List<List<object>> valueRead = new List<List<object>>();
                com.CommandText = query;

                SqlDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    List<object> row = new List<object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row.Add(reader[i].ToString());
                    }

                    valueRead.Add(row);
                }

                return valueRead;
            }
            catch (Exception e)
            {
                throw new ApiException(e.Message);
            }
        }

        /// <summary>
        /// Transact SQL query for INSERT, UPDATE, or DELETE operation
        /// </summary>
        /// <param name="query">Query string</param>
        /// <returns>True when one or more row(s) affected</returns>
        public override bool TransactPost(string query)
        {
            com.CommandText = query;
            int rowAffected = com.ExecuteNonQuery();
            if (rowAffected > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Release all resources
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Database = null;
                    Connection = null;
                    con.Dispose();
                    com.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        private string ValueConverter(object value)
        {
            if (value.GetType().IsSubclassOf(typeof(DateTime)) || value is DateTime)
            {
                return "'" + DateTimeUtility.GetSqlFormatDate((DateTime)value) + "'";
            }

            if ((value.GetType().IsSubclassOf(typeof(long))   || value is long)   || 
                (value.GetType().IsSubclassOf(typeof(int))    || value is int)    || 
                (value.GetType().IsSubclassOf(typeof(short))  || value is short)  ||
                (value.GetType().IsSubclassOf(typeof(double)) || value is double) || 
                (value.GetType().IsSubclassOf(typeof(float))  || value is float))
            {
                return value.ToString();
            }

            if (value.GetType().IsSubclassOf(typeof(string)) || value is string)
            {
                return "'" +  value.ToString() + "'";
            }

            if (value.GetType().IsSubclassOf(typeof(bool)) || value is bool)
            {
                if((bool) value == true)
                {
                    return "1";
                }
                return "0";
            }

            if  (value.GetType().IsSubclassOf(typeof(ModelTable)) || value is ModelTable)
            {
                return ((ModelTable)value).Id.ToString();
            }

            if (value.GetType().IsSubclassOf(typeof(Enum)))
            {
                return ((int)value).ToString();
            }

            throw new ApiException("Invalid value for invoking to database");
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

                    foreach(KeyValuePair<string, object> keyVal in raw)
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
    }
}
