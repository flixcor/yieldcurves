using System.Threading;
using System.Threading.Tasks;
using Common.EventStore.Lib.EfCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Common.EventStore
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var build = CreateHostBuilder(args).Build();
            var ding = (IEventListener)build.Services.GetService(typeof(IEventListener));

            var dingTask = ding.ListenAsync(CancellationToken.None);
            var runTask = build.RunAsync();

            await Task.WhenAll(dingTask, runTask);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
