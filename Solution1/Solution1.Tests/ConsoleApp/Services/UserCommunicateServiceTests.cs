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
        private readonly IUserCommunicateService _userCommunicationService;
        private readonly Mock<IPerformerCommandsService> _performerCommandsService;
        private readonly Mock<ILogger> _loggerMock;
        private readonly Mock<IInvoker> _invokerMock;
        

        public UserCommunicateServiceTests()
        {
            _loggerMock = new Mock<ILogger>();
            _performerCommandsService = new Mock<IPerformerCommandsService>();
            _invokerMock = new Mock<IInvoker>();
            

            _userCommunicationService = new UserCommunicateService(_loggerMock.Object, _invokerMock.Object, _performerCommandsService.Object);
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


    }
}