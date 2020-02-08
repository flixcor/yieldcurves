using System;
using System.Linq;
using Common.Core;
using Common.EventStore.Lib;
using NodaTime;
using NUnit.Framework;
using static Common.Events.Helpers;

namespace Common.Tests
{
    public interface IWhen<T> where T : Aggregate<T>, new()
    {
        IThen<T> When(Action<T> action);
    }

    public interface IThen<T> where T : Aggregate<T>, new()
    {
        void Then(params IEvent[] events);
    }

    public abstract class AggregateTest<T> : IWhen<T>, IThen<T> where T : Aggregate<T>, new()
    {
        protected T Aggregate { get; private set; } = new T();

        public IWhen<T> Given(params IEvent[] events)
        {
            var version = Aggregate.Version;

            foreach (var item in events)
            {
                version++;

                Aggregate.LoadFromHistory(Wrap(Aggregate.Id, new Instant(), version, item));
            }
            
            return this;
        }

        public IThen<T> When(Action<T> action)
        {
            action(Aggregate);
            return this;
        }

        void IThen<T>.Then(params IEvent[] events)
        {
            var wrapped = events.ToList();

            var uncommitted = Aggregate.GetUncommittedEvents().OrderBy(x => x.Version).ToArray();
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
