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

        public static Action<IModel, object> CreateSetter<T>(PropertyInfo propertyInfo) where T : IModel
        {
            ParameterExpression obj = Expression.Parameter(typeof(T), "instance");
            ParameterExpression value = Expression.Parameter(typeof(object), "value");
            UnaryExpression body =Expression.Convert( Expression.Assign(Expression.Property(obj, propertyInfo.Name), Expression.Convert(value, propertyInfo.PropertyType)), typeof(IModel));

            var q =  Expression.Lambda<Action<T, Object>>(body, obj, value).Compile();
            return ConvertSetter<T>(q);

        }

        public static Func<IModel, object> CreateGetter<T>(string name) where T : IModel
        {
            ParameterExpression instance = Expression.Parameter(typeof(T), "instance");

            var body = Expression.Convert(Expression.Property(instance, name), typeof(IModel));

            var q = Expression.Lambda<Func<T, object>>(body, instance).Compile();
            return ConvertGetter<T>(q);
        }

        private static Action<IModel, object> ConvertSetter<T>(Action<T, object> myActionT)
        {
            if (myActionT == null) return null;
            else return new Action<IModel, object>((o, p) => myActionT((T)o, p));
        }

        private static Func<IModel, object> ConvertGetter<T>(Func<T, object> myActionT)
        {
            if (myActionT == null) return null;
            else return new Func<IModel, object>(o => myActionT((T)o));
        }
    }
}
