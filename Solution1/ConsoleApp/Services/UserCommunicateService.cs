﻿using System;
using System.Net;
using System.Net.Http;
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
        private readonly IInvoker _invoker;
        private readonly ICloseApplicationService _closeApplicationService;

        public UserCommunicateService(ILogger logger, IWeatherServiсe weatherService, IInvoker invoker, ICloseApplicationService closeApplicationService)
        {
            _logger = logger;
            _weatherServiсe = weatherService;
            _invoker = invoker;
            _closeApplicationService = closeApplicationService;
        }

        public async Task<bool> CommunicateAsync()
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
                return true;
            }

            if (pointMenu < 0 || pointMenu > 2)
            {
                Console.WriteLine(Constants.Errors.UnacceptableValue);
                _logger.LogError($"User entered value out of range.");
                return true;
            }

            switch (pointMenu)
            {
                case 0:
                    _invoker.SetCommand(new CurrentWeatherCommand(this));
                    break;
                case 1:
                    _invoker.SetCommand(new ForecastWeatherCommand(this));
                    break;
                case 2:
                    _invoker.SetCommand(new ExitCommand(_closeApplicationService));
                    break;
            }

            var result = true;

            try
            {
                result = await _invoker.RunAsync();
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

            return result;
        }

        public async Task<bool> GetCurrentWeatherAsync()
        {
            Console.WriteLine("Please, enter city name:");
            var weather = await _weatherServiсe.GetByCityNameAsync(Console.ReadLine());

            Console.WriteLine(weather.GetStringRepresentation());
            return true;
        }

        public async Task<bool> GetForecastByCityNameAsync()
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

                Console.WriteLine(Constants.Errors.IncorrectValue);
                _logger.LogError($"User entered incorrect value for 'countDay'.");
            }

            var weather = await _weatherServiсe.GetForecastByCityNameAsync(cityName, countDay);
            Console.WriteLine(weather.GetMultiStringRepresentation());
            return true;
        }
    }
}
