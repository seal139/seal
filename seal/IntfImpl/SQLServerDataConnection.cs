using seal.Helper;
using seal.Interface;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace seal.IntfImpl
{
    /// <summary>
    /// Provide basic CRUD operation for SQL Server
    /// </summary>
    public class SQLServerDataConnection : IData, IDisposable
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
        private bool disposedValue;

        /// <summary>
        /// Database name
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// Connection string URL
        /// </summary>
        public string Connection { get; set; }

        /// <summary>
        ///  Construct SQL Server Connection object
        /// </summary>
        /// <param name="connectionString">Connection string URL</param>
        public SQLServerDataConnection(string connectionString)
        {
            Connection = connectionString;
            con = new SqlConnection(connectionString);
            com = new SqlCommand
            {
                Connection = con
            };
        }

        /// <summary>
        /// Construct SQL Server Connection object with no parameter
        /// </summary>
        public SQLServerDataConnection()
        {
            con = new SqlConnection();
            com = new SqlCommand
            {
                Connection = con
            };
        }

        /// <summary>
        /// Close connection
        /// </summary>
        public void Close()
        {
            con.Close();
        }

        /// <summary>
        /// Open connection
        /// </summary>
        public void Open()
        {
            con.ConnectionString = Connection;
            con.Open();
        }

        /// <summary>
        /// Transact SQL query for SELECT operation
        /// </summary>
        /// <param name="query">Query string</param>
        /// <returns>List of row(List of value for each column)</returns>
        public IList<IList<object>> TransactGet(string query)
        {
            try
            {
                IList<IList<object>> valueRead = new List<IList<object>>();
                com.CommandText = query;

                SqlDataReader reader = com.ExecuteReader();
                while (reader.Read())
                {
                    IList<object> row = new List<object>();
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
        public bool TransactPost(string query)
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
        protected virtual void Dispose(bool disposing)
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

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~SQLServerDataConnection()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
