using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BusinessLayer.Extensions;
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
        private readonly IInvoker _invoker;
        private readonly IPerformerCommandsService _performerCommandsService;

        public UserCommunicateService(ILogger logger, IInvoker invoker, IPerformerCommandsService performerCommandsService)
        {
            _logger = logger;
            _invoker = invoker;
            _performerCommandsService = performerCommandsService;
        }

        public async Task<bool> CommunicateAsync()
        {
            Console.WriteLine("Select menu item:");
            Console.WriteLine("0 - Exit");
            Console.WriteLine("1 - Get currently weather");
            Console.WriteLine("2 - Get weather for a period of time");

            bool parseResult = int.TryParse(Console.ReadLine(), out var pointMenu);

            if (!parseResult)
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

            var result = true;

            switch (pointMenu)
            {
                case 0:
                    _invoker.SetCommand(new ExitCommand(_performerCommandsService));
                    result = false;
                    break;
                case 1:
                    _invoker.SetCommand(new CurrentWeatherCommand(_performerCommandsService));
                    break;
                case 2:
                    _invoker.SetCommand(new ForecastWeatherCommand(_performerCommandsService));
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
    }
}
