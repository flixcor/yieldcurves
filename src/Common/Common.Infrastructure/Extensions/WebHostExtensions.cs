using System.Threading.Tasks;
using Common.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Common.Infrastructure.Extensions
{
    public static class WebHostExtensions
    {
        public static async Task RunAsync(this IHostBuilder builder)
        {
            var host = builder.Build();
            using var scope = host.Services.CreateScope();
            var listener = scope.ServiceProvider.GetService<IMessageBusListener>();

            if (listener != null)
            {
                _ = listener.SubscribeToAll();
            }

            await host.RunAsync();
        }
    }
}
