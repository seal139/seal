using seal.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace seal.Interface
{
    /// <summary>
    /// Define mechanism for class conversion between Object and raw Dictionary
    /// </summary>
    public interface IModelConverter
    {
        /// <summary>
        /// [Method definition] Fill every field on Model from raw Dictionary
        /// </summary>
        /// <param name="values"></param>
        void Pack(Dictionary<string, object> values);

        /// <summary>
        /// [Method definition] Convert value of every field on model to raw Dictionary
        /// </summary>
        /// <returns></returns>
        Dictionary<string, object> Unpack();
    }
}
