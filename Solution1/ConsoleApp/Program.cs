using System;
using System.Threading.Tasks;
using AutoMapper;
using BusinessLayer.Abstract;
using BusinessLayer.Service;
using BusinessLayer.Service.Abstract;
using ConsoleApp.AutoMap;
using Ninject;

namespace ConsoleApp
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var ninjectKernel = new StandardKernel();
            ninjectKernel.Bind<IWeatherServise>().To<WeatherService>();
            ninjectKernel.Bind<IApiService>().To<ApiService>();
            ninjectKernel.Bind<IPrintService>().To<PrintService>();
            ninjectKernel.Bind<IMapper>().To<Mapper>()
                .WithConstructorArgument("configurationProvider", MapperConfig.GetConfiguration());

            IWeatherServise weatherService = ninjectKernel.Get<IWeatherServise>();

            while (true)
            {
                Console.WriteLine("Please, enter city name:");

                var cityName = Console.ReadLine();
                if (string.IsNullOrEmpty(cityName))
                {
                    continue;
                }

                Console.WriteLine(await weatherService.GetByCityNameAsync(cityName));
                Console.WriteLine('\n');
            }
        }
    }
}
