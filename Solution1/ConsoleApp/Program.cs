using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BusinessLayer.Abstract;
using BusinessLayer.Extension;
using ConsoleApp.Configuration;
using ConsoleApp.Extension;
using Microsoft.Extensions.Logging;
using Ninject;
using static ConsoleApp.Constants;

namespace ConsoleApp
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var log = LoggerConfiguration.GetConfiguration<Program>();

            var ninjectKernel = new StandardKernel();
            ninjectKernel.AddServices();

            IWeatherServiсe weatherService = ninjectKernel.Get<IWeatherServiсe>();

            while (true)
            {
                Console.WriteLine("Please, enter city name:");

                var cityName = Console.ReadLine();
                if (string.IsNullOrEmpty(cityName))
                {
                    Console.WriteLine(Validation.User.EmptyCityName);
                    log.LogInformation(Validation.Dev.EmptyCityName);
                    continue;
                }

                try
                {
                    Console.WriteLine((await weatherService.GetByCityNameAsync(cityName)).PrintToString());
                }
                catch (HttpRequestException ex)
                {
                    if (ex.StatusCode == HttpStatusCode.NotFound)
                    {
                        Console.WriteLine(Errors.User.BadCityName);
                        log.LogWarning(
                            string.Format(
                                Errors.Dev.BadCityName,
                                DateTime.Now,
                                (int)HttpStatusCode.NotFound,
                                HttpStatusCode.NotFound, "User entered incorrect city name."));
                    }
                    else
                    {
                        Console.WriteLine(Errors.User.RequestError);
                        log.LogWarning(
                            string.Format(
                                Errors.Dev.RequestError,
                                DateTime.Now,
                                (int)ex.StatusCode,
                                ex.StatusCode,
                                ex.Message));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(Errors.User.UnexpectedError);
                    log.LogWarning(
                        string.Format(
                            Errors.Dev.UnexpectedError,
                            DateTime.Now,
                            ex.Message));
                }
            }
        }
    }
}
