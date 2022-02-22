using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BusinessLayer.Abstract;
using BusinessLayer.Extensions;
using ConsoleApp.Service.Abstract;
using Microsoft.Extensions.Logging;
using static ConsoleApp.Constants;

namespace ConsoleApp.Service
{
    public class UserCommunicationService : IUserCommunicationService
    {
        private readonly ILogger _logger;
        private readonly IWeatherServiсe _weatherServiсe;

        public UserCommunicationService(ILogger logger, IWeatherServiсe weatherService)
        {
            _logger = logger;
            _weatherServiсe = weatherService;
        }

        public async Task Communication()
        {
            Console.WriteLine("Please, enter city name:");

            var cityName = Console.ReadLine();

            if (string.IsNullOrEmpty(cityName))
            {
                Console.WriteLine(Validation.EmptyCityName);
                _logger.LogInformation("The user entered an empty city name. Try again");
                return;
            }

            try
            {
                Console.WriteLine((await _weatherServiсe.GetByCityNameAsync(cityName)).GetStringRepresentation());
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    Console.WriteLine(Errors.BadCityName);
                    _logger.LogError($"{DateTime.Now}| Status code: {(int)HttpStatusCode.NotFound} {HttpStatusCode.NotFound}. User entered incorrect city name.");
                }
                else
                {
                    Console.WriteLine(Errors.RequestError);
                    _logger.LogError($"{DateTime.Now}| Status code: {(int)ex.StatusCode} {ex.StatusCode}. {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(Errors.UnexpectedError);
                _logger.LogError($"{DateTime.Now}| {ex.Message}");
            }
        }
    }
}
