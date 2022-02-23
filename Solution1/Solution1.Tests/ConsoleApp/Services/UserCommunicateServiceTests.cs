using BusinessLayer.DTOs;
using BusinessLayer.Services.Abstract;
using ConsoleApp.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Weather.Tests.ConsoleApp.Services
{
    public class UserCommunicateServiceTests
    {
        private readonly UserCommunicateService _userCommunicationService;
        private readonly Mock<IWeatherServiсe> _weatherServiceMock;
        private readonly Mock<ILogger> _loggerMock;

        public UserCommunicateServiceTests()
        {
            _loggerMock = new Mock<ILogger>();
            _weatherServiceMock = new Mock<IWeatherServiсe>();
            _userCommunicationService = new UserCommunicateService(_loggerMock.Object, _weatherServiceMock.Object);
        }

        [Fact]
        public async Task Communicate_EnterEmptyCityName_ShowNoticeAndReturnAsync()
        {
            // Arrange
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            Console.SetIn(new StringReader(string.Format(Environment.NewLine)));

            //Act
            await _userCommunicationService.CommunicateAsync();

            //Assert
            var expected = string.Format("Please, enter city name:{0}City name can't be empty.{0}", Environment.NewLine);
            Assert.Equal(expected, consoleOutput.ToString());
        }

        [Fact]
        public async Task Communicate_EnterCityName_ShowStringRepresentationAsync()
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
            await _userCommunicationService.CommunicateAsync();

            //Assert
            var expected = string.Format("Please, enter city name:{0}In Minsk 10 C. It's fresh.{0}", Environment.NewLine);
            Assert.Equal(expected, consoleOutput.ToString());
        }

        [Theory]
        [InlineData("Minsk", "Please, enter city name:\r\nUnexpected error. Try one time yet.\r\n", null)]
        [InlineData("Minsk", "Please, enter city name:\r\nRequest error. Try again later.\r\n", HttpStatusCode.BadRequest)]
        [InlineData("AAA", "Please, enter city name:\r\nEntered incorrect city name. Try one time yet.\r\n", HttpStatusCode.NotFound)]
        public async Task Communicate_EnterCityName_HandlingExceptionAndShowNoticeAsync(string cityName, string message, HttpStatusCode? statusCode)
        {
            // Arrange
            Exception exception;

            if (statusCode.HasValue)
            {
                exception = new HttpRequestException(null, null, statusCode);
            }
            else
            {
                exception = new Exception();
            }
            
            _weatherServiceMock
                .Setup(weatherApiService =>
                    weatherApiService
                    .GetByCityNameAsync(cityName))
                .Throws(exception);

            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            Console.SetIn(new StringReader(string.Format("{0}{1}", cityName, Environment.NewLine)));

            //Act
            await _userCommunicationService.CommunicateAsync();

            //Assert
            Assert.Equal(message, consoleOutput.ToString());
        }        
    }
}