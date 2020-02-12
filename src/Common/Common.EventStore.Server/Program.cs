using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Common.EventStore
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var build = CreateHostBuilder(args).Build();
            //var ding = (IEventListener)build.Services.GetService(typeof(IEventListener));

            await build.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
