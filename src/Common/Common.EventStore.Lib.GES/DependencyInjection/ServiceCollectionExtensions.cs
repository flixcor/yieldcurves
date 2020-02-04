using System;
using System.Reflection;
using Common.Core;
using Common.EventStore.Lib;
using Common.EventStore.Lib.DependencyInjection;
using Common.EventStore.Lib.GES;
using EventStore.Client;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void WithGES(this IPersistenceOption o, string connectionString) => o.Services.AddGES(connectionString);

        private static IServiceCollection AddGES(this IServiceCollection services, string connectionString) => services
            .AddSingleton(new ApplicationName(Assembly.GetEntryAssembly()?.GetName()?.Name ?? throw new Exception()))
            .AddSingleton(x => new EventStoreClient(new EventStoreClientSettings(new Uri(connectionString))))
            .AddScoped<IMessageBusListener, EventStoreListener>()
            .AddScoped<IEventRepository, EventRepository>()
            .AddTransient<EventStoreQuery>()
            .AddTransient<EventStoreSocketSubscriber>();
    }
}
