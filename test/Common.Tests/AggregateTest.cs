using System;
using System.Linq;
using Common.Core;
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
            Aggregate.LoadStateFromHistory(events);
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
            var count = events.Count();

            var uncommitted = Aggregate.GetUncommittedEvents().OrderBy(x => x.Version).ToArray();
            Assert.AreEqual(count, uncommitted.Count());

            for (var i = 0; i < count - 1; i++)
            {
                Assert.IsTrue(EventsMatch(events[i], uncommitted[i]));
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
