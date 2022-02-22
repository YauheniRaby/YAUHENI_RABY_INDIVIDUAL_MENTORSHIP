using System.Net.Http;
using AutoMapper;
using BusinessLayer.Abstract;
using BusinessLayer.Service;
using BusinessLayer.Service.Abstract;
using ConsoleApp.AutoMap;
using ConsoleApp.Configuration;
using ConsoleApp.Service;
using ConsoleApp.Service.Abstract;
using Microsoft.Extensions.Logging;
using Ninject;

namespace ConsoleApp.Extension
{
    public static class RegisterDependencies
    {
        public static void AddServices(this IKernel ninjectKernel)
        {
            ninjectKernel.Bind<IUserCommunicationService>().To<UserCommunicationService>();
            ninjectKernel.Bind<IWeatherServiсe>().To<WeatherService>();
            ninjectKernel.Bind<IWeatherApiService>().To<WeatherApiService>()
                .WithConstructorArgument("httpClient", new HttpClient());
            ninjectKernel.Bind<IMapper>().To<Mapper>()
                .WithConstructorArgument("configurationProvider", MapperConfig.GetConfiguration());
            ninjectKernel.Bind<ILogger>().ToMethod(x => LoggerConfiguration.GetConfiguration<Program>());
        }
    }
}
