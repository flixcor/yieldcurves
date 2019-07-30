using Common.Core;
using Common.Core.Extensions;
using EventStore.ClientAPI;
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
                .AddScoped<IMessageBusListener, EventStoreListener>()
                .AddScoped<IRepository>(x=> new EventStoreRepository(connectionString));
        }
    }
}
