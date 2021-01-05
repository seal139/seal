using seal.Enumeration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace seal.Interface
{
    /// <summary>
    /// Define mechanism for database transaction
    /// </summary>
    public interface IData
    {
        /// <summary>
        /// [Method definition] INSERT, UPDATE, or DELETE
        /// </summary>
        /// <param name="query">Query string</param>
        /// <returns>True when code successfuly executed</returns>
        bool TransactPost(string query);

        /// <summary>
        /// [Method definition] SELECT
        /// </summary>
        /// <param name="query">Query string</param>
        /// <returns>List of rows (List of value for each column)</returns>
        IList<IList<object>> TransactGet(string query);

        /// <summary>
        /// [Method definition] Set database
        /// </summary>
        string Database { set; get; }

        /// <summary>
        /// [Method definition] Set connection string (Server)
        /// </summary>
        string Connection { set; get; }

        /// <summary>
        /// [Method definition] Open data transaction
        /// </summary>
        void Open();

        /// <summary>
        /// [Method definition] Close data transaction
        /// </summary>
        void Close();

     

    }
}
