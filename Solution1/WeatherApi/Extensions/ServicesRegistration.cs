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
            services.AddSingleton<ISaveWeatherService, SaveWeatherService>();
            services.AddSingleton<IInvoker, Invoker>();
            services.AddSingleton<IMapper>(service => new Mapper(MapperConfig.GetConfiguration()));
            services.AddSingleton<IBackgroundJobService, BackgroundJobService>();
            services.AddSingleton<IHistoryWeatherService, HistoryWeatherService>();

            services.AddHttpClient<IWeatherApiService, WeatherApiService>();
        }
    }
}