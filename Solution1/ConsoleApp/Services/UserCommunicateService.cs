using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BusinessLayer.Command;
using BusinessLayer.Command.Abstract;
using BusinessLayer.Extensions;
using BusinessLayer.Services.Abstract;
using ConsoleApp.Extensions;
using ConsoleApp.Services.Abstract;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ConsoleApp.Services
{
    public class UserCommunicateService : IUserCommunicateService
    {
        private readonly ILogger _logger;
        private readonly IInvoker _invoker;
        private readonly IWeatherServiсe _weatherServiсe;

        public UserCommunicateService(ILogger logger, IInvoker invoker, IWeatherServiсe weatherServiсe)
        {
            _logger = logger;
            _invoker = invoker;
            _weatherServiсe = weatherServiсe;
        }

        public async Task<bool> CommunicateAsync()
        {
            Console.WriteLine("Select menu item:");
            Console.WriteLine("0 - Exit");
            Console.WriteLine("1 - Get currently weather");
            Console.WriteLine("2 - Get weather for a period of time");

            bool isGoodParse = int.TryParse(Console.ReadLine(), out var pointMenu);

            if (!isGoodParse)
            {
                Console.WriteLine(Constants.Validation.IncorrectValue);
                _logger.LogError($"User entered incorrect value.");
                return true;
            }

            if (pointMenu < 0 || pointMenu > 2)
            {
                Console.WriteLine(Constants.Errors.UnacceptableValue);
                _logger.LogError($"User entered value out of range.");
                return true;
            }

            var isContinue = true;

            try
            {
                switch (pointMenu)
                {
                    case 0:
                        isContinue = false;
                        Console.WriteLine("Сlose the application");
                        break;
                    case 1:
                        await GetCurrentWeatherCommandAsync();
                        break;
                    case 2:
                        await GetForecastByCityNameAsync();
                        break;
                }
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    Console.WriteLine(Constants.Errors.BadCityName);
                    _logger.LogError($"Status code: {ex.StatusCode.GetStringRepresentation()} User entered incorrect city name.");
                }
                else
                {
                    Console.WriteLine(Constants.Errors.RequestError);
                    _logger.LogError($"Status code: {ex.StatusCode.GetStringRepresentation()}");
                }
            }
            catch (ValidationException ex)
            {
                foreach (var error in ex.Errors)
                {
                    _logger.LogError(error.ErrorMessage);
                    Console.WriteLine(error.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(Constants.Errors.UnexpectedError);
                _logger.LogError(ex.Message);
            }

            return isContinue;
        }

        private async Task GetCurrentWeatherCommandAsync()
        {
            Console.WriteLine("Please, enter city name:");
            var command = new CurrentWeatherCommand(_weatherServiсe, Console.ReadLine());
            var result = await _invoker.RunAsync(command);
            Console.WriteLine(result.GetStringRepresentation());
        }

        private async Task GetForecastByCityNameAsync()
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

            var command = new ForecastWeatherCommand(_weatherServiсe, cityName, countDay);
            var result = await _invoker.RunAsync(command);
            Console.WriteLine(result.GetMultiStringRepresentation());
        }
    }
}
