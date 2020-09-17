using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lib.Aggregates;

namespace Lib.EventSourcing
{
    public static class AppService
    {
        public static async Task<long?> Handle<S, C>(CommandEnvelope<C> commandEnvelope, IAggregate<S> aggregate, IEventStore eventStore, CancellationToken cancel = default) where C : class where S : class
        {
            if (commandEnvelope.AggregateId == null || commandEnvelope.Command == null)
            {
                throw new Exception();
            }

            var streamName = aggregate.GetStreamName(commandEnvelope.AggregateId);

            var (version, state) = await Load(streamName, aggregate, eventStore, cancel);

            var events = aggregate.Handle(state, commandEnvelope.Command)
                .Select((e, i) => e.CreateEventEnvelope(commandEnvelope.AggregateId, version + i + 1))
                .ToArray();

            return await eventStore.Save(streamName, cancel, events);
        }

        private static async Task<(int, S)> Load<S>(string streamName, IAggregate<S> aggregate, IEventStore eventStore, CancellationToken cancel) where S : class
        {
            var state = aggregate.InitState();

            var version = -1;

            await foreach (var item in eventStore.Get(streamName, cancel))
            {
                state = aggregate.When(state, item?.Content);
                version = Math.Max(item?.Version ?? -1, version);
            }

            return (version, state);
        }
    }
}
