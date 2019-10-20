using AutoMapper;
using Common.Infrastructure.DependencyInjection;
using Common.Infrastructure.Extensions;
using CurveRecipes.Domain;
using CurveRecipes.Service.Features.AddTransformation;
using CurveRecipes.Service.Features.CreateCurveRecipe;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CurveRecipes.Service
{
    public class Startup : YieldCurvesStartupBase
    {
        public Startup(IConfiguration configuration) : base(configuration, "http://localhost:8081", true, typeof(Handler).Assembly)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            services.AddRedis("localhost:6379", typeof(MarketCurveDto).Assembly);

            services.AddMediator(typeof(Command).Assembly)
                .AddEventStore(Configuration.GetConnectionString("EventStore"))
                .AddAutoMapper(typeof(Command).Assembly, typeof(Shift).Assembly);
        }
    }
}
