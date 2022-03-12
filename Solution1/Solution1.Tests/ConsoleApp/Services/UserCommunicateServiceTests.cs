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
using ConsoleApp.Services.Abstract;
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
            Console.SetIn(new StringReader(string.Format("2{0}{1}{0}{2}", Environment.NewLine, cityName, countDays)));

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
            var expected = Menu.GetMenuRepresentation() + Environment.NewLine +
                "Please, enter city name:" + Environment.NewLine +
                "Please, enter count day:" + Environment.NewLine +
                "Minsk weather forecast:" + Environment.NewLine +
                "Day 0 (January 10, 2022): 10,0 C. It's fresh." + Environment.NewLine +
                "Day 1 (January 11, 2022): 11,0 C. It's fresh." + Environment.NewLine;
            
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
            Console.SetIn(new StringReader(string.Format("1{0}{1}{0}", Environment.NewLine, cityName)));

            var Weather = new WeatherDTO()
            {
                CityName = cityName,
                Temp = 10, 
                Comment = "It's fresh"
            };

            _invokerMock
                .Setup(invoker => invoker.RunAsync(It.IsAny<CurrentWeatherCommand>()))
                .ReturnsAsync(Weather);

            //Act
            await _userCommunicationService.CommunicateAsync();

            //Assert            
            var expected = Menu.GetMenuRepresentation() + Environment.NewLine +
                "Please, enter city name:" + Environment.NewLine +
                "In Minsk 10 C. It's fresh" + Environment.NewLine;

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
            Console.SetIn(new StringReader(string.Format("0{0}", Environment.NewLine)));

            //Act
            await _userCommunicationService.CommunicateAsync();

            //Assert
            Assert.Matches(pattern, consoleOutput.ToString());;
        }

        [Theory]
        [InlineData("Minsk", "Unexpected error. Try one time yet.\r\n", null, false)]
        [InlineData(default, "Validation error.", null, true)]
        [InlineData("Minsk", "Request error. Try again later.\r\n", HttpStatusCode.BadRequest, false)]
        [InlineData("AAA", "Entered incorrect city name. Try one time yet.\r\n", HttpStatusCode.NotFound, false)]
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
            Console.SetIn(new StringReader(string.Format("1{0}{1}{0}", Environment.NewLine, cityName)));

            //Act
            await _userCommunicationService.CommunicateAsync();

            //Assert
            var expected = $"{Menu.GetMenuRepresentation()}" + Environment.NewLine +
                $"Please, enter city name:" + Environment.NewLine +
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
            Console.SetIn(new StringReader(string.Format("{0}{1}", -1, Environment.NewLine)));

            var unacceptableValue = "Value out of range. Try one time yet.";

            //Act
            await _userCommunicationService.CommunicateAsync();

            //Assert
            var expected = string.Format("{0}{1}{2}{1}", Menu.GetMenuRepresentation(), Environment.NewLine, unacceptableValue);
            Assert.Equal(expected, consoleOutput.ToString());
        }

        [Fact]
        public async Task CommunicateAsync_EnterIncorrectValueValueForMenu_ShowNotice()
        {
            // Arrange
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);
            Console.SetIn(new StringReader(string.Format(Environment.NewLine)));

            var incorrectValue = "Incorrect value. Try one time yet.";

            //Act
            await _userCommunicationService.CommunicateAsync();

            //Assert
            var expected = string.Format("{0}{1}{2}{1}", Menu.GetMenuRepresentation(), Environment.NewLine, incorrectValue);
            Assert.Equal(expected, consoleOutput.ToString());
        }
    }
}