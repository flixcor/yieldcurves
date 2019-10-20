using Common.Infrastructure.DependencyInjection;
using Common.Infrastructure.Extensions;
using MarketCurves.Service.Features.AddCurvePoint;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MarketCurves.Service
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
                .AddMediator(typeof(Features.CreateMarketCurve.Query).Assembly)
                .AddRedis("localhost:6379", typeof(Features.AddCurvePoint.Instrument).Assembly);

            services.AddEventStore(Configuration.GetConnectionString("EventStore"));
        }
    }
}
