using System;
using System.Linq;
using System.Reflection;
using Common.Core;
using Common.Core.Extensions;
using Common.EventStore.Lib;
using Common.Infrastructure.DependencyInjection;
using Common.Infrastructure.EfCore;
using Common.Infrastructure.SignalR;
using EventStore.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Common.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IReadModelImplementation AddRedis(this IServiceCollection services, string connectionString, params Assembly[] assembliesToScan)
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

            return new ReadModelImplementation(services, readObjects);
        }

        public static IServiceCollection AddMediator(this IServiceCollection services, params Assembly[] assembliesToScan)
        {
            return services
                .AddTransient<IEventBus, InternalEventBus>()
                .Scan(scan => scan.FromAssemblies(assembliesToScan)
                    .AddClasses(classes => classes.AssignableToAny(typeof(IHandleQuery<,>), typeof(IHandleCommand<>), typeof(IHandleEvent<>)))
                        .AsImplementedInterfaces()
                            .WithScopedLifetime());
        }

        public static IServiceCollection AddEventStore(this IServiceCollection services, string connectionString) => services
                .AddSingleton(new ApplicationName(Assembly.GetEntryAssembly()?.GetName()?.Name ?? throw new Exception()))
                .AddSingleton(x => new EventStoreClient(new EventStoreClientSettings(new Uri(connectionString))))
                .AddScoped<IMessageBusListener, EventStoreListener>()
                .AddScoped<IEventRepository, EventStoreRepository>();

        public static IReadModelImplementation AddEfCore(this IServiceCollection services, string connectionString, params Assembly[] assemblyToScan)
        {
            return AddEfCore<GenericDbContext>(services, connectionString, assemblyToScan);
        }

        public static IReadModelImplementation AddEfCore<T>(this IServiceCollection services, string connectionString, params Assembly[] assemblyToScan) where T : GenericDbContext
        {
            var readModelTypes = typeof(ReadObject).GetDescendantTypes(assemblyToScan).ToList();

            services
                .AddScoped<GenericDbContext>(_ =>
                {
                    var optionsBuilder = new DbContextOptionsBuilder<T>();
                    optionsBuilder.UseSqlServer(connectionString);

                    var context = (T?)Activator.CreateInstance(typeof(T), optionsBuilder.Options, readModelTypes) ?? throw new Exception();
                    context.Database.EnsureCreated();

                    return context;
                })
                .AddScoped<IReadModelRepository<EventPosition>, EfCoreRepository<EventPosition>>()
                .AddScoped<IUnitOfWork, EfCoreUnitOfWork>();

            var repos = readModelTypes
                .Select(r => new
                {
                    @interface = typeof(IReadModelRepository<>).MakeGenericType(r),
                    implementation = typeof(EfCoreRepository<>).MakeGenericType(r)
                });

            foreach (var pair in repos)
            {
                services.AddScoped(pair.@interface, pair.implementation);
            }

            return new ReadModelImplementation(services, readModelTypes);
        }

        public static IServiceCollection WithWebSockets(this IReadModelImplementation readModelImplementation)
        {
            var services = readModelImplementation.GetServiceCollection();
            var usedTypes = readModelImplementation.GetUsedTypes();

            services.AddTransient<ISocketContext, GenericHubContext>();

            var decorators = usedTypes
                .Select(r => new
                {
                    @interface = typeof(IReadModelRepository<>).MakeGenericType(r),
                    implementation = typeof(SocketRepositoryDecorator<>).MakeGenericType(r)
                });


            foreach (var decorator in decorators)
            {
                services.Decorate(decorator.@interface, decorator.implementation);
            }

            return services;
        }
    }
}
