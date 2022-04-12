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
        private readonly Mock<IOptionsMonitor<AppConfiguration>> _appParams;
        private readonly WeatherController _weatherController;
        private readonly string cityName = "Minsk";

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
            _appParams = new Mock<IOptionsMonitor<AppConfiguration>>();
            
            _weatherController = new WeatherController(_weatherServiceMock.Object, _appParams.Object, _invokerMock.Object);
        }

        [Fact]
        public async Task GetCurrentWeatherByCityName_EnterCityName_ReturnWeather()
        {
            // Arrange
            var temp = 10;
            var comment = "TestComment";

            var weather = new WeatherDTO()
            {
                CityName = cityName,
                Temp = temp,
                Comment = comment
            };

            _invokerMock
                .Setup(invoker => invoker.RunAsync(It.IsAny<CurrentWeatherCommand>(), It.Is<CancellationToken>(x => !x.IsCancellationRequested)))
                .ReturnsAsync(weather);

            SetTimeoutForAppParams();

            //Act
            var response = await _weatherController.GetCurrentWeatherByCityNameAsync(cityName);

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

            SetTimeoutForAppParams();

            await Assert.ThrowsAsync(exception.GetType(), async () => await _weatherController.GetCurrentWeatherByCityNameAsync(cityName));            
        }

        [Fact]
        public async Task GetCurrentWeatherByCityName_CancellateOperation_Success()
        {
            SetTimeoutForAppParams(0);

            await Assert.ThrowsAsync<OperationCanceledException>( async () => await _weatherController.GetCurrentWeatherByCityNameAsync(cityName));
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
                CityName = cityName, 
                WeatherForPeriod =  new List<WeatherForDateDTO>
                {
                    new WeatherForDateDTO() { Comment = comment1, DateTime = dateTime1, Temp = temp1 },
                    new WeatherForDateDTO() { Comment = comment2, DateTime = dateTime2, Temp = temp2 }
                }
            };

            _invokerMock
                .Setup(invoker => invoker.RunAsync(It.IsAny<ForecastWeatherCommand>(), It.Is<CancellationToken>(x => !x.IsCancellationRequested)))
                .ReturnsAsync(forecastWeather);

            SetTimeoutForAppParams();

            //Act
            var response = await _weatherController.GetForecastWeatherByCityNameAsync(cityName, 2);

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

            SetTimeoutForAppParams();

            await Assert.ThrowsAsync(exception.GetType(), async () => await _weatherController.GetForecastWeatherByCityNameAsync(cityName, 2));
        }

        [Fact]
        public async Task GetForecastWeatherByCityName_CancellateOperation_Success()
        {
            SetTimeoutForAppParams(0);

            await Assert.ThrowsAsync<OperationCanceledException>(async () => await _weatherController.GetForecastWeatherByCityNameAsync(cityName, 2));
            _invokerMock.Verify(i => i.RunAsync(It.IsAny<ForecastWeatherCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        private void SetTimeoutForAppParams(int? timeout = null)
        {
            _appParams
                .Setup(x => x.CurrentValue)
                .Returns(new AppConfiguration() { RequestTimeout = timeout });
        }
    }
}
