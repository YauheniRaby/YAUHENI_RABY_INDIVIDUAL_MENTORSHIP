using BusinessLayer.Command;
using BusinessLayer.Command.Abstract;
using BusinessLayer.DTOs;
using BusinessLayer.Services.Abstract;
using KellermanSoftware.CompareNetObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WeatherApi.Configuration;
using WeatherApi.Controllers;
using Xunit;

namespace Weather.Tests.WeatherApi.Controllers
{
    public class WeatherControllerTests
    {
        private readonly Mock<IInvoker> _invokerMock;
        private readonly Mock<IWeatherServiсe> _weatherServiceMock;
        private readonly Mock<IHistoryWeatherService> _historyWeatherServiceMock;
        private readonly Mock<IOptionsMonitor<AppConfiguration>> _appConfig;
        private readonly Mock<IOptionsMonitor<WeatherApiConfiguration>> _apiConfig;
        private readonly WeatherController _weatherController;
        private readonly string _cityName = "Minsk";

        public static IEnumerable<object[]> Exceptions =>
            new List<object[]>
            {
                new object[] { new Exception() },
                new object[] { new OperationCanceledException() }
            };

        public WeatherControllerTests()
        {
            _invokerMock = new Mock<IInvoker>();
            _weatherServiceMock = new Mock<IWeatherServiсe>();
            _historyWeatherServiceMock = new Mock<IHistoryWeatherService>();
            _appConfig = new Mock<IOptionsMonitor<AppConfiguration>>();
            _apiConfig = new Mock<IOptionsMonitor<WeatherApiConfiguration>>();
            _weatherController = new WeatherController(_weatherServiceMock.Object, _historyWeatherServiceMock.Object, _appConfig.Object, _apiConfig.Object, _invokerMock.Object);
        }

        [Fact]
        public async Task GetCurrentWeatherByCityName_EnterCityName_ReturnWeather()
        {
            // Arrange
            var temp = 10;
            var comment = "TestComment";

            var weather = new WeatherDTO()
            {
                CityName = _cityName,
                Temp = temp,
                Comment = comment
            };

            _invokerMock
                .Setup(invoker => invoker.RunAsync(It.IsAny<CurrentWeatherCommand>(), It.Is<CancellationToken>(x => !x.IsCancellationRequested)))
                .ReturnsAsync(weather);

            SetTimeoutForAppConfig();
            SetDefaultValueForApiConfig();
            //Act
            var response = await _weatherController.GetCurrentWeatherByCityNameAsync(_cityName);

            //Assert
            _invokerMock.Verify(i => i.RunAsync(It.IsAny<CurrentWeatherCommand>(), It.Is<CancellationToken>(x => !x.IsCancellationRequested)));
            Assert.IsType<OkObjectResult>(response.Result);
            var result = (OkObjectResult)response.Result;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.True(new CompareLogic().Compare(weather, result.Value).AreEqual);
        }


