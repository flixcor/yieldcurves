using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleService.Shared
{
    public static class StreamNameMapper
    {


        private static readonly Dictionary<Type, Func<string, string>> s_dict = new Dictionary<Type, Func<string, string>>();

        public static bool TryMap<TState>(Func<string, string> getStreamName) => s_dict.TryAdd(typeof(TState), getStreamName);

        public static string GetStreamName<TState>(string id) =>
            s_dict.TryGetValue(typeof(TState), out var getStreamName)
                ? getStreamName(id)
                : id;
    }
}
