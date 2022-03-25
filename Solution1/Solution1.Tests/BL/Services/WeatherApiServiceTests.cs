using BusinessLayer.DTOs.WeatherAPI;
using BusinessLayer.Services;
using KellermanSoftware.CompareNetObjects;
using Moq;
using Moq.Protected;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Weather.Tests.Infrastructure;
using Xunit;

namespace Weather.Tests.BL.Services
{
    public class WeatherApiServiceTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandler;
        private readonly HttpClient _httpClient;
        private readonly WeatherApiService _weatherApiService;
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly string cityName = "Minsk";
        private readonly double temp = 10;

        public WeatherApiServiceTests()
        {
            _httpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandler.Object);
            _weatherApiService = new WeatherApiService(_httpClient);
            _serializerOptions = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        }

        [Fact]
        public async Task GetByCityNameAsync_ReturnedWeatherApiDTO_Success()
        {
            // Arrange
            var urlString = "https://api.openweathermap.org/data/2.5/weather?q=Minsk&appid=3fe39edadae3ae57d133a80598d5b120&units=metric";
            
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(new { Main = new { Temp = temp }, Name = cityName }, _serializerOptions)),
            };

            SetHttpHandlerSettings(response, urlString);

            // Act
            var result = await _weatherApiService.GetByCityNameAsync(cityName, new CancellationToken());

            // Assert
            var expectedWeatherApiDto = new WeatherApiDTO() { CityName = cityName, TemperatureValues = new WeatherApiTempDTO() { Temp = temp } };
            Assert.True(new CompareLogic().Compare(expectedWeatherApiDto, result).AreEqual);
        }

        [Fact]
        public async Task GetForecastByCityNameAsync_ReturnedForecastWeatherApiDTO_Success()
        {
            // Arrange
            var lat = 53;
            var lon = 27;
            var urlCoordinates = $"http://api.openweathermap.org/geo/1.0/direct?q={cityName}&appid=3fe39edadae3ae57d133a80598d5b120";
            var urlForecast = $"https://api.openweathermap.org/data/2.5/forecast?lat={lat}&lon={lon}&cnt=2&units=metric&appid=3fe39edadae3ae57d133a80598d5b120";
            var listDataForTest = new[]
            {
                new { DateTime = new DateTime(2022, 10 ,11, 09, 00, 00), Temp = temp },
                new { DateTime = new DateTime(2022, 10 ,11, 12, 00, 00), Temp = temp },
            };

            var responseCoordinates = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(new[] { new { Name = cityName, Lat = lat, Lon = lon } }, _serializerOptions)),
            };

            var listWeatherAnonymousObject = listDataForTest
                .Select(t => new { DateTime = t.DateTime.ToString("dd-MM-yyyy HH:mm:ss"), Main = new { Temp = temp } })
                .ToArray();

            var responseForecast = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(
                    JsonSerializer.Serialize(
                        new { City = new { Name = cityName }, List = listWeatherAnonymousObject},
                        new JsonSerializerOptions{ PropertyNamingPolicy = new CamelCaseNamingPolicy()})),
            };

            SetHttpHandlerSettings(responseCoordinates, urlCoordinates);
            SetHttpHandlerSettings(responseForecast, urlForecast);      
            
            // Act
            var result = await _weatherApiService.GetForecastByCityNameAsync(cityName, listWeatherAnonymousObject.Count(), new CancellationToken());

            // Assert            
            var expected = new ForecastWeatherApiDTO()
            {
                City = new CityApiDTO() { Name = cityName },
                WeatherPoints = listDataForTest
                    .Select(w => new WeatherInfoApiDTO()
                    {
                        DateTime = w.DateTime,
                        Temp = new TempApiDTO() { Value = w.Temp }
                    })
                    .ToList()
            };
            
            Assert.True(new CompareLogic().Compare(expected, result).AreEqual);
        }

        [Fact]
        public async Task GetByCityNameAsync_GenerateOperationCanceledException_Success()
        {
            // Arrange
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            
            // Assert
            await Assert.ThrowsAsync<OperationCanceledException>(async() => await _weatherApiService.GetByCityNameAsync(cityName, cancellationTokenSource.Token));
        }

        [Fact]
        public async Task GetForecastByCityNameAsync_GenerateOperationCanceledException_Success()
        {
            // Arrange
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            
            // Assert
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await _weatherApiService.GetForecastByCityNameAsync(cityName, 2, cancellationTokenSource.Token));
        }

        private void SetHttpHandlerSettings(HttpResponseMessage response, string uri)
        {
            _httpMessageHandler
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.Is<HttpRequestMessage>(
                      request =>
                      request.Method == HttpMethod.Get
                      && request.RequestUri.ToString() == uri),
                  ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(response);
        }        
    }
}
