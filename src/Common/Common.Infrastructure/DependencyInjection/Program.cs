using System.Threading.Tasks;
using Common.Infrastructure.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Common.Infrastructure.DependencyInjection
{
    public static class Program<TStartup> where TStartup : class
    {
        public static Task Main(string[] args)
        {
            return CreateHostBuilder(args).RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<TStartup>();
            });
    }
}
