using seal.Attributes;
using seal.Enumeration;
using System;

namespace seal.Base
{
    public class ModelSecure : ModelBase
    {
        public override void LazyInit(object value)
        {
            UUID = (string)value;
        }
        public override JoinMode RelationJoinMode => JoinMode.Once;

        //public ModelTable(int value) : base()
        //{
        //    Id = value;
        //}

        [Column("UUID")]
        public string UUID { get; set; }

        [Column("Created")]
        public DateTime Created { get; set; }

        [Column("LastModified")]
        public DateTime LastModified { get; set; }

        public override string UniqueIdentifier => "Id";

        public override string UniqueIdentifierValue => UUID;
    }
}
