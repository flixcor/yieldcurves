using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Infrastructure.Extensions
{
    public static class QueryableExtensions
    {
        public static async Task<T?> FirstOrNullAsync<T>(this IAsyncEnumerable<T> queryable, CancellationToken cancellationToken = default) where T : class
        {
            return await queryable.FirstOrDefaultAsync(cancellationToken);
        }
    }
}
