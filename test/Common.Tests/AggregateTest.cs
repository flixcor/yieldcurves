using System;
using System.Linq;
using Common.Core;
using Common.EventStore.Lib;
using NodaTime;
using NUnit.Framework;

namespace Common.Tests
{
    public interface IWhen<T> where T : Aggregate<T>
    {
        IThen<T> When(Action<T> action);
    }

    public interface IThen<T> where T : Aggregate<T>
    {
        void Then(params IEvent[] events);
    }

    public abstract class AggregateTest<T> : IWhen<T>, IThen<T> where T : Aggregate<T>
    {
        protected T Aggregate { get; private set; } = (T)Activator.CreateInstance(typeof(T), true);

        public IWhen<T> Given(params IEvent[] events)
        {
            var version = Aggregate.Version;

            foreach (var item in events)
            {
                version++;

                Aggregate.LoadFromHistory(new EventWrapper(item) 
                { 
                    AggregateId = Aggregate.Id,
                    Version = version
                });
            }
            
            return this;
        }

        public IThen<T> WhenCreated(Func<T> func)
        {
            Aggregate = func();

            return this;
        }

        IThen<T> IWhen<T>.When(Action<T> action)
        {
            action(Aggregate);
            return this;
        }

        void IThen<T>.Then(params IEvent[] events)
        {
            var wrapped = events.ToList();

            var uncommitted = Aggregate.GetUncommittedEvents().OrderBy(x => x.Metadata.Version).ToArray();
            Assert.AreEqual(wrapped.Count, uncommitted.Length);

            for (var i = 0; i < wrapped.Count - 1; i++)
            {
                Assert.IsTrue(EventsMatch(events[i], wrapped[i]));
            }
        }

        protected bool EventsMatch(IEvent a, IEvent b)
        {
            if (a.GetType() != b.GetType())
            {
                return false;
            }

            foreach (var prop in a.GetType().GetProperties().Where(x => x.Name != "Version"))
            {
                var valueA = prop.GetValue(a);
                var valueB = prop.GetValue(b);

                if (!valueA.Equals(valueB))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
