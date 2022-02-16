using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BusinessLayer.Abstract;
using BusinessLayer.DTOs;
using BusinessLayer.Extension;
using ConsoleApp.Configuration;
using ConsoleApp.Extension;
using Microsoft.Extensions.Logging;
using Ninject;

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
                    Console.WriteLine("City name can't be empty.");
                    log.LogInformation("The user entered an empty city name.");
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
                        Console.WriteLine("Entered incorrect city name. Try one time yet.");
                        log.LogWarning($"{DateTime.Now}| Status code: {(int)HttpStatusCode.NotFound} {HttpStatusCode.NotFound}. User entered incorrect city name.");
                    }
                    else
                    {
                        Console.WriteLine("Request error. Try again later.");
                        log.LogWarning($"{DateTime.Now}| Status code: {(int)ex.StatusCode} {ex.StatusCode}. {ex.Message}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unexpected error. Try one time yet.");
                    log.LogWarning($"{DateTime.Now}| {ex.Message}");
                }
            }
        }
    }
}
