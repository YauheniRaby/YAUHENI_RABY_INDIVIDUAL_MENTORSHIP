using Xunit;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using WeatherApi;
using System.Threading.Tasks;
using System.Net;
using BusinessLayer.DTOs;
using System.Text.Json;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System;

namespace Weather.Tests.Integration
{
    public class WeatherApiTests
    {
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly string cityName = "Minsk";
        private readonly int counDays = 3;
        private readonly List<string> comments = new List<string>() { "Dress warmly.", "It's fresh.", "Good weather.", "It's time to go to the beach." };
        
        public WeatherApiTests()      
        {
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
        }

        [Fact]
        public async Task GetWeatherByCityName_Success()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/weather/{cityName}");
            var httpClient = GetClient();

            //Act
            var response = await httpClient.SendAsync(request);

            //Assert
            Assert.NotNull(response);
            Assert.NotNull(response.Content);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var weather = JsonSerializer.Deserialize<WeatherDTO>(await response.Content.ReadAsStringAsync(), _serializerOptions);

            Assert.NotNull(weather);
            Assert.NotNull(weather.CityName);
            Assert.Equal(weather.CityName, cityName);
            Assert.NotNull(weather.Comment);
            Assert.Contains(weather.Comment, comments);
        }

        [Fact]
        public async Task GetForecastByCityName_Success()
        {
            // Arrange            
            var startDateTime = DateTime.Now.Date;
            var httpClient = GetClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/weather/{cityName}/{counDays}");

            //Act
            var response = await httpClient.SendAsync(request);

            //Assert
            Assert.NotNull(response);
            Assert.NotNull(response.Content);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            var forecast = JsonSerializer.Deserialize<ForecastWeatherDTO>(await response.Content.ReadAsStringAsync(), _serializerOptions);

            Assert.NotNull(forecast);
            Assert.NotNull(forecast.CityName);
            Assert.Equal(forecast.CityName, cityName);
            Assert.NotNull(forecast.WeatherForPeriod);
            Assert.Equal(forecast.WeatherForPeriod.Count, counDays+1);
            
            forecast.WeatherForPeriod.ForEach(x =>
            {
                Assert.NotNull(x);
                Assert.NotNull(x.Comment);
                Assert.Contains(x.Comment, comments);
                Assert.Equal(startDateTime, x.DateTime);
                startDateTime = startDateTime.AddDays(1);
            });
        }

        [Theory]
        [InlineData("aaaaaaaaaaaaaaaaaaaaa", HttpStatusCode.BadRequest)]
        [InlineData("", HttpStatusCode.NotFound)]
        public async Task GetWeatherByCityName_EnterInvalidData_HandlingException(string cityName, HttpStatusCode httpStatusCode)
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/weather/{cityName}");
            var httpClient = GetClient();

            //Act
            var response = await httpClient.SendAsync(request);

            //Assert
            Assert.NotNull(response);
            Assert.Equal(httpStatusCode, response.StatusCode);            
        }

        [Theory]
        [InlineData("Minsk", -1, HttpStatusCode.BadRequest)]
        [InlineData("Minsk", 10, HttpStatusCode.BadRequest)]
        [InlineData("aaaaaaaaaaaaaaaaaaaaa", 3, HttpStatusCode.BadRequest)]
        [InlineData("aaaaaaaaaaaaaaaaaaaaa", 7, HttpStatusCode.BadRequest)]
        [InlineData("", 10, HttpStatusCode.NotFound)]
        [InlineData("", 3, HttpStatusCode.NotFound)]

        public async Task GetForecastByCityName_EnterInvalidData_HandlingException(string cityName, int countDays, HttpStatusCode httpStatusCode)
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/weather/{cityName}/{countDays}");
            var httpClient = GetClient();

            //Act
            var response = await httpClient.SendAsync(request);

            //Assert
            Assert.NotNull(response);
            Assert.Equal(httpStatusCode, response.StatusCode);
        }

        [Fact]
        public async Task GetWeatherByCityName_CanceledOperation_Success()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/weather/{cityName}");
            var httpClient = GetClient(0);

            //Act
            var response = await httpClient.SendAsync(request);

            //Assert
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.RequestTimeout, response.StatusCode);
        }

        [Fact]
        public async Task GetForecastByCityName_CanceledOperation_Success()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/weather/{cityName}/{counDays}");
            var httpClient = GetClient(0);

            //Act
            var response = await httpClient.SendAsync(request);

            //Assert
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.RequestTimeout, response.StatusCode);
        }

        private HttpClient GetClient(int RequestTimeout = 1000, int maxCountDaysForecast = 5, int minCountDaysForecast = 0, bool isDebugMode = true)
        {
            var configuration = new Dictionary<string, string>
            {
                {"AppParams:MaxCountDaysForecast", $"{maxCountDaysForecast}"},
                {"AppParams:MinCountDaysForecast", $"{minCountDaysForecast}"},
                {"AppParams:IsDebugMode", $"{isDebugMode}"},
                {"AppParams:RequestTimeout", $"{RequestTimeout}"}
            };

            var server = new TestServer(new WebHostBuilder()
                .ConfigureAppConfiguration(configurationBuilder =>
                {
                    configurationBuilder.AddInMemoryCollection(configuration);
                })
                .UseStartup<Startup>());
            
            return server.CreateClient();            
        }
    }
}
