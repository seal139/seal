using seal.Base;
using seal.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace seal.Helper
{
    public static class GenericGetterSetter
    {
        public static Action<ModelBase, object> CreateSetter<T>(PropertyInfo propertyInfo) where T : IModel
        {
            ParameterExpression obj = Expression.Parameter(typeof(T), "instance");
            ParameterExpression value = Expression.Parameter(typeof(object), "value");

            BinaryExpression body = Expression.Assign(Expression.Property(obj, propertyInfo.Name), value);
            return Expression.Lambda<Action<ModelBase, object>>(body, obj, value).Compile();
        }

        public static Func<ModelBase, object> CreateGetter<T>(string name) where T : IModel
        {
            ParameterExpression instance = Expression.Parameter(typeof(T), "instance");

            var body = Expression.Property(instance, name);

            return Expression.Lambda<Func<ModelBase, object>>(body, instance).Compile();
        }
    }
}
