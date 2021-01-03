using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace seal.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class Entity : System.Attribute
    {
        public string TableName { set; get; }
        public Entity(string tableName)
        {
            this.TableName = tableName;
        }
    }
}