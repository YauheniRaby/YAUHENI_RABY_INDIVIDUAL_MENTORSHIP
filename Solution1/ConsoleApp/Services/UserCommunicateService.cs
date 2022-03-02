using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BusinessLayer.DTOs;
using BusinessLayer.Extensions;
using BusinessLayer.Services.Abstract;
using ConsoleApp.Command;
using ConsoleApp.Command.Abstract;
using ConsoleApp.Extensions;
using ConsoleApp.Services.Abstract;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace ConsoleApp.Services
{
    public class UserCommunicateService : IUserCommunicateService
    {
        private readonly ILogger _logger;
        private readonly IWeatherServiсe _weatherServiсe;
        private readonly IValidator<ForecastWeatherRequestDTO> _validator;
        private readonly Invoker _invoker;

        public UserCommunicateService(ILogger logger, IWeatherServiсe weatherService, IValidator<ForecastWeatherRequestDTO> validator)
        {
            _logger = logger;
            _weatherServiсe = weatherService;
            _validator = validator;
            _invoker = new Invoker();
        }

        public async Task StartUpApplicationAsync()
        {
            Console.WriteLine("Select menu item:");
            Console.WriteLine("0 - Get currently weather");
            Console.WriteLine("1 - Get weather for a period of time");
            Console.WriteLine("2 - Exit");

            bool parseResult = int.TryParse(Console.ReadLine(), out var pointMenu);

            if (!parseResult)
            {
                Console.WriteLine(Constants.Errors.IncorrectValue);
                _logger.LogError($"User entered incorrect value.");
                return;
            }

            if (pointMenu < 0 || pointMenu > 2)
            {
                Console.WriteLine(Constants.Errors.IncorrectValue);
                _logger.LogError($"User entered value out of range.");
                return;
            }

            switch (pointMenu)
            {
                case 0:
                    _invoker.SetCommand(new CurrentlyWeatherCommand(this));
                    break;
                case 1:
                    _invoker.SetCommand(new ForecastWeatherCommand(this));
                    break;
                case 2:
                    _invoker.SetCommand(new ExitCommand());
                    break;
            }

            try
            {
                await _invoker.RunAsync();
            }
            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    Console.WriteLine(Constants.Errors.BadCityName);
                    _logger.LogError($"Status code: {(int)ex.StatusCode} {ex.StatusCode}. User entered incorrect city name.");
                }
                else
                {
                    Console.WriteLine(Constants.Errors.RequestError);
                    _logger.LogError($"Status code: {(int)ex.StatusCode} {ex.StatusCode}. {ex.Message}");
                }
            }
            catch (ValidationException ex)
            {
                foreach (var error in ex.Errors)
                {
                    _logger.LogError(error.ErrorMessage);

                    if (error.PropertyName == nameof(ForecastWeatherDTO.CityName))
                    {
                        Console.WriteLine(Constants.Validation.IncorrectCityName);
                    }

                    if (error.PropertyName == nameof(ForecastWeatherDTO.WeatherForPeriod))
                    {
                        Console.WriteLine(Constants.Validation.IncorrectPeriod);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(Constants.Errors.UnexpectedError);
                _logger.LogError(ex.Message);
            }
        }

        public async Task GetCurrentlyWeatherAsync()
        {
            var weatherRequest = new ForecastWeatherRequestDTO();

            Console.WriteLine("Please, enter city name:");
            weatherRequest.CityName = Console.ReadLine();

            var weather = await _weatherServiсe.GetByCityNameAsync(weatherRequest);
            Console.WriteLine(weather.GetStringRepresentation());
        }

        public async Task GetForecastByCityNameAsync()
        {
            var weatherRequest = new ForecastWeatherRequestDTO();

            Console.WriteLine("Please, enter city name:");
            weatherRequest.CityName = Console.ReadLine();

            Console.WriteLine("Please, enter count day:");
            bool parseResult = int.TryParse(Console.ReadLine(), out var countDay);

            if (parseResult)
            {
                weatherRequest.PeriodOfDays = countDay;
            }

            var weather = await _weatherServiсe.GetForecastByCityNameAsync(weatherRequest);
            weather.FillCommentByTemp();
            Console.WriteLine(weather.GetMultiStringRepresentation());
        }
    }
}
