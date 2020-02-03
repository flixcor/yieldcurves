using CalculationEngine.Query.Service.Features.GetCalculatedCurveDetail;
using Common.Infrastructure.DependencyInjection;
using Common.Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CalculationEngine.Query.Service
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
                .AddEfCore(Configuration.GetConnectionString("SqlServer"), typeof(Features.GetCalculatedCurveDetail.Query).Assembly);

            services.AddEventStore(o => o.WithGES(Configuration.GetConnectionString("EventStore")))
                .AddMediator(typeof(Features.GetCalculatedCurveDetail.Query).Assembly);
        }
    }
}
