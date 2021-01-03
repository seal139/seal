using seal.Base;
using seal.Enumeration;
using seal.Helper;
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

        public override string CompileQuery(Operation operation, Dictionary<string, object> raw)
        {
            string query = "";
            switch (operation)
            {

                case Operation.Insert:

                    foreach(KeyValuePair<string, object> keyVal in raw)
                    {

                    }

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
            }
        }
    }
}
