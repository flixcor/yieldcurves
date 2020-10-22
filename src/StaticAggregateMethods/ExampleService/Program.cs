using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExampleService.ReadSide;
using Lib.AspNet;
using Lib.EventSourcing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Projac;

namespace Lib
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            using var cancel = new CancellationTokenSource();

            using var eventStore = new InMemoryEventStore();
            Bootstrap.Setup(eventStore);
            var host = CreateHostBuilder(args).Build();
            _ = WriteEvents(eventStore, cancel.Token);
            await host.RunAsync(cancel.Token);
        }

        private static async Task WriteEvents(IEventStore eventStore, CancellationToken token)
        {
            var projections = new GetCurve.Projection().Handlers.Concat(new GetCurveList.Projection().Handlers).ToArray();
            var projector = new Projector<InMemoryProjectionStore>(Resolve.WhenAssignableToHandlerMessageType(projections));

            await foreach (var item in eventStore.Subscribe(token))
            {
                try
                {
                    await projector.ProjectAsync(InMemoryProjectionStore.Instance, item, token);
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
                    .UseEndpoints(e =>
                    {
                        e.MapCommandsAndQueries();
                        e.MapGet("test.html", (context) =>
                        {
                            var response = context.Response;
                            response.StatusCode = 200;
                            response.ContentType = "text/html";
                            return response.WriteAsync(Body, context.RequestAborted);
                        });
                    })));

        const string Body = @"
<!DOCTYPE html>
<html>
    <head>
        <meta charset='utf-8\' />
        <title></title>
    </head>
    <body>
        <script>
            function doFetch()
            {
                fetch('/marketcurves', {
                    headers: {
                        'Accept-Encoding': 'gzip'
                    }
                }).then(response => {
                    response.json().then(body => {
                        console.log(body)
                    })
                })
            }

            function doPost()
            {
                fetch('/marketcurves', {
                    method: 'POST',
                    headers: {
                        'Accept-Encoding': 'gzip'
                    },
                    body: JSON.stringify({
                        name: 'sample',
                        instrument: 'instrument2'
                    })
                }).then(response => {
                    response.text().then(body => {
                        console.log(body)
                    })
                })
            }

            function doRandom() {
                const func = Math.random() >= 0.5
                    ? doFetch
                    : doPost
                
                func()
                setTimeout(() => doRandom(), 2000)
            }

            doRandom()
        </script>
    </body>
</html>
";
    }
}
