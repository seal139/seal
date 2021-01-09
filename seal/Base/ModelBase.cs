using seal.Enumeration;
using seal.Helper;
using seal.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using FieldInfo = seal.Helper.FieldInfo;

namespace seal.Base
{
    public abstract class ModelBase : IModel
    {
        public bool Joined => join;
        public bool Initialized => isInitialized;
        public Operation Mode => mode;
        public abstract JoinMode RelationJoinMode { get; }

        public abstract string UniqueIdentifier { get; }
        public abstract string UniqueIdentifierValue { get; }

        protected internal bool isInitialized;
        protected internal Operation mode;
        protected internal bool join;

        public ModelBase()
        {
            isInitialized = false;
            ti = GetTableInfo();
            mode = Operation.Insert;
        }

        /// <summary>
        /// Extract data field using Reflection mechanism.
        /// Override this method with native code for better performance
        /// </summary>
        /// <returns>Mapping column name - value</returns>
        public virtual IList<object> Unpack()
        {
            IList<object> ret = new List<object>();
            foreach (KeyValuePair<string, FieldInfo> f in ti)
            {
                ret.Add(f.Value.Getter(this));
            }
            return ret;
        }

        /// <summary>
        /// Fill data field using Reflection mechanism.
        /// Override this method with native code for better performance
        /// </summary>
        /// <param name="values">Mapping column name - value</param>
        public virtual void Pack(IList<object> values)
        {
           

            foreach (KeyValuePair<string, FieldInfo> f in ti)
            {

                //KeyValuePair<string, int> indexMap in ti.GetColumnMappingIndex
                //int index = ti.GetColumnMappingIndex[f.Value.Name];
              
                Action<IModel, object> setter = f.Value.Setter;
                int idx = ti.GetColumnMappingIndex[f.Key];

                if (f.Value.IsForeignKey)
                {
                    ModelFactory mf = ModelFactory.GetInstance();
                    TableInfo tblInfo = mf[f.Value.MethodDelegates.PropertyType.Name];
                    Func<IModel> constr = tblInfo.Constructor;
                    IModel q = constr();

                    q.LazyInit(values[idx++]);
                    setter(this, q);
                }
                else
                {
                    setter(this, values[idx]);
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
        public virtual void SetJoinedObjValue(string fieldName, IModel obj)
        {
            TableInfo ti = GetTableInfo();
            FieldInfo field = ti[fieldName];

            Action<IModel, object> setter = field.Setter;
            setter(this, obj);

            join = true;
        }

        public abstract void LazyInit(object value);

        TableInfo ti;
        protected internal TableInfo GetTableInfo()
        {
            ModelFactory mf = ModelFactory.GetInstance();
            TableInfo ti = mf[GetType().Name];
            return ti;
        }
    }
}
