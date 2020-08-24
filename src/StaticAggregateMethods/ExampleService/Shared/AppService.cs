using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static ExampleService.Shared.Delegates;

namespace ExampleService.Shared
{
    public static class AppService
    {
        public static async Task Handle<S, C>(Handle<S, C> handler, CommandEnvelope<C> commandEnvelope, IEventStore eventStore, CancellationToken cancel = default) where C : class where S: class, new()
        {
            if (commandEnvelope.AggregateId == null || commandEnvelope.Command == null)
            {
                throw new Exception();
            }

            var streamName = Registry.GetStreamName<S>(commandEnvelope.AggregateId);

            var (version, state) = await Load<S>(streamName, eventStore, cancel);

            var events = handler(state, commandEnvelope.Command)
                .Select((e, i) => new EventEnvelope { AggregateId = commandEnvelope.AggregateId, Version = version + i + 1, Content = e })
                .ToArray();

            await eventStore.Save(streamName, cancel, events);
        }

        private static async Task<(int, S)> Load<S>(string streamName, IEventStore eventStore, CancellationToken cancel) where S: class, new()
        {
            var state = new S();

            var version = -1;

            await foreach (var item in eventStore.Get(streamName, cancel))
            {
                state = Registry.When(state, item?.Content);
                version = Math.Max(item.Version, version);
            }

            return (version, state as S);
        }
    }
}
