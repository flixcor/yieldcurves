using Common.EventStore.Lib;
using Common.EventStore.Lib.DependencyInjection;
using Common.EventStore.Lib.EfCore;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class Extensions
    {
        public static void WithPostgres(this IPersistenceOption option, string connectionString, string? adminDb = null) => option.Services.AddPostgres(connectionString, adminDb);

        private static void AddPostgres(this IServiceCollection services, string connectionString, string? adminDb)
        {
            services
                .AddDbContext<EventStoreContext>(o => o.UseNpgsql(connectionString, u =>
                {
                    u.UseNodaTime();

                    if (adminDb != null)
                    {
                        u.UseAdminDatabase(adminDb);
                    }
                }))
                .AddScoped<IEventWriteRepository, EventRepository>();
        }
    }
}
