using System.Threading.Tasks;

namespace System.Threading.Channels
{
    public static class ChannelExtensions
    {
        public static ValueTask<bool> PublishAsync<T>(this ChannelWriter<T> writer, T item, CancellationToken token)
        {
            async Task<bool> AsyncSlowPath(T thing)
            {
                while (await writer.WaitToWriteAsync(token))
                {
                    if (writer.TryWrite(thing)) return true;
                }
                return false; // Channel was completed during the wait
            }

            return writer.TryWrite(item) ? new ValueTask<bool>(true) : new ValueTask<bool>(AsyncSlowPath(item));
        }
    }
}
