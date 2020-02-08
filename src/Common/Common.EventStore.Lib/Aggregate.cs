
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Common.Core;
using NodaTime;
using static Common.Events.Helpers;

[assembly: InternalsVisibleTo("Common.Tests")]
namespace Common.EventStore.Lib
{
    public abstract class Aggregate<T> where T : Aggregate<T>, new()
    {
        private readonly List<IEventWrapper> _events = new List<IEventWrapper>();
        internal IEnumerable<IEventWrapper> GetUncommittedEvents() => _events;
        internal void ClearEvents() => _events.Clear();

        public NonEmptyGuid Id { get; internal set; } = Guid.NewGuid();
        internal int Version { get; private set; }

        internal void LoadFromHistory(IEventWrapper eventWrapper)
        {
            Version++;

            if (eventWrapper.Version != Version)
            {
                throw new Exception();
            }

            When(eventWrapper.GetContent());
        }

        protected void GenerateEvent(IEvent @event)
        {
            Version++;
            _events.Add(Wrap(Id, new Instant(), Version, @event));
            When(@event);
        }

        protected abstract void When(IEvent @event);
    }
}
