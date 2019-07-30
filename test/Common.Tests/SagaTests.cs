using System.Linq;
using Common.Core;
using NUnit.Framework;

namespace Common.Tests
{
    public abstract class SagaTest<T> : AggregateTest<T> where T : Saga<T>
    {
        protected override void Then(params Event[] events)
        {
            var count = events.Count();

            var actual = Aggregate.GetUncommittedEvents().Where(x=> !(x is TransitionedEvent)).OrderBy(x => x.Version).ToArray();
            Assert.AreEqual(count, actual.Count());

            for (var i = 0; i < count - 1; i++)
            {
                Assert.IsTrue(EventsMatch(events[i], actual[i]));
            }
        }

        protected void Then<TEvent>(int count = 1) where TEvent : Event
        {
            var actual = Aggregate.GetUncommittedEvents().OfType<TEvent>();
            Assert.AreEqual(count, actual.Count());
        }

        protected void WhenTransitioned(params Event[] transitionedEvents)
        {
            foreach (var item in transitionedEvents)
            {
                Aggregate.TransitionEvent(item);
            }
        }

        protected void ThenTransitioned(params Event[] transitionedEvents)
        {
            var expected = transitionedEvents.Select(GetTransitionedEvent).ToArray();
            var count = expected.Count();
            var actual = Aggregate.GetUncommittedEvents().OfType<TransitionedEvent>().OrderBy(x => x.Version).ToArray();

            Assert.AreEqual(count, actual.Count());

            for (var i = 0; i < count - 1; i++)
            {
                Assert.IsTrue(EventsMatch(expected[i], actual[i]));
            }
        }

        private TransitionedEvent GetTransitionedEvent(Event e)
        {
            return e is TransitionedEvent t
                ? t
                : new TransitionedEvent(Aggregate.Id, e);
        }
    }
}
