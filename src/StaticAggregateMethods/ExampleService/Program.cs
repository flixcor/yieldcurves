using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
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
            using var eventStore = new InMemoryEventStore();
            Bootstrap.Setup(null);
            _ = WriteEvents(eventStore, default);
            CreateHostBuilder(args).Build().Run();
        }

        private static async Task WriteEvents(IEventStore eventStore, CancellationToken token)
        {
            await foreach (var item in eventStore.Subscribe(token))
            {
                try
                {
                    InMemoryProjectionStore.Project(item);
                    var json = JsonSerializer.Serialize(item, RestMapper.Options);
                    Console.WriteLine();
                    Console.WriteLine(EventMapper.Name(item.Content.GetType()));
                    Console.WriteLine(json);
                }
                catch (Exception e)
                {

                    Console.WriteLine(e.Message);
                }
                
            }
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
