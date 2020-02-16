using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace System.Linq
{
    public static class QueryableExtensions
    {
        public static async Task<T?> FirstOrNullAsync<T>(this IAsyncEnumerable<T> queryable, CancellationToken cancellationToken = default) where T : class
        {
            return await queryable.FirstOrDefaultAsync(cancellationToken);
        }

        public static async Task<T?> FirstOrNullAsync<T>(this IQueryable<T> queryable, CancellationToken cancellationToken = default) where T : class
        {
            return await queryable.FirstOrDefaultAsync(cancellationToken);
        }
    }
}
