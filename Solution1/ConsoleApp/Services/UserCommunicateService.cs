using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BusinessLayer.Extensions;
using BusinessLayer.Services.Abstract;
using ConsoleApp.Command.Abstract;
using ConsoleApp.Extensions;
using ConsoleApp.Services.Abstract;
using Microsoft.Extensions.Logging;

namespace ConsoleApp.Services
{
    public class UserCommunicateService : IUserCommunicateService
    {
        private readonly ILogger _logger;
        private readonly IWeatherServiсe _weatherServiсe;
        private readonly IList<ICommand> _commands;

        public UserCommunicateService(ILogger logger, IWeatherServiсe weatherService)
        {
            _logger = logger;
            _weatherServiсe = weatherService;
            _commands = new List<ICommand>();
            _commands.FillCommands(this);
        }

        public async Task MenuAsync()
        {
            Console.WriteLine("Select menu item:");
            Console.WriteLine("0 - Get currently weather");
            Console.WriteLine("1 - Get weather for a period of time");
            Console.WriteLine("2 - Exit");

            var i = int.Parse(Console.ReadLine());

            try
            {
                await _commands[i].Execute();
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    Console.WriteLine(Constants.Errors.BadCityName);
                    _logger.LogError($"{DateTime.Now}| Status code: {(int)HttpStatusCode.NotFound} {HttpStatusCode.NotFound}. User entered incorrect city name.");
                }
                else
                {
                    Console.WriteLine(Constants.Errors.RequestError);
                    _logger.LogError($"{DateTime.Now}| Status code: {(int)ex.StatusCode} {ex.StatusCode}. {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(Constants.Errors.UnexpectedError);
                _logger.LogError($"{DateTime.Now}| {ex.Message}");
            }
        }

        public async Task GetCurrentlyWeatherAsync()
        {
            var cityName = string.Empty;
            Console.WriteLine("Please, enter city name:");

            do
            {
                cityName = Console.ReadLine();
                if (!string.IsNullOrEmpty(cityName))
                {
                    break;
                }

                Console.WriteLine(Constants.Validation.EmptyCityName);
                _logger.LogInformation("The user entered an empty city name");
            }
            while (true);

            var weather = await _weatherServiсe.GetByCityNameAsync(cityName);

            Console.WriteLine(weather.GetStringRepresentation());
        }

        public async Task GetForecastByCityNameAsync()
        {
            var cityName = string.Empty;
            Console.WriteLine("Please, enter city name:");

            do
            {
                cityName = Console.ReadLine();
                if (!string.IsNullOrEmpty(cityName))
                {
                    break;
                }

                Console.WriteLine(Constants.Validation.EmptyCityName);
                _logger.LogInformation("The user entered an empty city name");
            }
            while (true);

            Console.WriteLine("Please, enter count day:");
            var countDay = Convert.ToInt32(Console.ReadLine());
            var weather = await _weatherServiсe.GetForecastByCityNameAsync(cityName, countDay);
            weather.FillCommentByTemp();
            Console.WriteLine(weather.GetMultiStringRepresentation());
        }
    }
}
