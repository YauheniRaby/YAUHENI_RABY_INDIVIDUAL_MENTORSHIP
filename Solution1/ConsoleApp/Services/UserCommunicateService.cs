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
        private readonly IValidator<DataForWeatherRequestDTO> _validator;
        private readonly Invoker _invoker;


        public UserCommunicateService(ILogger logger, IWeatherServiсe weatherService, IValidator<DataForWeatherRequestDTO> validator)
        {
            _logger = logger;
            _weatherServiсe = weatherService;
            _validator = validator;
            _invoker = new Invoker();
        }

        public async Task ShowMenuAsync()
        {
            Console.WriteLine("Select menu item:");
            Console.WriteLine("0 - Get currently weather");
            Console.WriteLine("1 - Get weather for a period of time");
            Console.WriteLine("2 - Exit");

            bool rarseResult = int.TryParse(Console.ReadLine(), out var pointMenu);
            if (rarseResult && pointMenu >= 0 && pointMenu <= 2)
            {
                try
                {
                    switch (pointMenu)
                    {
                        case 0:
                            _invoker.SetCommand(new CurrentlyWeatherCommand(this));
                            await _invoker.RunAsync();
                            break;
                        case 1:
                            _invoker.SetCommand(new ForecastWeatherCommand(this));
                            await _invoker.RunAsync();
                            break;
                        case 2:
                            _invoker.SetCommand(new ExitCommand());
                            await _invoker.RunAsync();
                            break;
                    }
                }
                catch (HttpRequestException ex)
                {
                    if (ex.StatusCode == HttpStatusCode.NotFound)
                    {
                        Console.WriteLine(Constants.Errors.BadCityName);
                        _logger.LogError($"Status code: {(int)HttpStatusCode.NotFound} {HttpStatusCode.NotFound}. User entered incorrect city name.");
                    }
                    else
                    {
                        Console.WriteLine(Constants.Errors.RequestError);
                        _logger.LogError($"Status code: {(int)ex.StatusCode} {ex.StatusCode}. {ex.Message}");
                    }
                }
                catch (ValidationException ex)
                {
                    Console.WriteLine(Constants.Validation.IncorrectValues);
                    foreach (var error in ex.Errors)
                    {
                        _logger.LogError(error.ErrorMessage);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(Constants.Errors.UnexpectedError);
                    _logger.LogError(ex.Message);
                }
            }
        }

        public async Task GetCurrentlyWeatherAsync()
        {
            var weatherRequest = new DataForWeatherRequestDTO();

            Console.WriteLine("Please, enter city name:");
            weatherRequest.CityName = Console.ReadLine();

            var validationResult = await _validator.ValidateAsync(weatherRequest, options => options.IncludeRuleSets("CityName"));
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var weather = await _weatherServiсe.GetByCityNameAsync(weatherRequest.CityName);
            Console.WriteLine(weather.GetStringRepresentation());
        }

        public async Task GetForecastByCityNameAsync()
        {
            var weatherRequest = new DataForWeatherRequestDTO();

            Console.WriteLine("Please, enter city name:");
            weatherRequest.CityName = Console.ReadLine();

            Console.WriteLine("Please, enter count day:");
            weatherRequest.PeriodOfDays = Convert.ToInt32(Console.ReadLine());

            var validationResult = await _validator.ValidateAsync(weatherRequest, options => options.IncludeAllRuleSets());
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var weather = await _weatherServiсe.GetForecastByCityNameAsync(weatherRequest);
            weather.FillCommentByTemp();
            Console.WriteLine(weather.GetMultiStringRepresentation());
        }
    }
}
