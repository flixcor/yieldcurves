using System;
using Common.EventStore.Lib.DependencyInjection;
using Common.EventStore.Lib.EfCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class Extensions
    {
        public static IServiceCollection AddEventStoreContext(this IServiceCollection services, Action<IPersistenceOption> persistence)
        {
            services.AddScoped<IAggregateRepository, AggregateRepository>();

            var options = new PersistenseOption(services);
            persistence(options);

            return services;
        }
    }
}
