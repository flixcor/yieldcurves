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
            var link = RestMapper.TryMapQuery<NameAndAdd, IReadOnlyCollection<object>>("/test");

            IEnumerable<Link> GetLinks()
            {
                if (link != null)
                {
                    yield return link;
                }
            }

            RestMapper.TrySetIndex(GetLinks);

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
