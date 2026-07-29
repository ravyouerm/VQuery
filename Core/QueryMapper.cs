using System;
using System.Data;

namespace VQuery.Core
{
    internal static class QueryMapper
    {
        public static T MapRow<T>(
            DataRow row)
            where T : new()
        {
            T item = new();

            var propertyMap =
                ReflectionCache.GetPropertyMap(
                    typeof(T));

            foreach (DataColumn column in row.Table.Columns)
            {
                if (!propertyMap.TryGetValue(
                    column.ColumnName,
                    out var prop))
                {
                    continue;
                }

                object value =
                    row[column];

                if (value == DBNull.Value)
                    continue;

                try
                {
                    Type targetType =
                        Nullable.GetUnderlyingType(
                            prop.PropertyType)
                        ?? prop.PropertyType;

                    object? convertedValue;

                    if (targetType == typeof(Guid))
                    {
                        convertedValue =
                            Guid.Parse(
                                value.ToString()!);
                    }
                    else if (targetType.IsEnum)
                    {
                        convertedValue =
                            Enum.Parse(
                                targetType,
                                value.ToString()!,
                                true);
                    }
                    else
                    {
                        convertedValue =
                            Convert.ChangeType(
                                value,
                                targetType);
                    }

                    prop.SetValue(
                        item,
                        convertedValue);
                }
                catch(Exception ex)
                {
                #if DEBUG
                    Console.WriteLine(ex);
                #endif
                }
            }

            return item;
        }
    }
}