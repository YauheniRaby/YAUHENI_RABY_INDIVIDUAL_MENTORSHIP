using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BusinessLayer.Abstract;
using BusinessLayer.Extensions;
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
            var logger = LoggerConfiguration.GetConfiguration<Program>();

            var ninjectKernel = new StandardKernel();
            ninjectKernel.AddServices();

            var weatherService = ninjectKernel.Get<IWeatherServiсe>();

            while (true)
            {
                Console.WriteLine("Please, enter city name:");

                var cityName = Console.ReadLine();
                if (string.IsNullOrEmpty(cityName))
                {
                    Console.WriteLine(Validation.EmptyCityName);
                    logger.LogInformation("The user entered an empty city name.");
                    continue;
                }

                try
                {
                    Console.WriteLine((await weatherService.GetByCityNameAsync(cityName)).GetStringRepresentation());
                }
                catch (HttpRequestException ex)
                {
                    if (ex.StatusCode == HttpStatusCode.NotFound)
                    {
                        Console.WriteLine(Errors.BadCityName);
                        logger.LogError($"{DateTime.Now}| Status code: {(int)HttpStatusCode.NotFound} {HttpStatusCode.NotFound}. User entered incorrect city name.");
                    }
                    else
                    {
                        Console.WriteLine(Errors.RequestError);
                        logger.LogError($"{DateTime.Now}| Status code: {(int)ex.StatusCode} {ex.StatusCode}. {ex.Message}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(Errors.UnexpectedError);
                    logger.LogError($"{DateTime.Now}| {ex.Message}");
                }
            }
        }
    }
}
