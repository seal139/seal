using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using seal.Base;

namespace seal.Interface
{
    /// <summary>
    /// Define mechanism for converting object to raw form and vice versa
    /// </summary>
    interface ISerialization
    {
        /// <summary>
        /// [Method definition] Convert model to raw form
        /// </summary>
        /// <typeparam name="T">Model type</typeparam>
        /// <param name="table">Obect</param>
        /// <returns>Dictionary that represent Column name and it's value</returns>
        Dictionary<string, object> Serialize<T>(T table) where T : IModelConverter;

        /// <summary>
        /// [Method definition] Convert raw data to Model object
        /// </summary>
        /// <typeparam name="T">Model type</typeparam>
        /// <param name="raw">Raw data</param>
        /// <returns>Instance of Model</returns>
        T Deserialize<T>(Dictionary<string, object> raw) where T : IModelConverter, new();
    }
}
