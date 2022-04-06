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
using BusinessLayer;
using BusinessLayer.Command.Abstract;
using BusinessLayer.Command;
using BusinessLayer.Services.Abstract;
using BusinessLayer.DTOs;
using Weather.Tests.Infrastructure;
using System.Globalization;
using BusinessLayer.DTOs.Enums;
using System.Threading;
using ConsoleApp.Configuration.Abstract;

namespace Weather.Tests.ConsoleApp.Services
{
    public class UserCommunicateServiceTests
    {
        private readonly UserCommunicateService _userCommunicationService;
        private readonly Mock<IWeatherServiсe> _weatherServiceMock;
        private readonly Mock<ILogger> _loggerMock;
        private readonly Mock<IInvoker> _invokerMock;
        private readonly Mock<IConfig> _config;
        private readonly string cityName = "Minsk";
        private readonly double temp = 5;
        private readonly string comment = Constants.WeatherComments.Fresh;
        private readonly DateTime dateStartForecast = new DateTime(2022, 01, 10);

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
            _config = new Mock<IConfig>();

            _userCommunicationService = new UserCommunicateService(_loggerMock.Object, _invokerMock.Object, _weatherServiceMock.Object, _config.Object);
        }

        [Fact]
        public async Task CommunicateAsync_GetForecastWeather_ShowForecastWeather()
        {
            // Arrange
            var countDays = 3;
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);
            Console.SetIn(new StringReader($"2{Environment.NewLine}{cityName}{Environment.NewLine}{countDays}"));
            var culture = new CultureInfo("en_US");

            var weatherForPeriod = new List<WeatherForDateDTO>();
            for (int currentCountDays = 0; currentCountDays < countDays; currentCountDays++)
            {
                weatherForPeriod.Add(new WeatherForDateDTO() { DateTime = dateStartForecast.AddDays(currentCountDays), Temp = temp + currentCountDays, Comment = comment });
            }
            var forecastWeather = new ForecastWeatherDTO() { CityName = cityName, WeatherForPeriod = weatherForPeriod };

