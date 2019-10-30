using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Infrastructure.Extensions
{
    public static class AsyncEnumerableExtensions
    {
        //we will want to refactor so this isn't necessary
        public static async Task<IEnumerable<T>> AsEnumerableAsync<T>(this IAsyncEnumerable<T> asyncEnumerable)
        {
            return await asyncEnumerable.ToListAsync();
        }
    }
}
