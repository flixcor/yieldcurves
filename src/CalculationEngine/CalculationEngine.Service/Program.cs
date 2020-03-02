using System;
using System.Threading.Tasks;
using Common.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CalculationEngine.Service
{
    public class Program
    {
        public static void Main()
        {
            IServiceCollection services = new ServiceCollection();
            // Startup.cs finally :)
            var startup = new Startup();
            startup.ConfigureServices(services);
            IServiceProvider serviceProvider = services.BuildServiceProvider();

            var logger = serviceProvider.GetService<ILoggerFactory>()
            .CreateLogger<Program>();

            logger.LogDebug("Logger is working!");

            using var scope = serviceProvider.CreateScope();
            var listener = scope.ServiceProvider.GetService<IMessageBusListener>();

            var _ = listener.SubscribeToAll();
            Console.WriteLine("waiting for events. press enter to exit");
            Console.ReadLine();
        }
    }
}
