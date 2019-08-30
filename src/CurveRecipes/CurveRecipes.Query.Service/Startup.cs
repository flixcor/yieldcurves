using Common.Infrastructure.Extensions;
using CurveRecipes.Query.Service.Features;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CurveRecipes.Query.Service
{
    public class Startup
    {
        private const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            

            services.AddRedis("localhost:6379", typeof(GetCurveRecipeDetail).Assembly)
                .AddMediator(typeof(GetCurveRecipeDetail).Assembly)
                .AddEventStore(Configuration.GetConnectionString("EventStore"));

            services.AddControllers();

            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                builder =>
                {
                    builder.WithOrigins("http://localhost:8081","http://127.0.0.1:8081").WithMethods("GET").AllowAnyHeader();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(MyAllowSpecificOrigins);

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            
            
        }
    }
}
