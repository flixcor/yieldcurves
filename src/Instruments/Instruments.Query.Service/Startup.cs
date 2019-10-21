using Common.Infrastructure.DependencyInjection;
using Common.Infrastructure.Extensions;
using Instruments.Query.Service.Features.GetInstrumentsOverview;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Instruments.Query.Service
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
                .AddMediator(typeof(Features.GetInstrumentsOverview.Query).Assembly)
                .AddRedis("localhost:6379", typeof(Dto).Assembly)
                    .WithWebSockets()
                .AddEventStore(Configuration.GetConnectionString("EventStore"));
        }
    }
}
