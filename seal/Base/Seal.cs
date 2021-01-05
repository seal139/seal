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

        public void Init<T>() where T : IModel
        {
            TableInfo tbl = new TableInfo();
            tbl.FieldName = typeof(T).Name;
            tbl.Name = typeof(T).Name;

            PropertyInfo[] property_infos = (typeof(T)).GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public);
            foreach (PropertyInfo info in property_infos)
            {
                object[] attrs = info.GetCustomAttributes(true);
                FieldInfo ci = new FieldInfo();
                ci.Name = info.Name;

                bool isField = false;
                foreach (object attr in attrs)
                {
                    Column columnName = attr as Column;
                    if (columnName != null)
                    {
                        ci.FieldName = columnName.ColumnName;
                        isField = true;
                    }
                }

                if (!isField)
                {
                    continue;
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

        public void Post<T>(T model) where T : IModel
        {
            queryBuffer.Add(Serializer.CompileQuery(model.Mode, typeof(T).Name, model.Unpack(), model.UniqueIdentifier));
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

        public void Post<T>(T[] model) where T : IModel
        {
            foreach (T obj in model)
            {
                queryBuffer.Add(Serializer.CompileQuery(obj.Mode, typeof(T).Name, obj.Unpack(), obj.UniqueIdentifier));
            }
        }

        public void Delete<T>(T model) where T : IModel
        {
           queryBuffer.Add(Serializer.CompileQuery(Operation.Delete, typeof(T).Name, model.Unpack(), model.UniqueIdentifier));
        }

        public void Delete<T>(T[] model) where T : IModel
        {
            foreach(T obj in model)
            {
                queryBuffer.Add(Serializer.CompileQuery(Operation.Delete,typeof(T).Name, obj.Unpack(), obj.UniqueIdentifier));
            }
        }

        public void Delete<T>(string whereClause) where T : IModel
        {
            queryBuffer.Add(Serializer.CompileQuery(Operation.Delete, typeof(T).Name, null, null) + " " +  whereClause);
        }

        public T[] FindList<T>(string whereClause) where T : IModel
        {
            throw new NotImplementedException();
        }

        public T Find<T>(string whereClause) where T : IModel
        {
            throw new NotImplementedException();
        }

        public T[] ListAll<T>() where T : IModel
        {
            throw new NotImplementedException();
        }

        //private void GetFieldRcrv(string t, ref Dictionary<string, int> selectedTable, int index, ref Dictionary<string, Dictionary<string, int>> columnMapping, ref int colIndex)
        //{
        //    selectedTable.Add(t, index);
        //    Dictionary<string, Tuple<string, PropertyInfo>> fieldList = DataFactory.GetClassInfo(t);
        //    Dictionary<string, int> columnValMapping = new Dictionary<string, int>();
        //    foreach (KeyValuePair<string, Tuple<string, PropertyInfo>> column in fieldList)
        //    {
        //        columnValMapping.Add(column.Key, ++colIndex);
        //    }
        //    columnMapping.Add(t, columnValMapping);
        //}
    }
}
