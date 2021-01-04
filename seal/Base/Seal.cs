using seal.Attributes;
using seal.Enumeration;
using seal.Helper;
using seal.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FieldInfo = seal.Helper.FieldInfo;

namespace seal.Base
{
    public class Seal : IApi
    {

        public static Seal GetInstance()
        {
            if (!initialized)
            {
                instance = new Seal();
                initialized = true;
            }
            return instance;
        }
        private static Seal instance = null;
        private static bool initialized = false;

        private Seal()
        {
            queryBuffer = new List<string>();
        }
        private IList<string> queryBuffer;
        

        public ISerialization Serializer { get; set; }
        public IData DbDriver { get; set; }

        public void Init<T>() where T : ModelBase
        {
            TableInfo tbl = new TableInfo();
            tbl.FieldName = typeof(T).Name;
            tbl.Name = typeof(T).Name;

            PropertyInfo[] property_infos = (typeof(T)).GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public);
            foreach (PropertyInfo info in property_infos)
            {
                object[] attrs = info.GetCustomAttributes(true);
                FieldInfo ci = new FieldInfo();
                ci.FieldName = info.Name;
                ci.Name = info.Name;

                foreach (object attr in attrs)
                {
                    Column columnName = attr as Column;
                    if (columnName != null)
                    {
                        ci.Name = columnName.ColumnName;
                    }
                }

                if (!info.PropertyType.IsArray)
                {
                    if (info.CanRead && info.CanWrite)
                    {
                        ci.MethodDelegates = info;
                        ci.Setter = GenericGetterSetter.CreateSetter<T>(info);
                        ci.Getter = GenericGetterSetter.CreateGetter<T>(info.Name);

                        if (info.PropertyType.IsSubclassOf(typeof(ModelTable)))
                        {
                            ci.IsForeignKey = true;
                            ci.ForeignClass = info.PropertyType;
                        }
                        else
                        {
                            ci.IsForeignKey = false;
                            ci.ForeignClass = null;
                        }
                    }
                }

                tbl[info.Name] = ci;
            }
        }

        public void Post<T>(T model) where T : ModelTable
        {
            queryBuffer.Add(DbDriver.CompileQuery(model.Mode, model.Unpack(), model.UniqueIdentifier));
        }

        public void Sync()
        {
            DbDriver.Open();
            foreach(string query in queryBuffer)
            {
                DbDriver.TransactPost(query);
            }
            DbDriver.Close();
        }

        public void Post<T>(T[] model) where T : ModelTable
        {
            foreach (T obj in model)
            {
                queryBuffer.Add(DbDriver.CompileQuery(obj.Mode, obj.Unpack(), obj.UniqueIdentifier));
            }
        }

        public void Delete<T>(T model) where T : ModelTable
        {
           queryBuffer.Add(DbDriver.CompileQuery(Operation.Delete, model.Unpack(), model.UniqueIdentifier));
        }

        public void Delete<T>(T[] model) where T : ModelTable
        {
            foreach(T obj in model)
            {
                queryBuffer.Add(DbDriver.CompileQuery(Operation.Delete, obj.Unpack(), obj.UniqueIdentifier));
            }
        }

        public void Delete<T>(string whereClause) where T : ModelTable
        {
            queryBuffer.Add(DbDriver.CompileQuery(Operation.Delete, null, null) + " " +  whereClause);
        }

        public T[] FindList<T>(string whereClause) where T : ModelBase
        {
            throw new NotImplementedException();
        }

        public T Find<T>(string whereClause) where T : ModelBase
        {
            throw new NotImplementedException();
        }

        public T[] ListAll<T>() where T : ModelBase
        {
            throw new NotImplementedException();
        }
    }
}
