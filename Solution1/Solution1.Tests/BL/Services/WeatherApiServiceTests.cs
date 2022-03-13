using BusinessLayer.DTOs.WeatherAPI;
using BusinessLayer.Services;
using KellermanSoftware.CompareNetObjects;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Weather.Tests.Infrastructure;
using Weather.Tests.Infrastructure.Extensions;
using Xunit;

namespace Weather.Tests.BL.Services
{
    public class WeatherApiServiceTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandler;
        private readonly HttpClient _httpClient;
        private readonly WeatherApiService _weatherApiService;
        private readonly JsonSerializerOptions _serializerOptions;

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
            var cityName = "Minsk";
            var temp = 1.86;
            var urlString = "https://api.openweathermap.org/data/2.5/weather?q=Minsk&appid=3fe39edadae3ae57d133a80598d5b120&units=metric";
            
            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(new { Main = new { Temp = temp }, Name = cityName }, _serializerOptions)),
            };

            _httpMessageHandler.GetSettings(response, urlString);
            
            // Act
            var result = await _weatherApiService.GetByCityNameAsync(cityName);

            // Assert
            var expectedWeatherApiDto = new WeatherApiDTO() { CityName = cityName, TemperatureValues = new WeatherApiTempDTO() { Temp = temp } };
            Assert.True(new CompareLogic().Compare(expectedWeatherApiDto, result).AreEqual);
        }

        [Fact]
        public async Task GetForecastByCityNameAsync_ReturnedForecastWeatherApiDTO_Success()
        {
            // Arrange
            var cityName = "Minsk";
            var lat = 53;
            var lon = 27;
            var temp = 10;
            var dataTime = new DateTime(2022, 03, 05, 9, 0, 0);
            var countWeatherPoints = 2;
            var pointsPeriod = 3;

            var urlCoordinates = $"http://api.openweathermap.org/geo/1.0/direct?q={cityName}&appid=3fe39edadae3ae57d133a80598d5b120";
            var urlForecast = $"https://api.openweathermap.org/data/2.5/forecast?lat={lat}&lon={lon}&cnt=2&units=metric&appid=3fe39edadae3ae57d133a80598d5b120";

            var responseCoordinates = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(new[] { new { Name = cityName, Lat = lat, Lon = lon } }, _serializerOptions)),
            };

            var listWeatherAnonymousObject = Enumerable.Range(0, 0)
                           .Select(a => new { DateTime = default(string), Main = new { Temp = default(int) } }).ToList();

            for (int currentCountTempPoints = 0; currentCountTempPoints < countWeatherPoints; currentCountTempPoints++)
            {
                listWeatherAnonymousObject
                    .Add(
                    new 
                    { 
                        DateTime = dataTime.AddHours(currentCountTempPoints * pointsPeriod).ToString("dd-MM-yyyy hh:mm:ss"), 
                        Main = new { Temp = temp + currentCountTempPoints }
                    });
            }

            var responseForecast = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(
                    JsonSerializer.Serialize(
                        new { City = new { Name = cityName }, List = listWeatherAnonymousObject.ToArray()},
                        new JsonSerializerOptions{ PropertyNamingPolicy = new CamelCaseNamingPolicy()})),
            };

            _httpMessageHandler.GetSettings(responseCoordinates, urlCoordinates);
            _httpMessageHandler.GetSettings(responseForecast, urlForecast);        
            
            // Act
            var result = await _weatherApiService.GetForecastByCityNameAsync(cityName, countWeatherPoints);

            // Assert
            var expected = new ForecastWeatherApiDTO()
            {
                City = new CityApiDTO() { Name = cityName },
                WeatherPoints = new List<WeatherInfoApiDTO>()
            };
            for (int currentCountTempPoints = 0; currentCountTempPoints < countWeatherPoints; currentCountTempPoints++)
            {
                expected.WeatherPoints
                    .Add(
                    new WeatherInfoApiDTO() 
                    { 
                        DateTime = dataTime.AddHours(currentCountTempPoints * pointsPeriod), 
                        Temp = new TempApiDTO() { Value = temp + currentCountTempPoints}
                    });
            }

            Assert.True(new CompareLogic().Compare(expected, result).AreEqual);
        }
    }
}