        [Theory]
        [MemberData(nameof(Exceptions))]
        public async Task GetCurrentWeatherByCityName_ExceptionHandlingFromDependencyService_Success(Exception exception)
        {
            _invokerMock
                .Setup(invoker => invoker.RunAsync(It.IsAny<CurrentWeatherCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);

            SetTimeoutForAppConfig();
            SetDefaultValueForApiConfig();

            await Assert.ThrowsAsync(exception.GetType(), async () => await _weatherController.GetCurrentWeatherByCityNameAsync(_cityName));
        }

        [Fact]
        public async Task GetCurrentWeatherByCityName_CancellateOperation_Success()
        {
            SetTimeoutForAppConfig(0);
            SetDefaultValueForApiConfig();

            await Assert.ThrowsAsync<OperationCanceledException>(async () => await _weatherController.GetCurrentWeatherByCityNameAsync(_cityName));
            _invokerMock.Verify(i => i.RunAsync(It.IsAny<CurrentWeatherCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetForecastWeatherByCityName_EnterCityNameAndCounDays_ReturnForecast()
        {
            // Arrange
            var temp1 = 10;
            var comment1 = "TestComment";
            var dateTime1 = new DateTime(2022, 02, 02, 12, 0, 0);
            var temp2 = 13;
            var comment2 = "TestComment2";
            var dateTime2 = new DateTime(2022, 02, 02, 15, 0, 0);

            var forecastWeather = new ForecastWeatherDTO()
            {
                CityName = _cityName,
                WeatherForPeriod = new List<WeatherForDateDTO>
                {
                    new WeatherForDateDTO() { Comment = comment1, DateTime = dateTime1, Temp = temp1 },
                    new WeatherForDateDTO() { Comment = comment2, DateTime = dateTime2, Temp = temp2 }
                }
            };

            _invokerMock
                .Setup(invoker => invoker.RunAsync(It.IsAny<ForecastWeatherCommand>(), It.Is<CancellationToken>(x => !x.IsCancellationRequested)))
                .ReturnsAsync(forecastWeather);

            SetTimeoutForAppConfig();
            SetDefaultValueForApiConfig();

            //Act
            var response = await _weatherController.GetForecastWeatherByCityNameAsync(_cityName, 2);

            //Assert
            _invokerMock.Verify(i => i.RunAsync(It.IsAny<ForecastWeatherCommand>(), It.Is<CancellationToken>(x => !x.IsCancellationRequested)));
            Assert.IsType<OkObjectResult>(response.Result);
            var result = (OkObjectResult)response.Result;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.NotNull(result.Value);
            Assert.True(new CompareLogic().Compare(forecastWeather, result.Value).AreEqual);
        }

        [Theory]
        [MemberData(nameof(Exceptions))]
        public async Task GetForecastWeatherByCityName_ExceptionHandling_Success(Exception exception)
        {
            _invokerMock
                .Setup(invoker => invoker.RunAsync(It.IsAny<ForecastWeatherCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);

            SetTimeoutForAppConfig();
            SetDefaultValueForApiConfig();

            await Assert.ThrowsAsync(exception.GetType(), async () => await _weatherController.GetForecastWeatherByCityNameAsync(_cityName, 2));
        }

        [Fact]
        public async Task GetForecastWeatherByCityName_CancellateOperation_Success()
        {
            SetTimeoutForAppConfig(0);
            SetDefaultValueForApiConfig();

            await Assert.ThrowsAsync<OperationCanceledException>(async () => await _weatherController.GetForecastWeatherByCityNameAsync(_cityName, 2));
            _invokerMock.Verify(i => i.RunAsync(It.IsAny<ForecastWeatherCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetHistoryWeatherByCityNameAsync_EnterHistoryWeatherRequest_ReturnHistoryWeather()
        {
            // Arrange
            var dateTimeStart = new DateTime(2022, 02, 02, 12, 0, 0);
            var dateTimeEnd = new DateTime(2022, 03, 03, 15, 30, 0);
            
            var dateTime1 = new DateTime(2022, 02, 05, 0, 0, 0);
            var Temp1 = 10;
            var Comment1 = "TestComment1";

            var dateTime2 = new DateTime(2022, 02, 07, 0, 0, 0);
            var Temp2 = 10;
            var Comment2 = "TestComment2";

            var historyWeather = new HistoryWeatherDTO()
            {
                CityName = _cityName,
                WeatherList = new List<WeatherWithDatetimeDTO>()
                { 
                    new WeatherWithDatetimeDTO() { DateTime = dateTime1, Temp = Temp1, Comment = Comment1 },
                    new WeatherWithDatetimeDTO() { DateTime = dateTime2, Temp = Temp2, Comment = Comment2 }
                }
            };

            _invokerMock
                .Setup(invoker => invoker.RunAsync(It.IsAny<HistoryWeatherCommand>(), It.Is<CancellationToken>(x => !x.IsCancellationRequested)))
                .ReturnsAsync(historyWeather);

            SetTimeoutForAppConfig();            

            //Act
            var response = await _weatherController.GetHistoryWeatherByCityNameAsync(new HistoryWeatherRequestDTO() { CityName = _cityName, EndPeriod = dateTimeEnd, StartPeriod = dateTimeStart});

            //Assert
            _invokerMock.Verify(i => i.RunAsync(It.IsAny<HistoryWeatherCommand>(), It.Is<CancellationToken>(x => !x.IsCancellationRequested)));
            Assert.IsType<OkObjectResult>(response.Result);
            var result = (OkObjectResult)response.Result;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.NotNull(result.Value);
            Assert.True(new CompareLogic().Compare(historyWeather, result.Value).AreEqual);
        }

        [Fact]
        public async Task GetHistoryWeatherByCityNameAsync_CancellateOperation_Success()
        {
            SetTimeoutForAppConfig(0);
            
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await _weatherController.GetHistoryWeatherByCityNameAsync(new HistoryWeatherRequestDTO()));
            _invokerMock.Verify(i => i.RunAsync(It.IsAny<HistoryWeatherCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Theory]
        [MemberData(nameof(Exceptions))]
        public async Task GetHistoryWeatherByCityNameAsync_ExceptionHandling_Success(Exception exception)
        {
            _invokerMock
                .Setup(invoker => invoker.RunAsync(It.IsAny<HistoryWeatherCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);

            SetTimeoutForAppConfig();
            SetDefaultValueForApiConfig();

            await Assert.ThrowsAsync(exception.GetType(), async () => await _weatherController.GetHistoryWeatherByCityNameAsync(new HistoryWeatherRequestDTO()));
        }

        private void SetTimeoutForAppConfig(int? timeout = null)
        {
            _appConfig
                .Setup(x => x.CurrentValue)
                .Returns(new AppConfiguration() { RequestTimeout = timeout });
        }

        private void SetDefaultValueForApiConfig()
        {
            _apiConfig
                .Setup(x => x.CurrentValue)
                .Returns(new WeatherApiConfiguration() 
                { 
                    CurrentWeatherUrl = "test.com/current", 
                    CoordinatesUrl = "test.com/coordinates", 
                    ForecastWeatherUrl = "test.com/forecast", 
                    Key = "TestKey"
                });
        }
    }
}
