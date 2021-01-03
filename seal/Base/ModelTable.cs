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
    public abstract class ModelTable : ModelBase, IValidation
    {

        public static implicit operator ModelTable(int id)
        {
            object obj = new object();
            ModelTable val = (ModelTable)obj;
            val.Id = id;
            return val;
        }

        public static explicit operator int(ModelTable model)
        {
            return model.Id;
        }

        internal void HasInitialized(int id)
        {
            Id = id;
            isInitialized = true;
        }

        /// <summary>
        /// Set foreign field data. Override this method with native code for better performance
        /// </summary>
        /// <param name="field">Field name</param>
        /// <param name="value">Object value</param>
        internal void FillJoin(string field, object value)
        {
            TableInfo ti = GetTableInfo();
            FieldInfo fInfo = ti[field];
            if(!fInfo.IsForeignKey)
            {
                throw new ApiException("Field is not foreign key");
            }
            PropertyInfo p = fInfo.MethodDelegates;         
            p.SetValue(this, value);
            isJoined = true;
        }

        private bool isJoined;
        public override bool Joined { get { return isJoined; } }
   

        public abstract void ValidateOnDelete(Error error);
        public abstract void ValidateOnInsert(Error error);
        public abstract void ValidateOnUpdate(Error error);

        public int Id { get; internal set; }
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
    }
}
