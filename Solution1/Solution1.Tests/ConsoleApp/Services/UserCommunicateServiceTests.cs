﻿using ConsoleApp.Services;
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

namespace Weather.Tests.ConsoleApp.Services
{
    public class UserCommunicateServiceTests
    {
        private readonly UserCommunicateService _userCommunicationService;
        private readonly Mock<IWeatherServiсe> _weatherServiceMock;
        private readonly Mock<ILogger> _loggerMock;
        private readonly Mock<IInvoker> _invokerMock;
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
            _userCommunicationService = new UserCommunicateService(_loggerMock.Object, _invokerMock.Object, _weatherServiceMock.Object);
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
                .Setup(invoker => invoker.RunAsync(It.IsAny<ForecastWeatherCommand>()))
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

            _invokerMock.Verify(i => i.RunAsync(It.IsAny<ForecastWeatherCommand>()));
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
                .Setup(invoker => invoker.RunAsync(It.IsAny<CurrentWeatherCommand>()))
                .ReturnsAsync(Weather);

            //Act
            await _userCommunicationService.CommunicateAsync();

            //Assert            
            var expected = $"{Menu.GetMenuRepresentation()}{Environment.NewLine}" +
                $"Please, enter city name:{Environment.NewLine}" +
                $"In {cityName} {temp} C. {comment}{Environment.NewLine}";

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
    }
}