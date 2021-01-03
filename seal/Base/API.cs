using seal.Attributes;
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
    public class API : IApi, ISerialization
    {
        public static API GetInstance()
        {

        }

        private List<string> queryBuffer;

        public void Delete<T>(string whereClause) where T : ModelTable
        {
            throw new NotImplementedException();
        }

        public T Deserialize<T>(Dictionary<string, object> raw) where T : ModelBase, new()
        {
            T model = new T();
            model.Pack(raw);
            return model;
        }

        public T Get<T>(string whereClause) where T : ModelBase
        {
            throw new NotImplementedException();
        }

        public T Get<T>() where T : ModelBase
        {
            throw new NotImplementedException();
        }

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
           
        }

        public Dictionary<string, object> Serialize<T>(T table) where T : ModelTable
        {
            return table.Unpack();
        }

        public void Sync()
        {
            throw new NotImplementedException();
        }
    }
}
