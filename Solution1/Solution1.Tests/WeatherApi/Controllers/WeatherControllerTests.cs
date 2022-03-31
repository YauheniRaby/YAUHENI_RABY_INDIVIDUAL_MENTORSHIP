﻿using BusinessLayer.Command;
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
        private readonly Mock<IOptions<Config>> _config;
        private readonly WeatherController _weatherController;
        private readonly string cityName = "Minsk";

        public WeatherControllerTests()
        {
            _invokerMock = new Mock<IInvoker>();
            _weatherServiceMock = new Mock<IWeatherServiсe>();
            _config = new Mock<IOptions<Config>>();
            
            _weatherController = new WeatherController(_weatherServiceMock.Object, _config.Object, _invokerMock.Object);

            _config
                .Setup(config => config.Value)
                .Returns(new Config() { RequestTimeout = int.MaxValue });
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

            //Act
            var response = await _weatherController.GetForecastWeatherByCityNameAsync(cityName, 2);

            //Assert
            _invokerMock.Verify(i => i.RunAsync(It.IsAny<ForecastWeatherCommand>(), It.Is<CancellationToken>(x => !x.IsCancellationRequested)));
            Assert.IsType<OkObjectResult>(response.Result);
            var result = (OkObjectResult)response.Result;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.True(new CompareLogic().Compare(forecastWeather, result.Value).AreEqual);
        }
    }
}