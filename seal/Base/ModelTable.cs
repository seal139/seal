using seal.Attributes;
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
    public class ModelTable : ModelBase
    {
        public override JoinMode RelationJoinMode { get { return JoinMode.Once; } }

        public static explicit operator ModelTable(int id)
        {
            object obj = new object();
            ModelTable val = (ModelTable)obj;
            val.Id = id;
            return val;
        }

        public static implicit operator int?(ModelTable model)
        {
            return model.Id;
        }

        internal void HasInitialized(int id)
        {
            Id = id;
            isInitialized = true;
        }

        //public ModelTable(int value) : base()
        //{
        //    Id = value;
        //}

        [Column("Id")]
        public int Id { get; set; }

        [Column("Created")]
        public DateTime Created { get; set; }

        [Column("LastModified")]
        public DateTime LastModified { get; set; }

        public override string UniqueIdentifier { get{ return "Id"; } }

        public override string UniqueIdentifierValue { get { return Id.ToString(); } }
    }
}
