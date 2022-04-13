using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using WeatherApi.Infrastructure;

namespace WeatherApi.Extensions
{
    public static class StartupFiltersRegistration
    {
        public static void AddStartupFilters(this IServiceCollection services)
        {
            services.AddTransient<IStartupFilter, BackgroundJobFilter>();
        }
    }
}
