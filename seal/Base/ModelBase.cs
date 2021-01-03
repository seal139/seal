using seal.Helper;
using seal.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FieldInfo = seal.Helper.FieldInfo;

namespace seal.Base
{
    public abstract class ModelBase : IModel, IModelConverter
    {
        public abstract bool Joined { get; }
        public bool Initialized { get; }
        public Operation Mode { get { return mode; } }

        protected internal bool isInitialized;
        protected internal Operation mode;

        public ModelBase()
        {
            isInitialized = false;
            mode = Operation.INSERT;
        }

        /// <summary>
        /// Fill data field using Reflection mechanism.
        /// Override this method with native code for better performance
        /// </summary>
        /// <param name="values">Mapping column name - value</param>
        public virtual void Pack(Dictionary<string, object> values)
        {
            TableInfo ti = GetTableInfo();

            foreach (KeyValuePair<string, object> keyValue in values)
            {
                FieldInfo field = ti[keyValue.Key];
                PropertyInfo p = field.MethodDelegates;
                p.SetValue(this, keyValue.Value);
            }

            mode = Operation.UPDATE;
            isInitialized = true;
        }

        /// <summary>
        /// Extract data field using Reflection mechanism.
        /// Override this method with native code for better performance
        /// </summary>
        /// <returns>Mapping column name - value</returns>
        public virtual Dictionary<string, object> Unpack()
        {
            TableInfo ti = GetTableInfo();

            Dictionary<string, object> ret = new Dictionary<string, object>();
            foreach(KeyValuePair<string, FieldInfo> f in ti)
            {
                object obj = f.Value.MethodDelegates.GetValue(this, null);
                ret.Add(f.Key, obj);
            }
            return ret;
        }

        protected internal TableInfo GetTableInfo()
        {
            ModelFactory mf = ModelFactory.GetInstance();
            TableInfo ti = mf[this.GetType().Name];
            return ti;
        }
    }
}
