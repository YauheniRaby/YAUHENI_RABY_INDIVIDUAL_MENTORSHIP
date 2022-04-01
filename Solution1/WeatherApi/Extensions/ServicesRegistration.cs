using AutoMapper;
using BusinessLayer.Command;
using BusinessLayer.Command.Abstract;
using BusinessLayer.Services;
using BusinessLayer.Services.Abstract;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using WeatherApi.AutoMap;

namespace WeatherApi.Extensions
{
    public static class ServicesRegistration
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddSingleton<IWeatherServiсe, WeatherService>();
            services.AddSingleton<IInvoker, Invoker>();
            services.AddSingleton<IWeatherApiService>(service => new WeatherApiService(new HttpClient()));
            services.AddSingleton<IMapper>(service => new Mapper(MapperConfig.GetConfiguration()));            
        }
    }
}