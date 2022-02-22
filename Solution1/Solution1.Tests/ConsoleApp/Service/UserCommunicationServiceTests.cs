using BusinessLayer.Abstract;
using BusinessLayer.DTOs;
using BusinessLayer.Service;
using ConsoleApp.Service;
using ConsoleApp.Service.Abstract;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Weather.Tests.ConsoleApp.Service
{
    public class UserCommunicationServiceTests
    {
        private readonly UserCommunicationService _userCommunicationService;
        private readonly Mock<IWeatherServiсe> _weatherServiceMock;
        private readonly Mock<ILogger> _loggerMock;

        public UserCommunicationServiceTests()
        {
            _loggerMock = new Mock<ILogger>();
            _weatherServiceMock = new Mock<IWeatherServiсe>();
            _userCommunicationService = new UserCommunicationService(_loggerMock.Object, _weatherServiceMock.Object);
        }

        [Fact]
        public void Communication_EnterEmptyCityName_ShowNoticeAndReturn()
        {
            // Arrange

            //Act
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            Console.SetIn(new StringReader(string.Format(Environment.NewLine)));
            
            _userCommunicationService.Communication();

            //Assert
            var result = string.Format("Please, enter city name:{0}City name can't be empty.{0}", Environment.NewLine);
            Assert.Equal(result, consoleOutput.ToString());
        }

        [Fact]
        public void Communication_EnterCityName_ShowStringPresentation()
        {
            // Arrange
            var cityName = "Minsk";
            var weatherDTO = new WeatherDTO() { CityName = cityName, Temp = 10, Comment = "It's fresh." };
            _weatherServiceMock
                .Setup(weatherApiService =>
                    weatherApiService
                    .GetByCityNameAsync(cityName))
                .ReturnsAsync(weatherDTO);

            //Act
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            Console.SetIn(new StringReader(string.Format("Minsk{0}", Environment.NewLine)));

            _userCommunicationService.Communication();

            //Assert
            var result = string.Format("Please, enter city name:{0}In Minsk 10 C. It's fresh.{0}", Environment.NewLine);
            Assert.Equal(result, consoleOutput.ToString());
        }

        [Fact]
        public void Communication_EnterBadCityName_HandlingExceptionAndShowNotice()
        {
            // Arrange
            var exception = new HttpRequestException(null,null, HttpStatusCode.NotFound);
            _weatherServiceMock
                .Setup(weatherApiService =>
                    weatherApiService
                    .GetByCityNameAsync("AAA"))
                .Throws(exception);

            //Act
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            Console.SetIn(new StringReader(string.Format("AAA{0}", Environment.NewLine)));

            _userCommunicationService.Communication();

            //Assert
            var result = string.Format("Please, enter city name:{0}Entered incorrect city name. Try one time yet.{0}", Environment.NewLine);
            Assert.Equal(result, consoleOutput.ToString());
        }
        
        [Fact]
        public void Communication_EnterCityName_HandlingExceptionRequestErrorAndShowNotice()
        {
            // Arrange
            var exception = new HttpRequestException(null, null, HttpStatusCode.BadRequest);
            _weatherServiceMock
                .Setup(weatherApiService =>
                    weatherApiService
                    .GetByCityNameAsync("Minsk"))
                .Throws(exception);

            //Act
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            Console.SetIn(new StringReader(string.Format("Minsk{0}", Environment.NewLine)));

            _userCommunicationService.Communication();

            //Assert
            var result = string.Format("Please, enter city name:{0}Request error. Try again later.{0}", Environment.NewLine);
            Assert.Equal(result, consoleOutput.ToString());
        }

        [Fact]
        public void Communication_HandlingException_Seccess()
        {
            // Arrange
            
            //Act
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            Console.SetIn(new StringReader(string.Format("Minsk{0}", Environment.NewLine)));

            _userCommunicationService.Communication();

            //Assert
            var result = string.Format("Please, enter city name:{0}Unexpected error. Try one time yet.{0}", Environment.NewLine);
            Assert.Equal(result, consoleOutput.ToString());
        }
    }
}