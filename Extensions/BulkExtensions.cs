using System.Reflection;
using System.Text;
using VQuery.Core;

namespace VQuery.Extensions
{
    public static class BulkExtensions
    {
        public static int InsertMultiple<T>(
            this MySQLConnection db,
            string tableName,
            List<T> items)
        {
            if (items == null || items.Count == 0)
                return 0;

            var props =
            ReflectionCache
                .GetPropertyMap(typeof(T))
                .Values
                .ToArray();

            StringBuilder sql =
                new StringBuilder();

            Dictionary<string, object> parameters =
                new();

            sql.Append(
                $"INSERT INTO `{tableName}` (");

            sql.Append(
                string.Join(
                    ",",
                    props.Select(
                        p => $"`{p.Name}`")));

            sql.Append(") VALUES ");

            for (int i = 0; i < items.Count; i++)
            {
                if (i > 0)
                    sql.Append(",");

                sql.Append("(");

                for (int j = 0; j < props.Length; j++)
                {
                    if (j > 0)
                        sql.Append(",");

                    string paramName =
                        $"@{props[j].Name}_{i}";

                    sql.Append(paramName);

                    parameters.Add(
                        paramName,
                        props[j].GetValue(items[i])
                        ?? DBNull.Value);
                }

                sql.Append(")");
            }

            return db.ExecuteNonQuery(
                sql.ToString(),
                parameters);
        }
    }
}