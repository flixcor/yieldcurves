using Common.EventStore.Lib;
using Common.EventStore.Lib.EfCore;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class Extensions
    {
        public static IServiceCollection AddEventStoreContext(this IServiceCollection services, string connectionString, string? adminDb = null) =>
            services
                .AddDbContext<EventStoreContext>(o => o.UseNpgsql(connectionString, u => 
                {
                    u.UseNodaTime();

                    if (adminDb != null)
                    {
                        u.UseAdminDatabase(adminDb);
                    }
                }))
                .AddScoped<IAggregateRepository, AggregateRepository>()
                .AddScoped<IEventRepository, EventRepository>();
    }
}
