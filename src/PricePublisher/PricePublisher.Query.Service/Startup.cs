using Common.Infrastructure.DependencyInjection;
using Common.Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PricePublisher.Query.Service.Features.GetPriceDates;

namespace PricePublisher.Query.Service
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
                .AddEfCore(Configuration.GetConnectionString("SqlServer"), typeof(Features.GetPricesOverview.Query).Assembly)
                .WithWebSockets();

            services.AddEventStore(o => o.WithGES(Configuration.GetConnectionString("EventStore"), "admin", "changeit"))
                .AddMediator(typeof(Features.GetPricesOverview.Query).Assembly);
        }
    }
}
