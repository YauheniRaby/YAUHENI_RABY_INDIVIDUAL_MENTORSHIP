using System.Net.Http;
using AutoMapper;
using BusinessLayer.Abstract;
using BusinessLayer.Service;
using BusinessLayer.Service.Abstract;
using ConsoleApp.AutoMap;
using ConsoleApp.Configuration;
using Microsoft.Extensions.Logging;
using Ninject;

namespace ConsoleApp.Extension
{
    public static class RegisterDependencies
    {
        public static void AddServices(this IKernel ninjectKernel)
        {
            ninjectKernel.Bind<IWeatherServiсe>().To<WeatherService>();
            ninjectKernel.Bind<IWeatherApiService>().To<WeatherApiService>()
                .WithConstructorArgument("httpClient", new HttpClient());
            ninjectKernel.Bind<IMapper>().To<Mapper>()
                .WithConstructorArgument("configurationProvider", MapperConfig.GetConfiguration());
        }
    }


}
