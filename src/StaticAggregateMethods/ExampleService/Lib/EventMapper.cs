using System;
using System.Collections.Generic;

namespace ExampleService.Shared
{
    public static class EventMapper
    {
        private static readonly Dictionary<Type, string> s_names = new Dictionary<Type, string>();

        public static string Name(Type type) => s_names[type];

        public static bool TryMap<T>(string name)
        {
            var type = typeof(T);
            var result = !s_names.ContainsKey(type);

            if (result)
            {
                s_names[type] = name;
            }

            return result;
        }
    }
}
