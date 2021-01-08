using seal.Interface;
using System;
using System.Reflection;

namespace seal.Helper
{
    /// <summary>
    /// Mapping between column field and property getter-setter
    /// </summary>
    public class FieldInfo : IMapping
    {
        /// <summary>
        /// Property name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Column name
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Reflection getter-setter
        /// </summary>
        public PropertyInfo MethodDelegates { set; get; }

        public Action<IModel, object> Setter { get; set; }
        public Func<IModel, object> Getter { set; get; }

        /// <summary>
        /// Flag field as foreign key
        /// </summary>
        public bool IsForeignKey { get; set; }

        /// <summary>
        /// Get foreign class type
        /// </summary>
        public Type ForeignClass { get; set; }


    }
}
