using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Common.Core;

namespace Common.EventStore.Lib.EfCore
{
    public static class EventChannel
    {
        private static readonly ConcurrentDictionary<Guid, Channel<(IEventWrapper, IMetadata)>> s_subscribers = new ConcurrentDictionary<Guid, Channel<(IEventWrapper, IMetadata)>>();

        public static Task PublishAsync((IEventWrapper, IMetadata) tup, CancellationToken cancellationToken)
        {
            var tasks = s_subscribers.Values.Select(async x => await x.Writer.PublishAsync(tup, cancellationToken));
            return Task.WhenAll(tasks);
        }

        public static IAsyncEnumerable<(IEventWrapper, IMetadata)> Subscribe(CancellationToken cancellationToken)
        {
            var newChannel = Channel.CreateUnbounded<(IEventWrapper, IMetadata)>();
            var id = Guid.NewGuid();

            var success = false;

            while (!success && !cancellationToken.IsCancellationRequested)
            {
                success = s_subscribers.TryAdd(id, newChannel);
            }

            cancellationToken.Register(() => RemoveFromBag(id));

            return newChannel.Reader.ReadAllAsync();
        }

        private static void RemoveFromBag(Guid id, CancellationToken cancellationToken = default)
        {
            var success = false;
            Channel<(IEventWrapper, IMetadata)>? subscriber = null;

            while (!success && !cancellationToken.IsCancellationRequested)
            {
                success = s_subscribers.TryRemove(id, out subscriber);
            }

            subscriber?.Writer.Complete();
        }
    }
}
