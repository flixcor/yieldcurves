using Common.EventStore.Lib;
using Common.EventStore.Lib.DependencyInjection;
using Common.EventStore.Lib.Postgres;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjections
    {
        public static void WithPostgres(this IPersistenceOption option, string connectionString) => option.Services.AddPostgres(connectionString);

        private static void AddPostgres(this IServiceCollection services, string connectionString)
        {
            services
                .AddSingleton<IEventWriteRepository, EventRepository>(_ => new EventRepository(connectionString))
                .AddSingleton<IEventReadRepository, EventRepository>(_ => new EventRepository(connectionString))
                .AddHostedService(_ => new Common.EventStore.Lib.Postgres.EventListener(connectionString));
        }
    }
}
