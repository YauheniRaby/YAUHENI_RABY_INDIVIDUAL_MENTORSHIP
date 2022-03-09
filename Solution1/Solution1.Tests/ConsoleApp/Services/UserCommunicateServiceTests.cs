using BusinessLayer.DTOs;
using BusinessLayer.Services.Abstract;
using ConsoleApp.Command;
using ConsoleApp.Command.Abstract;
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
using Weather.Tests.Infrastructure;

namespace Weather.Tests.ConsoleApp.Services
{
    public class UserCommunicateServiceTests
    {
        private readonly SelectionCommandService _userCommunicationService;
        private readonly Mock<IWeatherServiсe> _weatherServiceMock;
        private readonly Mock<ILogger> _loggerMock;
        private readonly Mock<IInvoker> _invokerMock;
        private readonly Mock<ICloseApplicationService> _closeApplicationService;

        public UserCommunicateServiceTests()
        {
            _loggerMock = new Mock<ILogger>();
            _weatherServiceMock = new Mock<IWeatherServiсe>();
            _invokerMock = new Mock<IInvoker>();
            _closeApplicationService = new Mock<ICloseApplicationService>();

            _userCommunicationService = new SelectionCommandService(_loggerMock.Object, _weatherServiceMock.Object, _invokerMock.Object, _closeApplicationService.Object);
        }

        [Theory]
        [InlineData(0, typeof(ExitCommand))]
        [InlineData(1, typeof(CurrentWeatherCommand))]
        [InlineData(2, typeof(ForecastWeatherCommand))]
        public async Task CommunicateAsync_EnterPointMenu_InvokerSetRightCommand(int point, Type type)
        {
            // Arrange
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);
            Console.SetIn(new StringReader(string.Format("{0}{1}", point, Environment.NewLine)));
            
            //Act
            await _userCommunicationService.CommunicateAsync();

            //Assert
            _invokerMock.Verify(i => i.SetCommand(It.Is<ICommand>(x => x.GetType() == type)));
            _invokerMock.Verify(i => i.RunAsync());
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
                    .RunAsync())
                .Throws(exception);

            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            Console.SetIn(new StringReader(string.Format("1{0}{1}{0}", Environment.NewLine, cityName)));

            //Act
            await _userCommunicationService.CommunicateAsync();

            //Assert
            var expected = $"{Menu.GetMenuRepresentation()}\r\n{message}";
            if (IsValidateEror)
            {
                expected += "\r\n";
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
            var expected = string.Format("{0}\r\n{1}{2}", Menu.GetMenuRepresentation(), unacceptableValue, Environment.NewLine);
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
            var expected = string.Format("{0}\r\n{1}{2}", Menu.GetMenuRepresentation(), incorrectValue, Environment.NewLine);
            Assert.Equal(expected, consoleOutput.ToString());
        }

        [Fact]
        public async Task GetCurrentWeatherAsync_EnterCityName_ShowStringRepresentationAsync()
        {
            // Arrange
            var cityName = "Minsk";
            var weatherDto = new WeatherDTO() { CityName = cityName, Temp = 10, Comment = "It's fresh." };
            _weatherServiceMock
                .Setup(weatherApiService =>
                    weatherApiService
                    .GetByCityNameAsync(cityName))
                .ReturnsAsync(weatherDto);

            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            Console.SetIn(new StringReader(string.Format("Minsk{0}", Environment.NewLine)));

            //Act
            await _userCommunicationService.GetCurrentWeatherAsync();

            //Assert
            var expected = string.Format("Please, enter city name:{0}In Minsk 10 C. It's fresh.{0}", Environment.NewLine);
            Assert.Equal(expected, consoleOutput.ToString());
        }

        [Fact]
        public async Task GetForecastByCityNameAsync_EnterCityNameAndCountDay_ShowMultiStringRepresentationAsync()
        {
            // Arrange
            var cityName = "Minsk";
            var countDay = 3;

            var weatherForPeriod = new List<WeatherForDateDTO>()
            {
                new WeatherForDateDTO() { DateTime = new DateTime(2022, 12, 10), Temp = -1, Comment = "Dress warmly." },
                new WeatherForDateDTO() { DateTime = new DateTime(2022, 12, 11), Temp = 10, Comment = "It's fresh."  },
                new WeatherForDateDTO() { DateTime = new DateTime(2022, 12, 12), Temp = 25, Comment = "Good weather."  },
                new WeatherForDateDTO() { DateTime = new DateTime(2022, 12, 13), Temp = 35, Comment = "It's time to go to the beach."  }
            };

            var forecastWeatherDTO = new ForecastWeatherDTO() { CityName = cityName, WeatherForPeriod = weatherForPeriod };

            _weatherServiceMock
                .Setup(weatherApiService =>
                    weatherApiService.GetForecastByCityNameAsync(cityName, countDay)
                    )
                .ReturnsAsync(forecastWeatherDTO);

            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);
            Console.SetIn(new StringReader(string.Format("{0}{1}{2}{1}", cityName, Environment.NewLine, countDay)));

            //Act
            await _userCommunicationService.GetForecastByCityNameAsync();

            //Assert
            var expected = "Please, enter city name:\r\n" +
                "Please, enter count day:\r\n" +
                "Minsk weather forecast: \n" +
                "Day 0 (10 декабря 2022 г.): -1,0 C. Dress warmly. \n" +
                "Day 1 (11 декабря 2022 г.): 10,0 C. It's fresh. \n" +
                "Day 2 (12 декабря 2022 г.): 25,0 C. Good weather. \n" +
                "Day 3 (13 декабря 2022 г.): 35,0 C. It's time to go to the beach.\r\n";

            Assert.Equal(expected, consoleOutput.ToString());
        }
    }
}