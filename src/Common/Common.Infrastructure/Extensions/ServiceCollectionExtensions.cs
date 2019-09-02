using Common.Core;
using Common.Core.Extensions;
using Common.Infrastructure.EfCore;
using EventStore.ClientAPI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System.Linq;
using System.Reflection;

namespace Common.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRedis(this IServiceCollection services, string connectionString, params Assembly[] assembliesToScan)
        {
            services
                .AddScoped<IConnectionMultiplexer>(x => ConnectionMultiplexer.Connect(connectionString))
                .AddScoped(x => x.GetService<IConnectionMultiplexer>().GetDatabase());

            var readObjects = typeof(ReadObject).GetDescendantTypes(assembliesToScan);

            var repos = readObjects
                .Select(r => new
                {
                    @interface = typeof(IReadModelRepository<>).MakeGenericType(r),
                    implementation = typeof(RedisReadModelRepository<>).MakeGenericType(r)
                });

            foreach (var pair in repos)
            {
                services.AddScoped(pair.@interface, pair.implementation);
            }

            services.AddScoped<IReadModelRepository<EventPosition>, RedisReadModelRepository<EventPosition>>();

            return services;
        }

        public static IServiceCollection AddMediator(this IServiceCollection services, params Assembly[] assembliesToScan)
        {
            return services
                .AddTransient<IRequestMediator, Mediator>()
                .AddTransient<IEventBus, Mediator>()
                .Scan(scan => scan.FromAssemblies(assembliesToScan)
                    .AddClasses(classes => classes.AssignableToAny(typeof(IHandleQuery<,>), typeof(IHandleCommand<>), typeof(IHandleEvent<>)))
                        .AsImplementedInterfaces()
                            .WithScopedLifetime());
        }

        public static IServiceCollection AddEventStore(this IServiceCollection services, string connectionString)
        {
            return services
                .AddSingleton(new ApplicationName(Assembly.GetEntryAssembly().GetName().Name))
                .AddScoped(x=> EventStoreConnection.Create(connectionString))
                .AddScoped<IMessageBusListener, EventStoreListener>(x=> 
                {
                    var conn = x.GetRequiredService<IEventStoreConnection>();
                    var bus = x.GetRequiredService<IEventBus>();
                    var repo = x.GetRequiredService<IReadModelRepository<EventPosition>>();
                    var appName = x.GetRequiredService<ApplicationName>();
                    var uow = x.GetService<IUnitOfWork>();

                    return new EventStoreListener(conn, bus, repo, appName, uow);
                })
                .AddScoped<IRepository>(x=> new EventStoreRepository(connectionString));
        }

        public static IServiceCollection AddEfCore(this IServiceCollection services, string connectionString, Assembly assemblyToScan)
        {
            return services
                .AddScoped(_ => 
                {
                    var optionsBuilder = new DbContextOptionsBuilder<GenericDbContext>();
                    optionsBuilder.UseSqlServer(connectionString);

                    var context = new GenericDbContext(optionsBuilder.Options, assemblyToScan);
                    context.Database.EnsureCreated();

                    return context;
                })
                .AddScoped<IReadModelRepository<EventPosition>, EfCoreRepository<EventPosition>>()
                .AddScoped<IUnitOfWork, EfCoreUnitOfWork>();
        }
    }
}
