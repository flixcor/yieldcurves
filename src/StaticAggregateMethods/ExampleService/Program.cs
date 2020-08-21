using ExampleService.Features;
using ExampleService.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ExampleService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var testEndpoint = RestMapper.TryMapQuery<Test, string?>("/test");
            var commandEndpoint = RestMapper.TryMapCommand<NameAndAdd>("/marketcurves")?.WithExpected(new NameAndAdd());

            var endpoints = RestMapper.Enumerate(testEndpoint, commandEndpoint);

            RestMapper.TryMapIndex(() => endpoints);

            RestMapper.SetAggregateStore(new EsAggregateStore(new InMemoryEventStore()));

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) => Host
            .CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(web => web
                .Configure(app => app
                    .UseRouting()
                    .UseEndpoints(e => e
                        .MapCommandsAndQueries())));
    }
}
