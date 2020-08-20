using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using static Common.Infrastructure.RestMapper;

namespace Instruments.Service
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var bbLink = TryMapCommand<Features.CreateBloombergInstrument.Command>("/bloomberg");
            var regularLink = TryMapCommand<Features.CreateRegularInstrument.Command>("/regular");

            TryMapQuery<Features.GetCommands.Query, Features.GetCommands.Dto>("/", dto => new 
            { 
                _links = new
                {
                    bloomberg = bbLink.WithExpected(dto.Bloomberg),
                    regular = regularLink.WithExpected(dto.Regular)
                }
            });

            var build = CreateHostBuilder(args).Build();
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
