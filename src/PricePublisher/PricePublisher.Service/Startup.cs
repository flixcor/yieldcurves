using System;
using Common.Infrastructure.DependencyInjection;
using Common.Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PricePublisher.Service.Features.PublishPrice;

namespace PricePublisher.Service
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
                .AddMediator(typeof(Query).Assembly)
                .AddRedis("localhost:6379", typeof(Dto).Assembly);

            services.AddEventStore(o => o.WithGES(Configuration.GetConnectionString("EventStore")))
                .AddSingleton<Func<DateTime>>(() => DateTime.UtcNow);
        }
    }
}
