﻿using Common.Infrastructure.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;

namespace Common.EventStore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddJsonOptions(x=> x.JsonSerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb));
            services.AddCors();
            services.AddMediator();
            services.AddRedis("localhost:6379");
            services.AddEventStore(o => o.WithGES(Configuration.GetConnectionString("EventStore"), "admin", "changeit"));
            //services.AddEventStore(o => o.WithPostgres(Configuration.GetConnectionString("Postgres")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(builder =>
            {
                builder.WithOrigins("http://localhost:8081")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
