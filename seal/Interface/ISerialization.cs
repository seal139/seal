using seal.Enumeration;
using System;
using System.Collections.Generic;
using System.Reflection;

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
        IList<object> Serialize<T>(T table) where T : IModel;

        /// <summary>
        /// [Method definition] Convert raw data to Model object
        /// </summary>
        /// <typeparam name="T">Model type</typeparam>
        /// <param name="raw">Raw data</param>
        /// <returns>Instance of Model</returns>
        T Deserialize<T>(IList<object> raw) where T : IModel, new();

        /// <summary>
        /// [Method definition] Create compiled expression for property getter
        /// </summary>
        /// <param name="property">Assembly property info</param>
        /// <returns></returns>
        Func<IModel, object> CreateGetter(PropertyInfo property);

        Func<IModel> CreateCtor<T>() where T : IModel;

        /// <summary>
        /// [Method definition] Create compiled expression for property setter
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public Action<IModel, object> CreateSetter(PropertyInfo property);

    }
}
