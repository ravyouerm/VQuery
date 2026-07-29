using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace VQuery.Core
{
    internal static class ReflectionCache
    {
        private static readonly ConcurrentDictionary<
            Type,
            Dictionary<string, PropertyInfo>>
            Cache = new();

        public static Dictionary<string, PropertyInfo>
            GetPropertyMap(Type type)
        {
            return Cache.GetOrAdd(
                type,
                t => t.GetProperties()
                    .ToDictionary(
                        p => p.Name,
                        p => p,
                        StringComparer.OrdinalIgnoreCase));
        }
    }
}