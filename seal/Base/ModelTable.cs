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

        public abstract void ValidateOnDelete(Error error);
        public abstract void ValidateOnInsert(Error error);
        public abstract void ValidateOnUpdate(Error error);

        public int Id { get; internal set; }
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
    }
}
