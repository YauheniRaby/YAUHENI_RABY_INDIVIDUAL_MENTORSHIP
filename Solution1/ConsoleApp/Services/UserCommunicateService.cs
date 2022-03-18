using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BusinessLayer.Command;
using BusinessLayer.Command.Abstract;
using BusinessLayer.Configuration.Abstract;
using BusinessLayer.DTOs;
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
        private readonly IConfig _config;

        public UserCommunicateService(ILogger logger, IInvoker invoker, IWeatherServiсe weatherServiсe, IConfig config)
        {
            _logger = logger;
            _invoker = invoker;
            _weatherServiсe = weatherServiсe;
            _config = config;
        }

        public async Task<bool> CommunicateAsync()
        {
            Console.WriteLine("Select menu item:");
            Console.WriteLine("0 - Exit");
            Console.WriteLine("1 - Get currently weather");
            Console.WriteLine("2 - Get weather for a period of time");
            Console.WriteLine("3 - Get best weather for array cities");

            var isGoodParse = int.TryParse(Console.ReadLine(), out var pointMenu);

            if (!isGoodParse)
            {
                Console.WriteLine(Constants.Validation.IncorrectValue);
                _logger.LogError($"User entered incorrect value.");
                return true;
            }

            if (pointMenu < 0 || pointMenu > 3)
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
                    case 3:
                        await GetBestWeatherByArrayCityNameAsync();
                        break;
                }
            }
            catch (HttpRequestException ex)
            {
                var statusCodeRepresentation = "No Code";
                if (ex.StatusCode.HasValue)
                {
                    statusCodeRepresentation = ex.StatusCode.Value.GetStringRepresentation();
                }

                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    Console.WriteLine(Constants.Errors.BadCityName);
                    _logger.LogError($"Status code: {statusCodeRepresentation} User entered incorrect city name.");
                }
                else
                {
                    Console.WriteLine(Constants.Errors.RequestError);
                    _logger.LogError($"Status code: {statusCodeRepresentation}");
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

        private async Task GetBestWeatherByArrayCityNameAsync()
        {
            Console.WriteLine("Please, enter array city name (separator symbal - ',') :");
            var arrayCityNames = Console.ReadLine();
            if (string.IsNullOrEmpty(arrayCityNames))
            {
                Console.WriteLine(Constants.Validation.IncorrectValue);
                _logger.LogError($"User entered incorrect value for 'PeriodOfDays'.");
                return;
            }

            var command = new BestWeatherCommand(_weatherServiсe, arrayCityNames.Split(',').Select(cityName => cityName.Trim()));
            var dictionaryWeatherResponsesDTO = await _invoker.RunAsync(command);

            var countSuccessResponse = dictionaryWeatherResponsesDTO.TryGetValue(true, out var successfulWeatherResponses) ? successfulWeatherResponses.Count() : 0;
            var countFailResponse = dictionaryWeatherResponsesDTO.TryGetValue(false, out var failedWeatherResponses) ? failedWeatherResponses.Count() : 0;

            if (countSuccessResponse > 0)
            {
                var bestWeather = successfulWeatherResponses.OrderByDescending(w => w.Temp).First();
                Console.WriteLine($"City with the highest temperature {bestWeather.Temp} C: {bestWeather.CityName}. " +
                    $"Successful request count: {countSuccessResponse}, failed: {countFailResponse}.");
            }
            else
            {
                Console.WriteLine($"Error, no successful requests. Failed requests count: {countFailResponse}");
            }

            if (_config.IsDebugMode)
            {
                ShowDebugInformation(successfulWeatherResponses, "Success case:", nameof(WeatherResponseDTO.Temp));
                ShowDebugInformation(failedWeatherResponses, "On fail:", nameof(WeatherResponseDTO.ErrorMessage));
            }

            return;
        }

        private void ShowDebugInformation(IEnumerable<WeatherResponseDTO> responses, string header, string propertyName)
        {
            Console.WriteLine(
                responses
                    ?.Aggregate(
                        $"{header}",
                        (result, next) => $"{result}{Environment.NewLine}{GetRepresentationResponse(next, propertyName)}"));
        }

        private string GetRepresentationResponse(WeatherResponseDTO weatherResponse, string propertyName)
        {
            var propertyValue = typeof(WeatherResponseDTO).GetProperty(propertyName).GetValue(weatherResponse);

            return $"City: '{weatherResponse.CityName}', {propertyName}: {propertyValue}, Timer: {weatherResponse.LeadTime} ms.";
        }
    }
}
