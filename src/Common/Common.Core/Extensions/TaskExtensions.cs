using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Core.Extensions
{
    public static class TaskExtensions
    {
        public static async IAsyncEnumerable<T> Yield<T>(this Task<T?> task) where T : class
        {
            var result = await task;

            if (result != null)
            {
                yield return result;
            }
        }

        public static async IAsyncEnumerable<T> Yield<T>(this Task<Result<T>> task) where T : class
        {
            var result = await task;

            if (result.IsSuccessful)
            {
                yield return result.Content;
            }
        }
    }
}
