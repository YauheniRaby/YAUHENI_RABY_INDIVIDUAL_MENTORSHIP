using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer.DTOs;
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
                _logger.LogError($"User entered incorrect value for 'PeriodOfDays'.");
            }

            var weather = await _weatherServiсe.GetForecastByCityNameAsync(cityName, countDay);
            Console.WriteLine(weather.GetMultiStringRepresentation());
        }

        public Task CloseApplication()
        {
            Console.WriteLine(Constants.Notifications.CloseApp);
            return Task.CompletedTask;
        }

        public async Task GetBestWeatherAsync()
        {
            Console.WriteLine("Please, enter array city name (separator symbal - ',') :");
            var stringCityNames = Console.ReadLine();
            if (string.IsNullOrEmpty(stringCityNames))
            {
                Console.WriteLine(Constants.Validation.IncorrectValue);
                _logger.LogError($"User entered incorrect value for 'PeriodOfDays'.");
                return;
            }

            var listCityNames = stringCityNames.Split(',').Select(cityName => cityName.Trim());
            var weatherResponsesDTO = await _weatherServiсe.GetWeatherByArrayCityNameAsync(listCityNames);
            var dictionaryWeatherResponsesDTO = weatherResponsesDTO.GroupBy(w => w.IsSuccessfulRequest).ToDictionary(k => k.Key, v => v.ToList());

            var countSuccessResponse = dictionaryWeatherResponsesDTO.TryGetValue(true, out var successfulWeatherResponses) ? successfulWeatherResponses.Count : 0;
            var countFailResponse = dictionaryWeatherResponsesDTO.TryGetValue(false, out var failedWeatherResponses) ? failedWeatherResponses.Count : 0;

            if (countSuccessResponse > 0)
            {
                var bestWeather = successfulWeatherResponses.OrderByDescending(w => w.Temp).FirstOrDefault();
                Console.WriteLine($"City with the highest temperature {bestWeather.Temp} C: {bestWeather.CityName}." +
                    $"Successful request count: {countSuccessResponse}, failed: {countFailResponse}.");
            }
            else
            {
                Console.WriteLine($"Error, no successful requests. Failed requests count: {countFailResponse}");
            }

            if (Convert.ToBoolean(ConfigurationManager.AppSettings["isDebugMode"]))
            {
                ShowDebugInformation(successfulWeatherResponses, failedWeatherResponses);
            }

            return;
        }

        public void ShowDebugInformation(IEnumerable<WeatherResponseDTO> successfulWeatherResponses, IEnumerable<WeatherResponseDTO> failedWeatherResponses)
        {
            if (successfulWeatherResponses != null)
            {
                Console.WriteLine(
                    successfulWeatherResponses
                    .Aggregate(
                        $"Success case:",
                        (result, next) => $"{result}{Environment.NewLine}{next.GetRepresentationSuccessResponse()}"));
            }

            if (failedWeatherResponses != null)
            {
                Console.WriteLine(
                    failedWeatherResponses
                    .Aggregate(
                        $"On fail:",
                        (result, next) => $"{result}{Environment.NewLine}{next.GetRepresentationFailResponse()}"));
            }
        }
    }
}
