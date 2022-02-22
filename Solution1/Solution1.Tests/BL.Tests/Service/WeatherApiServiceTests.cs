using BusinessLayer.DTOs;
using BusinessLayer.Service.Abstract;
using KellermanSoftware.CompareNetObjects;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;


namespace Solution1.Tests.BL.Tests
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
        public async Task GetByCityNameAsync_ReternedWeatherApiDTO()
        {
            // Arrange
            var cityName = "Minsk";
            var expectedObject = new WeatherApiDTO() { CityName = cityName, TemperatureValues = new WeatherApiTempDTO() { Temp = 1.86 } };
            var urlString = "https://api.openweathermap.org/data/2.5/weather?q=Minsk&appid=3fe39edadae3ae57d133a80598d5b120&units=metric";

            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(@"{ ""main"": {""temp"": 1.86}, ""name"": ""Minsk""}"),
            };

            _httpMessageHandler
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.Is<HttpRequestMessage>(
                      request => request.Method == HttpMethod.Get
                      && request.RequestUri.ToString() == urlString),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(response);

            // Act
            var actualObject = await _weatherApiService.GetByCityNameAsync(cityName);

            // Assert
            Assert.True(new CompareLogic().Compare(actualObject, expectedObject).AreEqual);
        }
    }
}
