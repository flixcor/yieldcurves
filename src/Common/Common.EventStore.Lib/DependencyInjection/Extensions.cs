using System;
using Common.Core;
using Common.Infrastructure;
using Common.EventStore.Lib;
using Common.EventStore.Lib.DependencyInjection;
using Common.EventStore.Lib.Internal;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class Extensions
    {
        public static IServiceCollection AddEventStore(this IServiceCollection services, Action<IPersistenceOption> persistence)
        {
            services.AddScoped<IAggregateRepository, AggregateRepository>();
            services.AddSingleton<IMessageBusListener, EventListener>();
            services.AddHostedService<MessageBusListenerHostedService>();

            var options = new PersistenseOption(services);
            persistence(options);

            return services;
        }
    }
}
