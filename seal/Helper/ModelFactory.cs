using seal.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace seal.Helper
{
    /// <summary>
    /// Hold reflection getter-setter for class object model
    /// </summary>
    public sealed class ModelFactory
    {      
        private Dictionary<string, TableInfo> tableMapping;    
        private ModelFactory()
        {
            tableMapping = new Dictionary<string, TableInfo>();
        }

        private static ModelFactory obj = null;

        /// <summary>
        /// Get instance of ModelFactory
        /// </summary>
        /// <returns></returns>
        public static ModelFactory GetInstance()
        {
            if (obj == null)
            {
                obj = new ModelFactory();
            }
            return obj;
        }

        /// <summary>
        /// Class model container
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public TableInfo this[string tableName]
        {
            set
            {
                tableMapping.Add(tableName, value);
            }
            get
            {
                return tableMapping[tableName];
            }
        }

        /// <summary>
        /// Delete all table info
        /// </summary>
        public void Clear()
        {
            tableMapping.Clear();
        }

        /// <summary>
        /// Delete table by name
        /// </summary>
        /// <param name="tableName"></param>
        public void DeleteEntry(string tableName)
        {
            tableMapping.Remove(tableName);
        }
    }
}
