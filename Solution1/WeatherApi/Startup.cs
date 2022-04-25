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
using Microsoft.OpenApi.Models;
using WeatherApi.Configuration;
using WeatherApi.Extensions;
using WeatherApi.Infrastructure.Hangfire;

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
            var connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            services.AddDbContext<RepositoryContext>(options => options.UseSqlServer(connectionString), ServiceLifetime.Singleton);
            services.Configure<AppConfiguration>(Configuration.GetSection(nameof(AppConfiguration)));
            services.Configure<BackgroundJobConfiguration>(Configuration.GetSection(nameof(BackgroundJobConfiguration)));
            services.Configure<WeatherApiConfiguration>(Configuration.GetSection(nameof(WeatherApiConfiguration)));
            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

            services.AddSingleton<ExceptionHangfireFilter>();
            services.AddHangfire((provider, config) => config
                .UseSqlServerStorage(connectionString)
                .UseFilter(provider.GetService<ExceptionHangfireFilter>()));
            services.AddHangfireServer();

            services.AddStartupFilters();
            services.AddServices();
            services.AddRepositories();
            services.AddValidators();
            services.AddLogging(opt => opt.AddSimpleConsole());

            services.AddControllers();
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WeatherApi", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
        }
    }
}