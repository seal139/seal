using seal.Enumeration;
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
    public abstract class ModelBase : IModel
    {
        public bool Joined { get { return join; } }
        public bool Initialized { get { return isInitialized; } }
        public Operation Mode { get { return mode; } }
        public abstract JoinMode RelationJoinMode { get; }

        public abstract string UniqueIdentifier{get;}
        public abstract string UniqueIdentifierValue { get; }

        protected internal bool isInitialized;
        protected internal Operation mode;
        protected internal bool join;

        public ModelBase()
        {
            isInitialized = false;
            mode = Operation.Insert;
        }

        //public ModelBase(int value)
        //{
        //    isInitialized = false;
        //    mode = Operation.Update;
        //}

        /// <summary>
        /// Extract data field using Reflection mechanism.
        /// Override this method with native code for better performance
        /// </summary>
        /// <returns>Mapping column name - value</returns>
        public virtual IDictionary<string, object> Unpack()
        {
            TableInfo ti = GetTableInfo();

            Dictionary<string, object> ret = new Dictionary<string, object>();
            foreach (KeyValuePair<string, FieldInfo> f in ti)
            {
                Func<IModel, object> getter = f.Value.Getter;
                ret.Add(f.Key, getter(this));

                //object obj = f.Value.MethodDelegates.GetValue(this, null);
                //ret.Add(f.Key, obj);
            }
            return ret;
        }

        /// <summary>
        /// Fill data field using Reflection mechanism.
        /// Override this method with native code for better performance
        /// </summary>
        /// <param name="values">Mapping column name - value</param>
        public virtual void Pack(IDictionary<string, object> values)
        {
            TableInfo ti = GetTableInfo();

            foreach (KeyValuePair<string, object> keyValue in values)
            {
                FieldInfo field = ti[keyValue.Key];
                PropertyInfo p = field.MethodDelegates;

                Action<IModel, object> setter =  field.Setter;
              
                if (p.PropertyType.GetInterfaces().Contains(typeof(IModel)))
                {
                    ModelBase q = (ModelBase)Activator.CreateInstance(p.PropertyType);
                    // q.InvokeNew(keyValue.Value);

                    setter(this, q);
                    //p.SetValue(this, q);
                }
                else
                {
                    setter(this, keyValue.Value);
                }
               
            }

            mode = Operation.Update;
            isInitialized = true;
        }

        /// <summary>
        /// File joined table instance to field that marked foreign key
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="value"></param>
        public virtual void SetJoinedObjValue(string fieldName, object value)
        {
            join = true;
            TableInfo ti = GetTableInfo();
            FieldInfo field = ti[fieldName];
            PropertyInfo p = field.MethodDelegates;
            p.SetValue(this, value);
        }

        

        protected internal TableInfo GetTableInfo()
        {
            ModelFactory mf = ModelFactory.GetInstance();
            TableInfo ti = mf[this.GetType().Name];
            return ti;
        }
    }
}
