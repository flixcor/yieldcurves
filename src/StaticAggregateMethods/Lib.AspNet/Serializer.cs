using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Lib.AspNet
{
    public static class Serializer
    {
        public static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            IgnoreNullValues = true
        };

        private delegate Task<object?> DeserializeDelegate(Stream input, CancellationToken cancellationToken);
        private readonly static Dictionary<Type, DeserializeDelegate> s_dict = new Dictionary<Type, DeserializeDelegate>();

        public static bool TryRegister<TContract>(Func<JsonElement, TContract> func) => s_dict.TryAdd(typeof(TContract), GetDelegate(func));

        public static async Task<TContract?> DeserializeAsync<TContract>(Stream input, CancellationToken token)
        {
            if (s_dict.TryGetValue(typeof(TContract), out var del))
            {
                var obj = await del(input, token);
                return (TContract?)obj;
            }

            return await JsonSerializer.DeserializeAsync<TContract>(input, Options, token);
        }

        private static DeserializeDelegate GetDelegate<TContract>(Func<JsonElement, TContract> func) => async (stream, token) =>
        {
            var doc = await JsonDocument.ParseAsync(stream, default, token);
            return func(doc.RootElement);
        };
    }
}
