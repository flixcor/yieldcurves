
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Common.Core;

[assembly: InternalsVisibleTo("Common.Tests")]
namespace Common.EventStore.Lib
{
    public abstract class Aggregate<T>
    {
        private readonly List<IEventWrapper> _events = new List<IEventWrapper>();
        internal IEnumerable<IEventWrapper> GetUncommittedEvents() => _events;
        internal void ClearEvents() => _events.Clear();

        public Guid Id { get; internal set; } = Guid.NewGuid();
        internal int Version { get; private set; }

        internal void LoadFromHistory(IEventWrapper eventWrapper)
        {
            Version++;

            if (eventWrapper.Metadata.Version != Version)
            {
                throw new Exception();
            }

            Apply(eventWrapper.Content);
        }

        protected void ApplyEvent(IEvent @event)
        {
            Version++;
            _events.Add(new EventWrapper(@event)
            {
                AggregateId = Id,
                Version = Version
            });
            Apply(@event);
        }

        protected abstract void Apply(IEvent @event);
    }
}
