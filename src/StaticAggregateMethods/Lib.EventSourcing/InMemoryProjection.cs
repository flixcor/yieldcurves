using System;
using System.Linq;
using Projac;

namespace Lib.EventSourcing
{
    public abstract class InMemoryProjection<T> : Projection<InMemoryProjectionStore> where T : class, new()
    {
        protected InMemoryProjection() => 
            When<EventEnvelope>((mem, e) =>
            {
                if (!Handlers.Any(x=> x.Message == e.GetType()))
                {
                    mem.UpdatePosition<T>(e.Position);
                }
            });

        protected void CreateOrUpdateWhen<E>(Func<EventEnvelope<E>, T, T> mapper, Func<EventEnvelope<E>, string>? getId = null) where E : class
        {
            getId ??= (e) => e.AggregateId ?? throw new Exception();

            When<EventEnvelope<E>>((mem, e) => mem.AddOrUpdate<T>(e.Position, (t) => mapper(e, t), getId(e)));
        }

        protected void DeleteWhen<E>(Func<EventEnvelope<E>, string>? getId = null) where E : class
        {
            getId ??= (e) => e.AggregateId ?? throw new Exception();

            When<EventEnvelope<E>>((mem, e) => mem.Delete<T>(e.Position, getId(e)));
        }
    }
}
