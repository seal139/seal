using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace seal
{
    /// <summary>
    /// Mapping class object to database entity info
    /// </summary>
    internal class DataFactory
    {
        private Dictionary<string, Dictionary<string, Tuple<string, PropertyInfo>>> MappingObj;
        private Dictionary<string, Dictionary<string, KeyValuePair<string, bool>>> MappingRelation;
        private Dictionary<string, string> Alias;
        private Dictionary<string, Type> TypeMap;
        
        private DataFactory()
        {
            MappingObj = new Dictionary<string, Dictionary<string, Tuple<string, PropertyInfo>>>();
            MappingRelation = new Dictionary<string, Dictionary<string, KeyValuePair<string, bool>>>();
            Alias = new Dictionary<string, string>();
            TypeMap = new Dictionary<string, Type>();
        }

        private static DataFactory instance = new DataFactory();
  
        internal static void AddMap(string tableName, Dictionary<string, Tuple<string, PropertyInfo>> classInfo, Dictionary<string, KeyValuePair<string, bool>> relationInfo)
        {
            instance.MappingObj.Add(tableName, classInfo);
            instance.MappingRelation.Add(tableName, relationInfo);
        }

        internal static Dictionary<string, Tuple<string, PropertyInfo>> GetClassInfo(string tableName)
        {
            return instance.MappingObj[tableName];
        }

        internal static Dictionary<string, KeyValuePair<string, bool>> GetRelationInfo(string tableName)
        {
            return instance.MappingRelation[tableName];
        }

        internal static void AddAlias(string className, string alias)
        {
            instance.Alias.Add(className, alias);
        }

        internal static string GetAlias(string className)
        {
            return instance.Alias[className];
        }

        internal static void AddDataType(string className, Type t)
        {
            instance.TypeMap.Add(className, t);
        }

        internal static Type GetDataType(string className)
        {
            return instance.TypeMap[className];
        }
    }
}
