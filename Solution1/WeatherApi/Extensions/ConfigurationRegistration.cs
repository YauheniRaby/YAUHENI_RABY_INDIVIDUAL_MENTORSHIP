using BusinessLayer.Configuration.Abstract;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WeatherApi.Configuration;

namespace WeatherApi.Extensions
{
    public static class ConfigurationRegistration
    {
        public static void AddConfiguration(this IServiceCollection services, Config config)
        {
            services.AddSingleton<IConfig, Config>(services => config);
            services.AddLogging(opt =>
            {
                opt.AddConsole(c =>
                {
                    c.TimestampFormat = config.FormatDateTime;
                });
            });
        }
    }
}