            _invokerMock
                .Setup(invoker => invoker.RunAsync(It.IsAny<ForecastWeatherCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(forecastWeather);

            //Act
            await _userCommunicationService.CommunicateAsync();

            //Assert            
            var ferecastRepresentation = $"{cityName} weather forecast:{Environment.NewLine}";
            for (int currentCountDays = 0; currentCountDays < countDays; currentCountDays++)
            {
                ferecastRepresentation+= $"Day {currentCountDays} ({dateStartForecast.AddDays(currentCountDays).ToString(Constants.DateTimeFormats.Date, culture)}): {temp + currentCountDays:f1} C. {comment}{Environment.NewLine}";
            }
            var expected = $"{Menu.GetMenuRepresentation()}{Environment.NewLine}" +
                $"Please, enter city name:{Environment.NewLine}" +
                $"Please, enter count day:{Environment.NewLine}{ferecastRepresentation}";                

            _invokerMock.Verify(i => i.RunAsync(It.IsAny<ForecastWeatherCommand>(), It.IsAny<CancellationToken>()));
            Assert.Equal(expected, consoleOutput.ToString());
        }

        [Fact]
        public async Task CommunicateAsync_GetCurrentWeather_ShowCurrentWeather()
        {
            // Arrange
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);
            Console.SetIn(new StringReader($"1{Environment.NewLine}{cityName}{Environment.NewLine}"));

            var Weather = new WeatherDTO()
            {
                CityName = cityName,
                Temp = temp,
                Comment = comment
            };

            _invokerMock
                .Setup(invoker => invoker.RunAsync(It.IsAny<CurrentWeatherCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Weather);

            //Act
            await _userCommunicationService.CommunicateAsync();

            //Assert            
            var expected = $"{Menu.GetMenuRepresentation()}{Environment.NewLine}" +
                $"Please, enter city name:{Environment.NewLine}" +
                $"In {cityName} {temp} C. {comment}{Environment.NewLine}";

            _invokerMock.Verify(i => i.RunAsync(It.IsAny<CurrentWeatherCommand>(), It.IsAny<CancellationToken>()));
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
                    .RunAsync(It.IsAny<CurrentWeatherCommand>(), It.IsAny<CancellationToken>()))
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

        [Theory]
        [InlineData("-1", "Value out of range. Try one time yet.")]
        [InlineData("a", "Incorrect value. Try one time yet.")]
        [InlineData(default, "Incorrect value. Try one time yet.")]
        public async Task CommunicateAsync_EnterIncorrectValueForMenu_ShowNotice(string inputValue, string message)
        {
            // Arrange
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);
            Console.SetIn(new StringReader($"{inputValue}{Environment.NewLine}"));

            //Act
            await _userCommunicationService.CommunicateAsync();

            //Assert
            var expected = $"{Menu.GetMenuRepresentation()}{Environment.NewLine}{message}{Environment.NewLine}";
            Assert.Equal(expected, consoleOutput.ToString());
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task CommunicateAsync_GetBestWeatherByArrayCityNameAsync_ShowBestWeather(bool isDebugMode)
        {
            // Arrange
            var leadTime = 472;

            var cityName2 = "Moscow";
            var temp2 = 12;
            var leadTime2 = 476;

            var cityName3 = "AAA";
            var leadTime3 = 475;
            var testError3 = "Test error message";

            var cityName4 = "Paris";
            var testError4 = "Timeout exceeded";

            var dictionaryWeatherResponsesDTO = new Dictionary<ResponseStatus, IEnumerable<WeatherResponseDTO>>
            {
                { ResponseStatus.Successful, new List<WeatherResponseDTO>
                            {
                                new WeatherResponseDTO() { CityName = cityName, ResponseStatus = ResponseStatus.Successful, Temp = temp, LeadTime = leadTime },
                                new WeatherResponseDTO() { CityName = cityName2, ResponseStatus = ResponseStatus.Successful, Temp = temp2, LeadTime = leadTime2 }
                            }
                },
                { ResponseStatus.Fail, new List<WeatherResponseDTO>
                            {
                                new WeatherResponseDTO() { CityName = cityName3, ResponseStatus = ResponseStatus.Fail, ErrorMessage=testError3, LeadTime = leadTime3 }
                            }
                },
                { ResponseStatus.Canceled, new List<WeatherResponseDTO>
                            {
                                new WeatherResponseDTO() { CityName = cityName4, ResponseStatus = ResponseStatus.Canceled, ErrorMessage = testError4 }
                            }
                }
            };

            var consoleOutput = new StringWriter();

            Console.SetOut(consoleOutput);
            Console.SetIn(new StringReader($"3{Environment.NewLine}{cityName}, {cityName3}, {cityName2}{Environment.NewLine}"));

            _invokerMock
                .Setup(invoker => invoker.RunAsync(It.IsAny<BestWeatherCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(dictionaryWeatherResponsesDTO);

            _config
                .Setup(config => config.IsDebugMode)
                .Returns(isDebugMode);

            //Act
            await _userCommunicationService.CommunicateAsync();

            //Assert            
            var bestWeatherRepresentation = $"City with the highest temperature {temp2} C: {cityName2}. " +
                $"Successful request count: 2, failed: 1, canceled: 1.";
            
            var successfulResponsesRepresentation = "Success case:" +
                $"{Environment.NewLine}City: '{cityName}', Temp: {temp}, Timer: {leadTime} ms." +
                $"{Environment.NewLine}City: '{cityName2}', Temp: {temp2}, Timer: {leadTime2} ms.";
            var failResponsesRepresentation = "On fail:" +
                $"{Environment.NewLine}City: '{cityName3}', ErrorMessage: {testError3}, Timer: {leadTime3} ms.";
            var canceledResponsesRepresentation = "On canceled:" +
                $"{Environment.NewLine}Weather request for '{cityName4}' was canceled due to a timeout.";

            var debugInfoRepresentation = isDebugMode
                ? $"{successfulResponsesRepresentation}{Environment.NewLine}" +
                $"{failResponsesRepresentation}{Environment.NewLine}" +
                $"{canceledResponsesRepresentation}{Environment.NewLine}"
                : string.Empty;

            var expected = $"{Menu.GetMenuRepresentation()}{Environment.NewLine}" +
                $"Please, enter array city name (separator symbal - ',') :{Environment.NewLine}" +
                $"{bestWeatherRepresentation}{Environment.NewLine}{debugInfoRepresentation}";

            _invokerMock.Verify(i => i.RunAsync(It.IsAny<BestWeatherCommand>(), It.IsAny<CancellationToken>()));
            Assert.Equal(expected, consoleOutput.ToString());
        }
    }
}