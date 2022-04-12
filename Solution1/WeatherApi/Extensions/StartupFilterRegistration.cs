using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using WeatherApi.Infrastructure;

namespace WeatherApi.Extensions
{
    public static class StartupFilterRegistration
    {
        public static void AddStartupFilter(this IServiceCollection services)
        {
            services.AddTransient<IStartupFilter, BackgroundJobFilter>();
        }
    }
}
