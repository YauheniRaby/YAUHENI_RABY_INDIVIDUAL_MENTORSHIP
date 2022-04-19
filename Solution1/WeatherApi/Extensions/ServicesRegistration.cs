using AutoMapper;
using BusinessLayer.Command;
using BusinessLayer.Command.Abstract;
using BusinessLayer.Services;
using BusinessLayer.Services.Abstract;
using Microsoft.Extensions.DependencyInjection;
using WeatherApi.AutoMap;

namespace WeatherApi.Extensions
{
    public static class ServicesRegistration
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddSingleton<IWeatherServiсe, WeatherService>();
            services.AddSingleton<ILogWeatherService, LogWeatherService>();
            services.AddSingleton<IInvoker, Invoker>();
            services.AddSingleton<IMapper>(service => new Mapper(MapperConfig.GetConfiguration()));
            services.AddSingleton<IBackgroundJobService, BackgroundJobService>();

            services.AddHttpClient<IWeatherApiService, WeatherApiService>();
        }
    }
}