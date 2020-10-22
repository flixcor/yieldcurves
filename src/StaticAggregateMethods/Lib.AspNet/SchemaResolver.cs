using System;
using System.Collections.Generic;

namespace Lib.AspNet
{
    public static class SchemaResolver
    {
        private readonly static Dictionary<Type, string> s_dict = new Dictionary<Type, string>();

        public static bool TryRegister<TContract>(string schema) => s_dict.TryAdd(typeof(TContract), schema);

        public static string? GetSchema<TContract>()
        {
            s_dict.TryGetValue(typeof(TContract), out var result);
            return result;
        }
    }
}
