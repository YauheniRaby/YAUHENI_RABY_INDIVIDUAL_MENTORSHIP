using System;
using System.Configuration;
using System.Linq;
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

        public async Task GetBestWeatherAsync()
        {
            Console.WriteLine("Please, enter array city name (separator symbal - ',') :");
            var stringCityNames = Console.ReadLine();
            if (string.IsNullOrEmpty(stringCityNames))
            {
                Console.WriteLine(Constants.Validation.IncorrectValue);
                _logger.LogError($"User entered incorrect value for 'countDay'.");
                return;
            }

            var listCityNames = stringCityNames.Split(',').Select(cityName => cityName.Trim());
            var weatherResponsesDTO = await _weatherServiсe.GetWeatherByArrayCityNameAsync(listCityNames);
            var bestWeather = weatherResponsesDTO.Where(w => w.IsSuccessfulRequest).OrderByDescending(w => w.Temp).FirstOrDefault();

            if (bestWeather != null)
            {
                Console.WriteLine($"City with the highest temperature {bestWeather.Temp} C: {bestWeather.CityName}.");
            }

            var dictionaryWeatherResponsesDTO = weatherResponsesDTO.GroupBy(w => w.IsSuccessfulRequest).ToDictionary(k => k.Key, v => v.ToList());
            int countSuccessResponse = dictionaryWeatherResponsesDTO.TryGetValue(true, out var successRequest) ? successRequest.Count() : 0;
            int countFailResponse = dictionaryWeatherResponsesDTO.TryGetValue(false, out var failRequest) ? failRequest.Count() : 0;

            Console.WriteLine
                ($"Successful request count: {countSuccessResponse}, " +
                $"failed: {countFailResponse}.");

            if (Convert.ToBoolean(ConfigurationManager.AppSettings["isDebugMode"]))
            {
                if (countSuccessResponse > 0)
                {
                    Console.WriteLine(dictionaryWeatherResponsesDTO[true].GetRepresentationSuccessResponse());
                }

                if (countFailResponse > 0)
                {
                    Console.WriteLine(dictionaryWeatherResponsesDTO[false].GetRepresentationFailResponse());
                }
            }
        }
    }
}
