using System;
using System.Reflection;
using NodaTime;

namespace Common.Core
{
    public interface IEventWrapper
    {
        long Id { get; }
        Guid AggregateId { get; }
        Instant Timestamp { get; }
        int Version { get; }
        IEvent GetContent();
    }

    public interface IEventWrapper<T> : IEventWrapper where T : IEvent
    {
        T Content { get; }
    }

    internal class EventWrapper<T> : IEventWrapper<T> where T : IEvent
    {
        public EventWrapper(T content, long id, Guid aggregateId, Instant timestamp, int version)
        {
            Content = content;
            Id = id;
            AggregateId = aggregateId.NonEmpty();
            Timestamp = timestamp;
            Version = version;
        }

        public long Id { get; }
        public Guid AggregateId { get; }
        public Instant Timestamp { get; }
        public int Version { get; }

        public T Content { get; }

        IEvent IEventWrapper.GetContent() => Content;
    }

    public static class EventWrapperExtensions
    {
        public static IEventWrapper<T> Concrete<T>(this IEventWrapper abst) where T : IEvent
        {
            var content = abst.GetContent();

            if (content is T conc)
            {
                return new EventWrapper<T>(conc, abst.Id, abst.AggregateId, abst.Timestamp, abst.Version);
            }

            throw new Exception();
        }

        public static dynamic ConcreteGeneric(this IEventWrapper abst)
        {
            var concreteType = abst.GetContent().GetType();
            var ex = typeof(EventWrapperExtensions);
            var mi = ex.GetMethod("Concrete");
            var miConstructed = mi?.MakeGenericMethod(concreteType);
            object[] args = { abst};
            return miConstructed?.Invoke(null, args) ?? throw new Exception();
        }
    }
}
