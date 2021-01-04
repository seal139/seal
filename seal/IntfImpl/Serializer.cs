using seal.Helper;
using seal.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace seal.IntfImpl
{
    public class Serializer : ISerialization
    {
        private static bool initialized = false;
        private static Serializer instance = null;
        public static Serializer GetInstance()
        {
            if (!initialized)
            {
                instance = new Serializer();
                initialized = true;
            }
            return instance;
        }

        /// <summary>
        /// Serialize class to Raw format
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <returns></returns>
        public Dictionary<string, object> Serialize<T>(T table) where T : IModelConverter
        {
            ModelFactory factory = ModelFactory.GetInstance();
            TableInfo tInfo = factory[typeof(T).Name];
            Dictionary<string, object> rawData = table.Unpack();
            Dictionary<string, object> convertedData = new Dictionary<string, object>();
            foreach (KeyValuePair<string, object> pair in rawData)
            {
                convertedData.Add(tInfo[pair.Key].FieldName, pair.Value);
            }
            return convertedData;
        }

        /// <summary>
        /// Deserialize raw format to class <br/>
        /// Please note that you cannot directly deserialize raw data from serialize method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="raw"></param>
        /// <returns></returns>
        public T Deserialize<T>(Dictionary<string, object> raw) where T : IModelConverter, new()
        {
            T obj = new T();
            obj.Pack(raw);
            return obj;
        }

    }
}
