using Common.Infrastructure.DependencyInjection;
using Common.Infrastructure.Extensions;
using Instruments.Service.Features.CreateBloombergInstrument;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Instruments.Service
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
                .AddMediator(typeof(Features.CreateRegularInstrument.Command).Assembly)
                .AddRedis("localhost:6379");

            services.AddEventStore(Configuration.GetConnectionString("EventStore"));
        }
    }
}
