using seal.Enumeration;
using seal.Helper;
using seal.Interface;
using seal.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace seal.IntfImpl
{
    class SQLQueryBuilder
    {
        private string query;

        // Insert / Update
        string AutoQuery(TableInfo tableInfo, string primaryField, IList<object> rawField, bool isInsert)
        {
            var columnMapping = tableInfo.GetColumnMappingIndex;

            bool first = true;
            query = "";

            if (isInsert)
            {
                query = "INSERT INTO " + tableInfo.Name + " ";
                string column = "(";
                string value = ") VALUES (";

                foreach (KeyValuePair<string, int> kv in columnMapping)
                {
                    if (rawField[kv.Value] != null)
                    {
                        if (!first)
                        {
                            column += ", ";
                            value += ", ";
                        }

                        first = false;
                        column += kv.Key;
                        value += "@" + kv.Key;
                    }
                }

                query += column + value + ")";
            }
            else
            {
                query = "UPDATE " + tableInfo.Name + " SET ";

                foreach (KeyValuePair<string, int> kv in columnMapping)
                {
                    if (rawField[kv.Value] != null)
                    {
                        // Skip if primaryField
                        if ((string) rawField[kv.Value] == primaryField)
                        {
                            continue;
                        }

                        if (!first)
                        {
                            query += ", ";
                        }
                        first = false;
                        query += kv.Key + " = " + "@" + kv.Value;
                    }
                }

                query += " WHERE " + primaryField + " = @" + primaryField;
            }

            return query;
        }

        // Delete
        string AutoQuery(TableInfo tableInfo, string primaryField, object primaryKeyValue)
        {
            query = "DELETE FROM " + tableInfo.Name + " WHERE " + primaryField + " = @" + primaryField;

            return query;
        }

        // Select
        string AutoQuery(TableInfo tableInfo, string primaryField)
        {
            query = "SELECT * FROM " + tableInfo.Name + " WHERE " + primaryField + " = @" + primaryField;

            return query;
        }

        // Select
        string AutoQuery(TableInfo tableInfo, string primaryField, string whereClause)
        {
            query = "SELECT * FROM " + tableInfo.Name + " " + whereClause;

            return query;
        }

        string BuildQuery(string table, string[] columns, string[] whereClauses)
        {
            query = "SELECT ";
            string comma = ", ";
            string and = " AND ";

            // Set columns
            for(int i = 0; i < columns.Length; i++)
            {
                if(i + 1 < columns.Length)
                {
                    comma = " ";
                }
                query += columns[i] + comma;
            }

            query += "WHERE ";

            // Set where
            for (int i = 0; i < whereClauses.Length; i++)
            {
                if (i + 1 < whereClauses.Length)
                {
                    and = "";
                }
                query += whereClauses[i] + " = " + "@" + whereClauses[i] + and;
            }

            return query;
        }
    }
}
