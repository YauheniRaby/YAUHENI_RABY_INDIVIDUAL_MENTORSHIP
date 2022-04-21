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
        private readonly string _cityName = "Minsk";
        private readonly double _temp = 10;

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
            var urlCurrentWeather = Constants.CurrentWeatherUrl;

            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(new { Main = new { Temp = _temp }, Name = _cityName }, _serializerOptions)),
            };

            SetHttpHandlerSettings(response, string.Format(urlCurrentWeather, _cityName));

            // Act
            var result = await _weatherApiService.GetByCityNameAsync(_cityName, urlCurrentWeather, CancellationToken.None);

            // Assert
            var expectedWeatherApiDto = new WeatherApiDTO() { CityName = _cityName, TemperatureValues = new WeatherApiTempDTO() { Temp = _temp } };
            Assert.True(new CompareLogic().Compare(expectedWeatherApiDto, result).AreEqual);
        }

        [Fact]
        public async Task GetForecastByCityNameAsync_ReturnedForecastWeatherApiDTO_Success()
        {
            // Arrange
            var lat = 53;
            var lon = 27;
            var urlCoordinates = Constants.CoordinatesUrl;
            var urlForecast = Constants.ForecastUrl;

            var listDataForTest = new[]
            {
                new { DateTime = new DateTime(2022, 01 ,12, 09, 00, 00), Temp = _temp },
                new { DateTime = new DateTime(2022, 01 ,12, 12, 00, 00), Temp = _temp },
            };

            var responseCoordinates = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(new[] { new { Name = _cityName, Lat = lat, Lon = lon } }, _serializerOptions)),
            };
           

            var listWeatherAnonymousObject = listDataForTest
                .Select(t => new { DateTime = t.DateTime.ToString("yyyy-MM-dd HH:mm:ss"), Main = new { Temp = _temp } })
                .ToArray();

            var responseForecast = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(
                    JsonSerializer.Serialize(
                        new { City = new { Name = _cityName }, List = listWeatherAnonymousObject },
                        new JsonSerializerOptions { PropertyNamingPolicy = new CamelCaseNamingPolicy() })),
            };

            var countWeatherPoints = listWeatherAnonymousObject.Length;

            SetHttpHandlerSettings(responseCoordinates, string.Format(urlCoordinates, _cityName));
            SetHttpHandlerSettings(responseForecast, string.Format(urlForecast, lat, lon, countWeatherPoints));

            // Act
            var result = await _weatherApiService.GetForecastByCityNameAsync(_cityName, countWeatherPoints, urlForecast, urlCoordinates, CancellationToken.None);

            // Assert            
            var expected = new ForecastWeatherApiDTO()
            {
                City = new CityApiDTO() { Name = _cityName },
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
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await _weatherApiService.GetByCityNameAsync(_cityName, Constants.CurrentWeatherUrl, new CancellationToken(true)));
        }

        [Fact]
        public async Task GetForecastByCityNameAsync_GenerateOperationCanceledException_Success()
        {
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await _weatherApiService.GetForecastByCityNameAsync(_cityName, 2, Constants.ForecastUrl, Constants.CoordinatesUrl, new CancellationToken(true)));
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
