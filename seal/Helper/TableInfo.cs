using seal.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace seal.Helper
{
    /// <summary>
    /// Hold mapping information between database and class model
    /// </summary>
    public class TableInfo : IMapping, IEnumerable<KeyValuePair<string, FieldInfo>>
    {

        private Dictionary<string, FieldInfo> fields;

        /// <summary>
        /// Create instance of TableInfo
        /// </summary>
        public TableInfo()
        {
            fields = new Dictionary<string, FieldInfo>();
        }


        public Func<int, IModel> Ctor { get; set; }

        /// <summary>
        /// Class name that represent Table
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Actual table name in database
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Fields
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <returns>Field Info</returns>
        public FieldInfo this[string propertyName]
        {
            get { return fields[propertyName]; }
            set { fields.Add(propertyName, value); }
        }

        public IEnumerator<KeyValuePair<string, FieldInfo>> GetEnumerator()
        {
            return fields.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
