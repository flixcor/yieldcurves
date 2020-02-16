using System;
using Common.Core;
using Common.EventStore.Lib.DependencyInjection;
using Common.EventStore.Lib.Internal;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class Extensions
    {
        public static IServiceCollection AddEventStore(this IServiceCollection services, Action<IPersistenceOption> persistence)
        {
            services.AddScoped<IAggregateRepository, AggregateRepository>();


            var options = new PersistenseOption(services);
            persistence(options);

            return services;
        }
    }
}
