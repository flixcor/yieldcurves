using System;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Common.Core;

namespace Common.Infrastructure
{
    public class AkkaEventBus : IEventBus
    {
        private readonly IActorRef _actorRef;

        public AkkaEventBus(IActorRef actorRef)
        {
            _actorRef = actorRef ?? throw new ArgumentNullException(nameof(actorRef));
        }

        public Task Publish<T>(T @event, CancellationToken cancellationToken = default) where T : Event
        {
            _actorRef.Tell(@event);
            return Task.CompletedTask;
        }
    }
}
