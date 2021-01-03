using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace seal.Interface
{
    /// <summary>
    /// Define mechanism how database entity is represented in programming language
    /// </summary>
    public interface IMapping
    {
        /// <summary>
        /// Property identifier
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Database identifier
        /// </summary>
        public string FieldName { get; set; }
    }
}
