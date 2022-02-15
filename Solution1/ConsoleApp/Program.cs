using Ninject;
using System;
using BusinessLayer.Abstract;
using BusinessLayer.Service;
using DataAccessLayer.Abstract;
using DataAccessLayer.Repository;
using AutoMapper;
using ConsoleApp.AutoMap;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            IKernel ninjectKernel = new StandardKernel();
            ninjectKernel.Bind<IWeatherServise>().To<WeatherService>();
            ninjectKernel.Bind<IWetherRepository>().To<WetherRepositor>();
            ninjectKernel.Bind<IMapper>().To<Mapper>()
                .WithConstructorArgument("configurationProvider", MapperConfig.GetConfiguration());

            IWeatherServise weatherService = ninjectKernel.Get<IWeatherServise>();

            while (true)
            {
                Console.WriteLine("\nPlease, enter city name:");

                var cityName = Console.ReadLine();
                if (String.IsNullOrEmpty(cityName)) continue;
                
                Console.WriteLine(weatherService.GetByCityName(cityName));
            }            
        }
    }
}
