using seal.Enumeration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace seal.Interface
{
    /// <summary>
    /// 
    /// </summary>
    public interface IModel
    {
        /// <summary>
        /// [Method definition] Get information about data are already joined (Joined object are initialized)
        /// </summary>
        bool Joined { get; }

        /// <summary>
        /// [Method definition] Get information about data are already initialized
        /// </summary>
        bool Initialized { get; }

        /// <summary>
        /// [Method definition] Current object state for POST operation
        /// </summary>
        Operation Mode { get; }

        /// <summary>
        /// [Method definition] Specify how field contain another Model object
        /// </summary>
        JoinMode RelationJoinMode { get; }

        /// <summary>
        /// [Method definition] Return unique column row identification <br/>
        /// This field usually return column ID
        /// </summary>
        string UniqueIdentifier { get; }

        /// <summary>
        /// [Method definition] Return unique column value for row identification <br/>
        /// This field usually return column ID
        /// </summary>
        string UniqueIdentifierValue { get; }

        /// <summary>
        /// [Method definition] Fill every field on Model from raw Dictionary
        /// </summary>
        /// <param name="values"></param>
        void Pack(IList<object> values);

        /// <summary>
        /// [Method definition] Convert value of every field on model to raw Dictionary
        /// </summary>
        /// <returns></returns>
        IList<object> Unpack();

        /// <summary>
        /// Link relation field to another model object
        /// </summary>
        /// <param name="field">Field name</param>
        /// <param name="value">Instance value</param>
        void SetJoinedObjValue(string field, object value);
    }
}
