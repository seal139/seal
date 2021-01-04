using seal.Enumeration;
using seal.Helper;
using seal.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace seal.Base
{
    /// <summary>
    /// Provide basic CRUD operation
    /// </summary>
    public abstract class Data : IData, IDisposable
    {
        protected bool disposedValue;

        /// <summary>
        /// Database name
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// Connection URL
        /// </summary>
        public string Connection { get; set; }

        /// <summary>
        /// Close connection to database
        /// </summary>
        public abstract void Close();

        /// <summary>
        /// Open connection to database
        /// </summary>
        public abstract void Open();

        /// <summary>
        /// Execute SELECT statement
        /// </summary>
        /// <param name="query">SELECT query</param>
        /// <returns>List of row (List of value for each column)</returns>
        public abstract List<List<object>> TransactGet(string query);

        /// <summary>
        /// Execute non SELECT statment (eg: INSERT, UPDATE, DELETE)
        /// </summary>
        /// <param name="query">Query</param>
        /// <returns>True if operation is success otherwise false</returns>
        public abstract bool TransactPost(string query);

        /// <summary>
        /// Create query from raw data format
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="raw"></param>
        /// <param name="uniqueIdentifierField"></param>
        /// <returns></returns>
        public abstract string CompileQuery(Operation operation, Dictionary<string, object> raw, string uniqueIdentifierField);

        protected abstract void Dispose(bool disposing);

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Data()
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
