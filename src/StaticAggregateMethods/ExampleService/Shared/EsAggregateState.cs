using System;
using System.Collections.Generic;
using NodaTime;

namespace ExampleService.Shared
{
    public interface IAggregateState
    {
        string Id { get; }
    }

    public abstract record EsAggregateState<T>: IAggregateState where T : IAggregateState
    {
        public EsAggregateState()
        {
        }

        private readonly List<EventWrapper> _events = new List<EventWrapper>();
        public IReadOnlyCollection<EventWrapper> GetUncommittedEvents() => _events;
        internal void ClearEvents() => _events.Clear();

        internal int Version { get; private set; } = -1;
        public string Id { get; internal set; } = Guid.NewGuid().ToString();
        public virtual string StreamName => Id;

        protected abstract T When(object @event);

        internal T LoadFromHistory(EventWrapper eventWrapper)
        {
            if (eventWrapper.Version != Version + 1 || eventWrapper.Content is null)
            {
                throw new Exception();
            }

            Version++;

            return When(eventWrapper.Content);
        }

        public T Raise(object @event)
        {
            _events.Add(new EventWrapper
            {
                AggregateId = Id,
                Timestamp = SystemClock.Instance.GetCurrentInstant(),
                Version = Version + 1,
                Content = @event
            });

            Version++;

            return When(@event);
        }
    }
}
