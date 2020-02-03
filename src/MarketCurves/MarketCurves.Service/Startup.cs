using System.Reflection;
using Common.Infrastructure.DependencyInjection;
using Common.Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MarketCurves.Service
{
    public class Startup : YieldCurvesStartupBase
    {
        private static readonly Assembly s_assembly = typeof(Features.CreateMarketCurve.Query).Assembly;

        public Startup(IConfiguration configuration) : base(configuration, "http://localhost:8081", true, s_assembly)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            services
                .AddMediator(s_assembly)
                .AddRedis("localhost:6379", s_assembly);

            services.AddEventStore(o => o.WithGES(Configuration.GetConnectionString("EventStore")));
        }
    }
}
