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
    }
}
