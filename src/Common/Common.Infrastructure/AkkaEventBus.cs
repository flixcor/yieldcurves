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

        public Task Publish(IEventWrapper @event, CancellationToken cancellationToken = default)
        {
            _actorRef.Tell(@event);
            return Task.CompletedTask;
        }
    }
}
