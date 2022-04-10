using BusinessLayer.DTOs;
using BusinessLayer.Services.Abstract;
using DataAccessLayer.Configuration;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using WeatherApi.Configuration;
using WeatherApi.Extensions;

namespace WeatherApi
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
            string connectionString = Configuration.GetConnectionString(Constants.Connection.NameDBConnection);
            services.AddDbContextFactory<RepositoryContext>(options => options.UseSqlServer(connectionString));
            
            services.Configure<AppConfiguration>(Configuration.GetSection(Constants.ConfigurationSection.AppConfiguration));
            services.Configure<PermanentRequestDTO>(Configuration.GetSection(Constants.ConfigurationSection.PermanentRequest));
            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

            services.AddHangfire(x => x.UseSqlServerStorage(connectionString));
            services.AddHangfireServer();

            services.AddLogging(opt => opt.AddSimpleConsole());
            services.AddValidators();
            services.AddServices();
            services.AddRepository();

            services.AddControllers();
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WeatherApi", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IBackgroundJobClient backgroundJobs, IWebHostEnvironment env, IOptionsMonitor<PermanentRequestDTO> monitor)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WeatherApi v1"));
                app.UseHangfireDashboard("/dashboard");
                
            }
            app.UseHttpStatusExceptionHandler();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            monitor.OnChange(appConfig => backgroundJobs.Enqueue<IBackgroundJobService>(x => x.UpdateJobs(appConfig.CitiesOptions)));
            backgroundJobs.Enqueue<IBackgroundJobService>(x => x.UpdateJobs(monitor.CurrentValue.CitiesOptions));
        }
    }
}