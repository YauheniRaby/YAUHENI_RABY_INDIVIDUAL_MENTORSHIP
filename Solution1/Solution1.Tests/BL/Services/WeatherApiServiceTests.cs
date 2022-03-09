using BusinessLayer.DTOs.WeatherAPI;
using BusinessLayer.Services;
using KellermanSoftware.CompareNetObjects;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Weather.Tests.BL.Services
{
    public class WeatherApiServiceTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandler;
        private readonly HttpClient _httpClient;
        private readonly WeatherApiService _weatherApiService;

        public WeatherApiServiceTests()
        {
            _httpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandler.Object);
            _weatherApiService = new WeatherApiService(_httpClient);
        }

        [Fact]
        public async Task GetByCityNameAsync_ReturnedWeatherApiDTO_Success()
        {
            // Arrange
            var cityName = "Minsk";
            var urlString = "https://api.openweathermap.org/data/2.5/weather?q=Minsk&appid=3fe39edadae3ae57d133a80598d5b120&units=metric";
            
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(new { main = new { temp = 1.86 }, name = "Minsk" })),
            };

            _httpMessageHandler
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.Is<HttpRequestMessage>(
                      request =>
                      request.Method == HttpMethod.Get
                      && request.RequestUri.ToString() == urlString),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(response);

            // Act
            var result = await _weatherApiService.GetByCityNameAsync(cityName);

            // Assert
            var expectedWeatherApiDto = new WeatherApiDTO() { CityName = cityName, TemperatureValues = new WeatherApiTempDTO() { Temp = 1.86 } };
            Assert.True(new CompareLogic().Compare(expectedWeatherApiDto, result).AreEqual);
        }

        [Fact]
        public async Task GetForecastByCityNameAsync_ReturnedForecastWeatherApiDTO_Success()
        {
            // Arrange
            var cityName = "Minsk";
            var lat = 53;
            var lon = 27;
            var urlCoordinates = $"http://api.openweathermap.org/geo/1.0/direct?q={cityName}&appid=3fe39edadae3ae57d133a80598d5b120";
            var urlForecast = $"https://api.openweathermap.org/data/2.5/forecast?lat={lat}&lon={lon}&cnt=2&units=metric&appid=3fe39edadae3ae57d133a80598d5b120";

            var responseCoordinates = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(new[] { new { name = "Minsk", lat = 53, lon = 27 } })),
            };
            
            var responseForecast = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent
                (
                    JsonSerializer.Serialize(new
                    {
                        city = new { name = "Minsk" },
                        list = new[]
                            {
                                new { dt_txt = "2022-03-05 18:00:00", main = new { temp = 2 } },
                                new { dt_txt = "2022-03-05 21:00:00", main = new { temp = 4 } }
                            }
                    })
                ),
            };

            _httpMessageHandler
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.Is<HttpRequestMessage>(
                      request =>
                      request.Method == HttpMethod.Get
                      && request.RequestUri.ToString() == urlCoordinates),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(responseCoordinates);

            _httpMessageHandler
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.Is<HttpRequestMessage>(
                      request =>
                      request.Method == HttpMethod.Get
                      && request.RequestUri.ToString() == urlForecast),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(responseForecast);
            
            // Act
            var result = await _weatherApiService.GetForecastByCityNameAsync(cityName, 2);

            // Assert
            var expected = new ForecastWeatherApiDTO()
            {
                City = new CityApiDTO() { Name = cityName },
                WeatherPoints = new List<WeatherInfoApiDTO>()
                {
                    new WeatherInfoApiDTO()
                    {
                        DateTime = new DateTime(2022, 03, 05, 18, 00, 00),
                        Temp = new TempApiDTO() { Value = 2 }
                    },
                    new WeatherInfoApiDTO()
                    {
                        DateTime = new DateTime(2022, 03, 05, 21, 00, 00),
                        Temp = new TempApiDTO() { Value = 4 }
                    },
                }
            };

            Assert.True(new CompareLogic().Compare(expected, result).AreEqual);
        }
    }
}
