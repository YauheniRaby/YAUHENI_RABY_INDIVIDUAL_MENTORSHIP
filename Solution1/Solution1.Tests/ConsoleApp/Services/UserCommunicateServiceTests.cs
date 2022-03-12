using ConsoleApp.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using FluentValidation;
using FluentValidation.Results;
using BusinessLayer.Command.Abstract;
using BusinessLayer.Command;
using BusinessLayer.Services.Abstract;
using BusinessLayer.DTOs;
using Weather.Tests.Infrastructure;

namespace Weather.Tests.ConsoleApp.Services
{
    public class UserCommunicateServiceTests
    {
        private readonly UserCommunicateService _userCommunicationService;
        private readonly Mock<IWeatherServiсe> _weatherServiceMock;
        private readonly Mock<ILogger> _loggerMock;
        private readonly Mock<IInvoker> _invokerMock;
        
        public static IEnumerable<object[]> ParamsForExceptionHandlingTest =>
            new List<object[]>
            {
                new object[] { "Minsk", $"Unexpected error. Try one time yet.{Environment.NewLine}", null, false },
                new object[] { default, "Validation error.", null, true },
                new object[] { "Minsk", $"Request error. Try again later.{Environment.NewLine}", HttpStatusCode.BadRequest, false },
                new object[] { "AAA", $"Entered incorrect city name. Try one time yet.{Environment.NewLine}", HttpStatusCode.NotFound, false },
            };

        public UserCommunicateServiceTests()
        {
            _loggerMock = new Mock<ILogger>();
            _invokerMock = new Mock<IInvoker>();
            _weatherServiceMock = new Mock<IWeatherServiсe>();
            _userCommunicationService = new UserCommunicateService(_loggerMock.Object, _invokerMock.Object, _weatherServiceMock.Object);
        }

        [Fact]
        public async Task CommunicateAsync_GetForecastWeather_ShowForecastWeather()
        {
            // Arrange
            var cityName = "Minsk";
            var countDays = 2;
            var dateStartForecast = new DateTime(2022, 01, 10);
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);
            Console.SetIn(new StringReader($"2{Environment.NewLine}{cityName}{Environment.NewLine}{countDays}"));

            var forecastWeather = new ForecastWeatherDTO()
            {
                CityName = cityName,
                WeatherForPeriod = new List<WeatherForDateDTO>
                {
                    new WeatherForDateDTO() { DateTime = dateStartForecast, Temp = 10, Comment= "It's fresh."},
                    new WeatherForDateDTO() { DateTime = dateStartForecast.AddDays(1), Temp = 11, Comment= "It's fresh."}
                }
            };

            _invokerMock
                .Setup(invoker => invoker.RunAsync(It.IsAny<ForecastWeatherCommand>()))
                .ReturnsAsync(forecastWeather);

            //Act
            await _userCommunicationService.CommunicateAsync();

            //Assert            
            var expected = $"{Menu.GetMenuRepresentation()}{Environment.NewLine}" +
                $"Please, enter city name:{Environment.NewLine}" +
                $"Please, enter count day:{Environment.NewLine}" +
                $"Minsk weather forecast:{Environment.NewLine}" +
                $"Day 0 (January 10, 2022): 10,0 C. It's fresh.{Environment.NewLine}" +
                $"Day 1 (January 11, 2022): 11,0 C. It's fresh.{Environment.NewLine}";


            _invokerMock.Verify(i => i.RunAsync(It.IsAny<ForecastWeatherCommand>()));
            Assert.Equal(expected, consoleOutput.ToString());
        }

        [Fact]
        public async Task CommunicateAsync_GetCurrentWeather_ShowCurrentWeather()
        {
            // Arrange
            var cityName = "Minsk";
            var dateStartForecast = new DateTime(2022, 01, 10);
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);
            Console.SetIn(new StringReader($"1{Environment.NewLine}{cityName}{Environment.NewLine}"));

            var Weather = new WeatherDTO()
            {
                CityName = cityName,
                Temp = 10,
                Comment = "It's fresh."
            };

            _invokerMock
                .Setup(invoker => invoker.RunAsync(It.IsAny<CurrentWeatherCommand>()))
                .ReturnsAsync(Weather);

            //Act
            await _userCommunicationService.CommunicateAsync();

            //Assert            
            var expected = $"{Menu.GetMenuRepresentation()}{Environment.NewLine}" +
                $"Please, enter city name:{Environment.NewLine}" +
                $"In Minsk 10 C. It's fresh.{Environment.NewLine}";

            _invokerMock.Verify(i => i.RunAsync(It.IsAny<CurrentWeatherCommand>()));
            Assert.Equal(expected, consoleOutput.ToString());
        }

        [Fact]
        public async Task CloseApplication_Success()
        {
            // Arrange
            var pattern = $"Сlose the application{Environment.NewLine}$";
            
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);
            Console.SetIn(new StringReader($"0{Environment.NewLine}"));

            //Act
            await _userCommunicationService.CommunicateAsync();

            //Assert
            Assert.Matches(pattern, consoleOutput.ToString());
        }

        [Theory]
        [MemberData(nameof(ParamsForExceptionHandlingTest))]
        public async Task Communicate_EnterCityName_HandlingExceptionAndShowNoticeAsync(string cityName, string message, HttpStatusCode? statusCode, bool IsValidateEror)
        {
            // Arrange
            var exception = statusCode.HasValue ? new HttpRequestException(null, null, statusCode) :
                            IsValidateEror ? new ValidationException(new List<ValidationFailure>() { new ValidationFailure("CityName", message) }) : new Exception();

            _invokerMock
                .Setup(invoker =>
                    invoker
                    .RunAsync(It.IsAny<CurrentWeatherCommand>()))
                .Throws(exception);

            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);
            Console.SetIn(new StringReader($"1{Environment.NewLine}{cityName}{Environment.NewLine}"));

            //Act
            await _userCommunicationService.CommunicateAsync();

            //Assert
            var expected = $"{Menu.GetMenuRepresentation()}{Environment.NewLine}" +
                $"Please, enter city name:{Environment.NewLine}" +
                $"{message}";

            if (IsValidateEror)
            {
                expected += Environment.NewLine;
            }

            Assert.Equal(expected, consoleOutput.ToString());
        }

        [Fact]
        public async Task CommunicateAsync_EnterUnacceptableValueForMenu_ShowNotice()
        {
            // Arrange
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);
            Console.SetIn(new StringReader($"-1{Environment.NewLine}"));

            var unacceptableValue = "Value out of range. Try one time yet.";

            //Act
            await _userCommunicationService.CommunicateAsync();

            //Assert
            var expected = $"{Menu.GetMenuRepresentation()}{Environment.NewLine}{unacceptableValue}{Environment.NewLine}";
            Assert.Equal(expected, consoleOutput.ToString());
        }

        [Fact]
        public async Task CommunicateAsync_EnterIncorrectValueValueForMenu_ShowNotice()
        {
            // Arrange
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);
            Console.SetIn(new StringReader(Environment.NewLine));

            var incorrectValue = "Incorrect value. Try one time yet.";

            //Act
            await _userCommunicationService.CommunicateAsync();

            //Assert
            var expected = $"{Menu.GetMenuRepresentation()}{Environment.NewLine}{incorrectValue}{Environment.NewLine}";
            Assert.Equal(expected, consoleOutput.ToString());
        }
    }
}