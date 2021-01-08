using System;

namespace seal.Utils
{
    public static class DateTimeUtility
    {
        public static string GetSqlFormatDate(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }


    }
}
