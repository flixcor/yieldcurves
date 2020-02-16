using System;
using System.Net.Http;
using System.Net.Security;
using Common.EventStore.Lib;
using Common.EventStore.Lib.DependencyInjection;
using Common.EventStore.Lib.GES;
using EventStore.Client;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void WithGES(this IPersistenceOption o, string connectionString, string username, string password) => o.Services.AddGES(connectionString, username, password);

        private static IServiceCollection AddGES(this IServiceCollection services, string connectionString, string username, string password)
        {
            var eventStoreClient = new EventStoreClient(new Uri(connectionString),
            () => new HttpClient(new SocketsHttpHandler
            {
                SslOptions = new SslClientAuthenticationOptions
                {
                    RemoteCertificateValidationCallback = delegate { return true; }
                }
            }));

            var uri = new Uri(connectionString);

            return services
                .AddSingleton(x => eventStoreClient)
                .AddSingleton(new UserCredentials(username, password))
                .AddSingleton<IEventWriteRepository, EventRepository>()
                .AddSingleton<IEventReadRepository, EventRepository>();
        }
    }
}
