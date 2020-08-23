using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static ExampleService.Shared.Handlers;

namespace ExampleService.Shared
{
    public static class AppService
    {
        public static async Task Handle<C, S>(CommandHandler<C, S> handler, CommandEnvelope<C> commandEnvelope, IEventStore eventStore, CancellationToken cancel = default) where C : class where S: class, new()
        {
            if (commandEnvelope.AggregateId == null || commandEnvelope.Command == null)
            {
                throw new Exception();
            }

            var streamName = StreamNameMapper.GetStreamName<S>(commandEnvelope.AggregateId);
            var store = RestMapper.GetAggregateStore();

            var aggregate = await store.Load<S>(commandEnvelope.AggregateId, cancel);

            var events = handler(commandEnvelope.Command, aggregate.State)
                .Select((e, i) => new EventEnvelope { AggregateId = aggregate.Id, Version = aggregate.Version + i + 1, Content = e })
                .ToArray();

            await eventStore.Save(streamName, cancel, events);
        }
    }
}
