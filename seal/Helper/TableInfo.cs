using seal.Interface;
using System;
using System.Collections;
using System.Collections.Generic;

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
            columnIndexMapping = new Dictionary<string, int>();
        }

        public Func<IModel> Constructor { set; get; }

        private IDictionary<string, int> columnIndexMapping;

        public IDictionary<string, int> GetColumnMappingIndex => columnIndexMapping;

        public void AddMap(string field, int index)
        {
            columnIndexMapping.Add(field, index);
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
            get => fields[propertyName];
            set => fields.Add(propertyName, value);
        }

        public IEnumerator<KeyValuePair<string, FieldInfo>> GetEnumerator()
        {
            return fields.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
