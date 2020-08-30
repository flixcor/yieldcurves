using System;
using Projac;

namespace Lib.EventSourcing
{
    public abstract class InMemoryProjection<T> : Projection<InMemoryProjectionStore> where T : class, new()
    {
        protected void CreateOrUpdateWhen<E>(Func<EventEnvelope<E>, T, T> mapper, Func<EventEnvelope<E>, string>? getId = null) where E : class
        {
            getId ??= (e) => e.AggregateId;

            When<EventEnvelope<E>>((mem, e) =>
            {
                mem.AddOrUpdate<T>(e.Position, (t) => mapper(e, t), getId(e));
            });
        }

        protected void DeleteWhen<E>(Func<EventEnvelope<E>, string>? getId = null) where E : class
        {
            getId ??= (e) => e.AggregateId;

            When<EventEnvelope<E>>((mem, e) =>
            {
                mem.Delete<E>(e.Position, getId(e));
            });
        }
    }
}
