using seal.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace seal.Interface
{
    /// <summary>
    /// Define mechanism for ORM Core
    /// </summary>
    public interface IApi
    {
        /// <summary>
        /// [Method definition] INSERT or UPDATE operation
        /// </summary>
        /// <typeparam name="T">Model type</typeparam>
        /// <param name="model">Data object to INSERT or UPDATE</param>
        void Post<T>(T model) where T : ModelTable;

        /// <summary>
        /// [Method definition] DELETE operation
        /// </summary>
        /// <typeparam name="T">Model type</typeparam>
        /// <param name="whereClause">Query string</param>
        void Delete<T>(string whereClause) where T : ModelTable;


        /// <summary>
        /// [Method definition] Synchronize operation to database
        /// </summary>
        void Sync();

        /// <summary>
        /// [Method definition] SELECT operation
        /// </summary>
        /// <typeparam name="T">Model type</typeparam>
        /// <param name="whereClause">Query string</param>
        /// <returns></returns>
        T Get<T>(string whereClause) where T : ModelBase;

        /// <summary>
        /// [Method definition] SELECT operation
        /// </summary>
        /// <typeparam name="T">Model type</typeparam>
        /// <returns></returns>
        T Get<T>() where T : ModelBase;

        void Init<T>() where T : ModelBase;
    }
}
