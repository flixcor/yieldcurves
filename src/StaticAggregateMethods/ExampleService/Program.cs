using System.Collections.Generic;
using ExampleService.Features;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ExampleService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var testEndpoint = RestMapper.TryMapQuery<NameAndAdd, IReadOnlyCollection<object>>("/test");

            RestMapper.TryMapIndex(() => testEndpoint.Enumerate());

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
