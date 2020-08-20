using System.Reflection;
using Common.Infrastructure.Controller;
using Common.Infrastructure.SignalR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Common.Infrastructure.DependencyInjection
{
    public abstract class YieldCurvesStartupBase
    {
        private readonly string? _frontEndUrl;
        private readonly bool _withSignalR;

        public IConfiguration Configuration { get; }
        protected Assembly[] AssembliesToScan { get; }

        protected YieldCurvesStartupBase(IConfiguration configuration, string? frontEndUrl = null, bool withSignalR = false, params Assembly[] assembliesToScan)
        {
            Configuration = configuration;
            _frontEndUrl = frontEndUrl;
            _withSignalR = withSignalR;
            AssembliesToScan = assembliesToScan;
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(o => o.Conventions.Add(new GenericControllerRouteConvention()))
                .ConfigureApplicationPartManager(apm => apm.FeatureProviders
                    .Add(new GenericControllerFeatureProvider(AssembliesToScan)))
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            if (!string.IsNullOrWhiteSpace(_frontEndUrl))
            {
                services.AddCors();
            }

            if (_withSignalR)
            {
                services.AddSignalR();
            }
        }

        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!string.IsNullOrWhiteSpace(_frontEndUrl))
            {
                app.UseCors(builder =>
                {
                    builder.WithOrigins(_frontEndUrl)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            if (!env.IsDevelopment())
            {
                app.UseHttpsRedirection();    
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.RegisterCommandsAndQueries();

                if (_withSignalR)
                {
                    endpoints.MapHub<GenericHub>("/hub");
                    endpoints.MapControllers();
                }
            });
        }
    }
}
