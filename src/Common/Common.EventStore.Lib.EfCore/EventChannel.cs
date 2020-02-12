using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using Common.Core;

namespace Common.EventStore.Lib.EfCore
{
    public static class EventChannel
    {
        private static readonly ConcurrentDictionary<Guid, Channel<PersistedEvent>> s_subscribers = new ConcurrentDictionary<Guid, Channel<PersistedEvent>>();

        public static void Publish(PersistedEvent tup)
        {
            foreach (var item in s_subscribers.Values)
            {
                item.Writer.TryWrite(tup);
            }
        }

        public static IAsyncEnumerable<PersistedEvent> Subscribe(CancellationToken cancellationToken)
        {
            var newChannel = Channel.CreateUnbounded<PersistedEvent>();
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
            Channel<PersistedEvent>? subscriber = null;

            while (!success && !cancellationToken.IsCancellationRequested)
            {
                success = s_subscribers.TryRemove(id, out subscriber);
            }

            subscriber?.Writer.Complete();
        }
    }
}
