using seal.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace seal.Interface
{
    /// <summary>
    /// Define validation mechanism before submitting to database
    /// </summary>
    interface IValidation
    {
        /// <summary>
        /// [Method definition] Validate data before INSERT operation
        /// </summary>
        /// <param name="error">Error data</param>
        public void ValidateOnInsert(Error error);

        /// <summary>
        /// [Method definition] Validate data before DELETE operation
        /// </summary>
        /// <param name="error">Error data</param>
        public void ValidateOnDelete(Error error);

        /// <summary>
        /// [Method definition] Validate data before UPDATE operation
        /// </summary>
        /// <param name="error">Error data</param>
        public void ValidateOnUpdate(Error error);
    }
}
