using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BusinessLayer.Extensions;
using BusinessLayer.Services.Abstract;
using ConsoleApp.Services.Abstract;
using Microsoft.Extensions.Logging;
using static ConsoleApp.Constants;

namespace ConsoleApp.Services
{
    public class UserCommunicateService : IUserCommunicateService
    {
        private readonly ILogger _logger;
        private readonly IWeatherServiсe _weatherServiсe;

        public UserCommunicateService(ILogger logger, IWeatherServiсe weatherService)
        {
            _logger = logger;
            _weatherServiсe = weatherService;
        }

        public async Task<bool> CommunicateAsync()
        {
            Console.WriteLine("Please, enter city name:");

            var cityName = Console.ReadLine();

            if (cityName.ToUpper() == Commands.Exit)
            {
                return false;
            }

            if (string.IsNullOrEmpty(cityName))
            {
                Console.WriteLine(Validation.EmptyCityName);
                _logger.LogInformation("The user entered an empty city name. Try again");
                return true;
            }

            var weatherRepresentation = (string)default;
            try
            {
                weatherRepresentation = (await _weatherServiсe.GetByCityNameAsync(cityName)).GetStringRepresentation();
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    Console.WriteLine(Errors.BadCityName);
                    _logger.LogError($"{DateTime.Now}| Status code: {(int)HttpStatusCode.NotFound} {HttpStatusCode.NotFound}. User entered incorrect city name.");
                    return true;
                }
                else
                {
                    Console.WriteLine(Errors.RequestError);
                    _logger.LogError($"{DateTime.Now}| Status code: {(int)ex.StatusCode} {ex.StatusCode}. {ex.Message}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(Errors.UnexpectedError);
                _logger.LogError($"{DateTime.Now}| {ex.Message}");
                return true;
            }

            Console.WriteLine(weatherRepresentation);
            return true;
        }
    }
}
