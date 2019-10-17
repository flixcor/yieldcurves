using System.Reflection;
using Common.Infrastructure.SignalR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Common.Infrastructure.DependencyInjection
{
    public abstract class YieldCurvesStartupBase
    {
        private const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        private readonly string _frontEndUrl;
        private readonly bool _withSignalR;

        public IConfiguration Configuration { get; }
        protected Assembly[] AssembliesToScan { get; }

        protected YieldCurvesStartupBase(IConfiguration configuration, string frontEndUrl = null, bool withSignalR = false, params Assembly[] assembliesToScan)
        {
            Configuration = configuration;
            _frontEndUrl = frontEndUrl;
            _withSignalR = withSignalR;
            AssembliesToScan = assembliesToScan;
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            if (!string.IsNullOrWhiteSpace(_frontEndUrl))
            {
                services.AddCors(options =>
                {
                    options.AddPolicy(MyAllowSpecificOrigins,
                    builder =>
                    {
                        builder.WithOrigins(_frontEndUrl)
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowCredentials();
                    });
                });
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
                app.UseCors(MyAllowSpecificOrigins);
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                if (_withSignalR)
                {
                    endpoints.MapHub<GenericHub>("/hub");
                }
            });
        }
    }
}
