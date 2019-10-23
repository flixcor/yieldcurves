using System;
using System.Linq;
using Common.Core;
using NUnit.Framework;

namespace Common.Tests
{
    public abstract class AggregateTest<T> where T : Aggregate<T>
    {
        protected T Aggregate { get; private set; } = (T)Activator.CreateInstance(typeof(T), true);

        protected void Given(params Event[] events)
        {
            Aggregate.LoadStateFromHistory(events);
        }

        protected void WhenCreated(Func<T> func)
        {
            Aggregate = func();
        }

        protected void When(Action<T> action)
        {
            action(Aggregate);
        }

        protected virtual void Then(params IEvent[] events)
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
