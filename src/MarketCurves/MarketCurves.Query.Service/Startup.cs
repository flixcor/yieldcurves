using Common.Infrastructure.DependencyInjection;
using Common.Infrastructure.Extensions;
using MarketCurves.Query.Service.Features.GetMarketCurveDetail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MarketCurves.Query.Service
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
                .AddMediator(typeof(Features.GetMarketCurveDetail.Query).Assembly)
                .AddRedis("localhost:6379", typeof(InstrumentDto).Assembly)
                    .WithWebSockets()
                .AddEventStore(o => o.WithGES(Configuration.GetConnectionString("EventStore")));
        }
    }
}
