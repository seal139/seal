using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace seal
{
    public class Transaction
    {
        public void Begin()
        {

        }

        public void Commit()
        {

        }

        protected enum Operation
        {
            INSERT,
            UPDATE,
            DELETE,
        }

        protected void transactNonSelect<T>(T data, Operation opType) where T: Model
        {
            string opr = "";
            string sttmnt = "";
            switch (opType)
            {
                case Operation.INSERT:
                    opr = "INSERT INTO ";
                    sttmnt = "(Created, LastModified";
                    foreach(string fieldName in listAllField(data))
                    {
                        sttmnt += ", " + fieldName;
                    }
                    sttmnt += ") VALUES (" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ", " +  DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ", ";
                    break;

                case Operation.UPDATE:
                    opr = "UPDATE ";
                    break;

                case Operation.DELETE:
                    opr = "DELETE FROM ";
                    break;
                default:
                    throw new ApiException("Invalid operation");
            }

            opr += data.GetType().Name + " ";


            //INSERT INTO table_name (column1, column2, column3, ...)
            //VALUES(value1, value2, value3, ...);

            //DELETE FROM table_name WHERE condition;

            //UPDATE table_name
            //SET column1 = value1, column2 = value2, ...
            //WHERE condition;
            
            

        }

        public static object GetPropertyValue(object source, string propertyName)
        {
            PropertyInfo property = source.GetType().GetProperty(propertyName);
            return property.GetValue(source, null);
        }

        public static string[] listAllField(object source)
        {
            List<string> fields = new List<string>();

            PropertyInfo[] property_infos = source.GetType().GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public);
            foreach (PropertyInfo info in property_infos)
            {
                string name = info.Name;
                string attributes = info.PropertyType.Attributes.ToString();
                if (info.CanRead)
                {
                    if (!info.PropertyType.IsArray)
                    {
                        if((info.Name == "Id") || (info.Name == "Created") || (info.Name == "LastModified"))
                        {
                            continue;
                        }
                        fields.Add(info.Name);
                    }
                    else
                    {
                        throw new ApiException("Field must not an array");
                    }
                }
            }

            return fields.ToArray();
        }


    }

    public abstract class DataAPI<T> where T : Model
    {
        public void CreateEntity<T>(T model)
        {
            //queryTranslator("CREATE");
        }

        public void ModifyEntity<T>(T model)
        {
            //queryTranslator("UPDATE");
        }

        public void DeleteEntity<T>(T model)
        {
            //queryTranslator("UPDATE");
        }


        public void Find<T>(params KeyValuePair<object, object>[] whereClause)
        {

        }      
    }
}
