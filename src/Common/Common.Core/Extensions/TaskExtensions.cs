using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Core.Extensions
{
    public static class TaskExtensions
    {
        public static async IAsyncEnumerable<T> Yield<T>(this Task<T> task)
        {
            var result = await task;

            if (result != null)
            {
                yield return result;    
            }
        }

        public static async IAsyncEnumerable<T> Yield<T>(this Task<Maybe<T>> task) where T : class
        {
            var result = await task;

            if (result.Found)
            {
                yield return result.Coalesce((T)default);
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
