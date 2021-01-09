namespace seal.Interface
{
    /// <summary>
    /// Define mechanism for ORM Core
    /// </summary>
    public interface IApi
    {
        void Post<T>(T model) where T : IModel;
        void Post<T>(T[] model) where T : IModel;

        void Delete<T>(string whereClause) where T : IModel;
        void Delete<T>(T model) where T : IModel;
        void Delete<T>(T[] model) where T : IModel;


        T[] FindList<T>(string whereClause) where T : IModel;

        T Find<T>(string whereClause) where T : IModel;
        T[] ListAll<T>() where T : IModel;
        void Init<T>() where T : IModel;
        void Sync();
        ISerialization Serializer { set; get; }
        IData DbDriver { set; get; }
    }
}
