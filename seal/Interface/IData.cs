using seal.Enumeration;
using seal.Helper;
using System.Collections.Generic;

namespace seal.Interface
{
    /// <summary>
    /// Define mechanism for database transaction
    /// </summary>
    public interface IData
    {   
        bool TransactPost(string query);
        IList<IList<object>> TransactGet(string query);

        string Database { set; get; }
        string Connection { set; get; }
        void Open();
        void Close();
    }
}
