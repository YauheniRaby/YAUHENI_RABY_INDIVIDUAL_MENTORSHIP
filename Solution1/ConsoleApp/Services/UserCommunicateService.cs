using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BusinessLayer.DTOs;
using BusinessLayer.Extensions;
using BusinessLayer.Services.Abstract;
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
        private readonly IList<ICommand> _commands;
        private readonly IValidator<DataForWeatherRequestDTO> _validator;

        public UserCommunicateService(ILogger logger, IWeatherServiсe weatherService, IValidator<DataForWeatherRequestDTO> validator)
        {
            _logger = logger;
            _weatherServiсe = weatherService;
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
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
            catch (ValidationException ex)
            {
                Console.WriteLine(Constants.Validation.IncorrectValues);
                foreach (var e in ex.Errors)
                {
                    _logger.LogError($"{DateTime.Now}| {e.PropertyName}: {e.ErrorMessage}");
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
