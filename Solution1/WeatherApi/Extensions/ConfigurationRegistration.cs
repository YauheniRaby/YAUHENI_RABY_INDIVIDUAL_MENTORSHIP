using BusinessLayer.Configuration.Abstract;
using Microsoft.Extensions.DependencyInjection;
using WeatherApi.Configuration;

namespace WeatherApi.Extensions
{
    public static class ConfigurationRegistration
    {
        public static void AddConfiguration(this IServiceCollection services)
        {
            services.AddSingleton<IConfig, FileConfig>();
        }
    }
}