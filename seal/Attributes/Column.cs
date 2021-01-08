namespace seal.Attributes
{
    [System.AttributeUsage(System.AttributeTargets.Property)]
    public class Column : System.Attribute
    {
        public string ColumnName { set; get; }
        public Column(string columnName)
        {
            ColumnName = columnName;
        }
    }
}
