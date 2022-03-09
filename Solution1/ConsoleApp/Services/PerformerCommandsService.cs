using System;
using System.Threading.Tasks;
using BusinessLayer.Extensions;
using BusinessLayer.Services.Abstract;
using ConsoleApp.Services.Abstract;
using Microsoft.Extensions.Logging;

namespace ConsoleApp.Services
{
    public class PerformerCommandsService : IPerformerCommandsService
    {
        private readonly ILogger _logger;
        private readonly IWeatherServiсe _weatherServiсe;

        public PerformerCommandsService(ILogger logger, IWeatherServiсe weatherService)
        {
            _logger = logger;
            _weatherServiсe = weatherService;
        }

        public async Task GetCurrentWeatherAsync()
        {
            Console.WriteLine("Please, enter city name:");
            var weather = await _weatherServiсe.GetByCityNameAsync(Console.ReadLine());

            Console.WriteLine(weather.GetStringRepresentation());
        }

        public async Task GetForecastByCityNameAsync()
        {
            Console.WriteLine("Please, enter city name:");
            string cityName = Console.ReadLine();

            Console.WriteLine("Please, enter count day:");
            int countDay;

            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out var days))
                {
                    countDay = days;
                    break;
                }

                Console.WriteLine(Constants.Validation.IncorrectValue);
                _logger.LogError($"User entered incorrect value for 'countDay'.");
            }

            var weather = await _weatherServiсe.GetForecastByCityNameAsync(cityName, countDay);
            Console.WriteLine(weather.GetMultiStringRepresentation());
        }

        public Task CloseApplication()
        {
            Console.WriteLine(Constants.Notifications.CloseApp);
            return Task.CompletedTask;
        }
    }
}
