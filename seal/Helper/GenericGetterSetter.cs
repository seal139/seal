namespace seal.Helper
{
    public static class GenericGetterSetter
    {

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TArg">Input</typeparam>
        /// <typeparam name="T">Output</typeparam>
        /// <returns></returns>
        //public static Func<TArg, IModel> CreateCtor<TArg, T>() where T : IModel
        //{
        //    var constructor = typeof(T).GetConstructor(new Type[] { typeof(TArg) });
        //    var parameter = Expression.Parameter(typeof(TArg), "p");
        //    var creatorExpression = Expression.Lambda<Func<TArg, T>>(
        //        Expression.New(constructor, new Expression[] { parameter }), parameter);
        //    return creatorExpression.Compile();
        //}


    }
}
