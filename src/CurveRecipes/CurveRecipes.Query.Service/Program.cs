using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using static Common.Infrastructure.RestMapper;

namespace CurveRecipes.Query.Service
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var detailsLink = TryMapQuery<Features.GetCurveRecipeDetail.Query, Features.GetCurveRecipeDetail.Dto?>("/{id}");
            TryMapListQuery<Features.GetCurveRecipesOverview.Query, Features.GetCurveRecipesOverview.Dto>("/", dto => new 
            { 
                name = dto.Name,
                _links = new
                {
                    self = detailsLink?.WithRouteValues(new { id = dto.Id })
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
