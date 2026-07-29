using System;
using System.Data;

namespace VQuery.Core
{
    internal static class ReaderMapper
    {
        public static T MapReader<T>(
            IDataRecord reader)
            where T : new()
        {
            T item = new();

            var propertyMap =
                ReflectionCache.GetPropertyMap(
                    typeof(T));

            for (int i = 0; i < reader.FieldCount; i++)
            {
                string columnName =
                    reader.GetName(i);

                if (!propertyMap.TryGetValue(
                    columnName,
                    out var prop))
                {
                    continue;
                }

                if (reader.IsDBNull(i))
                    continue;

                object value =
                    reader.GetValue(i);

                try
                {
                    Type targetType =
                        Nullable.GetUnderlyingType(
                            prop.PropertyType)
                        ?? prop.PropertyType;

                    object convertedValue;

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