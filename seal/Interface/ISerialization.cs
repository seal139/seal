using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using seal.Base;
using seal.Enumeration;

namespace seal.Interface
{
    /// <summary>
    /// Define mechanism for converting object to raw form and vice versa
    /// </summary>
    public interface ISerialization
    {
        /// <summary>
        /// [Method definition] Convert model to raw form
        /// </summary>
        /// <typeparam name="T">Model type</typeparam>
        /// <param name="table">Obect</param>
        /// <returns>Dictionary that represent Column name and it's value</returns>
        IDictionary<string, object> Serialize<T>(T table) where T : IModel;

        /// <summary>
        /// [Method definition] Convert raw data to Model object
        /// </summary>
        /// <typeparam name="T">Model type</typeparam>
        /// <param name="raw">Raw data</param>
        /// <returns>Instance of Model</returns>
        T Deserialize<T>(IDictionary<string, object> raw) where T : IModel, new();

        /// <summary>
        /// Create CRUD query
        /// </summary>
        /// <param name="operation">CRUD Operation</param>
        /// <param name="raw">Raw data</param>
        /// <param name="uniqueIdentifierField">Primary field</param>
        /// <returns></returns>
        string CompileQuery(Operation operation, IDictionary<string, object> raw, string uniqueIdentifierField);
    }
}
