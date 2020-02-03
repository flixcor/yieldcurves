using Akka.Actor;
using CalculationEngine.Service.ActorModel.Actors;
using CalculationEngine.Service.Helpers;
using Common.Core;
using Common.Infrastructure;
using Common.Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CalculationEngine.Service
{
    public class Startup
    {
        public Startup()
        {
            var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddUserSecrets<Program>();

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(provider =>
            {
                var serviceScopeFactory = provider.GetService<IServiceScopeFactory>();
                var hocon = HoconLoader.FromFile("akka.net.hocon");

                var actorSystem = ActorSystem.Create("calculationengine", hocon);
                actorSystem.AddServiceScopeFactory(serviceScopeFactory);
                return actorSystem;
            });

            services.AddEventStore(o => o.WithGES(Configuration.GetConnectionString("EventStore")));
            services.AddEfCore<AkkaPersistenceContext>(Configuration.GetConnectionString("SqlServer"));
            services.AddLogging();

            services.AddScoped(x =>
            {
                var logger = x.GetService<ILogger>();
                var system = x.GetService<ActorSystem>();
                var actor = system.ActorOf<CalculationEngineActor>();
                return new AkkaEventBus(actor).WithLogging();
            });

            
            services.AddSingleton(Configuration);
        }
    }
}
