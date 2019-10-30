using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Infrastructure.Extensions
{
    public static class AsyncEnumerableExtensions
    {
        public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> asyncEnumerable)
        {
            var list = new List<T>();
            await foreach (var item in asyncEnumerable)
            {
                list.Add(item);
            }

            return list;
        }

        public static async Task<T> FirstOrDefaultAsync<T>(this IAsyncEnumerable<T> asyncEnumerable)
        {
            await foreach (var item in asyncEnumerable)
            {
                return item;
            }

            return default;
        }
    }
}
