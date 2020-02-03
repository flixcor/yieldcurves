using Common.Infrastructure.DependencyInjection;
using Common.Infrastructure.Extensions;
using CurveRecipes.Query.Service.Features.GetCurveRecipeDetail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CurveRecipes.Query.Service
{
    public class Startup : YieldCurvesStartupBase
    {
        public Startup(IConfiguration configuration) : base(configuration, "http://localhost:8081", true, typeof(Handler).Assembly)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            services
                .AddRedis("localhost:6379", typeof(Features.GetCurveRecipeDetail.Query).Assembly)
                    .WithWebSockets()
                .AddMediator(typeof(Features.GetCurveRecipeDetail.Query).Assembly)
                .AddEventStore(o => o.WithGES(Configuration.GetConnectionString("EventStore")));
        }
    }
}
