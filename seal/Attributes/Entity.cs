namespace seal.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class Entity : System.Attribute
    {
        public string TableName { set; get; }
        public Entity(string tableName)
        {
            TableName = tableName;
        }
    }
}