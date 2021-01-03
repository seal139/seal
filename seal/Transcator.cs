using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Sql;
using System.Data.SqlClient;


namespace seal
{
    public class Transactor
    {
        private SqlConnection con;
        private Transactor()
        {
            con = new SqlConnection();
        }

        private static Transactor sqlTransactor = new Transactor();

        public static string ConnectionStringBuilder(string server, string databaseName, string username, string password)
        {
            return "Data Source= " + server + "; Initial Catalog=" + databaseName + ";user=" + username + "; Password=" + password;
        }

        public static  string ConnectionStringBuilder(string server, string databaseName)
        {
            return "Data Source= " + server + "; Initial Catalog=" + databaseName + ";Integrated Security=True";
        }
        public static void setConnection(string connectionString)
        {
            sqlTransactor.con.ConnectionString = connectionString; // SEPTIAN-WST\SQLSERVER
        }

        static void setDatabase(string dbName)
        {
            sqlTransactor.con.ChangeDatabase(dbName); //AdventureWorks2019
        }

        public static List<List<string>> TransactSelect(string query)
        {
            List<List<string>> valueRead = new List<List<String>>();
            sqlTransactor.con.Open();
            try
            {
                SqlCommand command = new SqlCommand(query, sqlTransactor.con);
                command.ExecuteNonQuery();

                //Cek ada
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    List<string> row = new List<string>();
                    for(int i = 0; i < reader.FieldCount; i++)
                    {
                        row.Add(reader[i].ToString());
                    }

                    valueRead.Add(row);
                }

            }
            catch (Exception e)
            {
                
            }
            finally
            {
                sqlTransactor.con.Close();               
            }
            return valueRead;
        }
    }
}
