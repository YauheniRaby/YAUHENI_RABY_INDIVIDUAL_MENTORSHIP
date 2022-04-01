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
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly string cityName = "Minsk";
        private readonly List<string> comments = new List<string>() { "Dress warmly.", "It's fresh.", "Good weather.", "It's time to go to the beach." };
        private readonly Dictionary<string, string> configuration = new Dictionary<string, string>
        {
            {"AppParams:MaxCountDaysForecast", "5"},
            {"AppParams:MinCountDaysForecast", "0"},
            {"AppParams:IsDebugMode", "true"},
            {"AppParams:RequestTimeout", "10000"}
        };

        public WeatherApiTests()      
        {
            var server = new TestServer(new WebHostBuilder()
                .ConfigureAppConfiguration(configurationBuilder =>
                {
                    configurationBuilder.AddInMemoryCollection(configuration);
                })
                .UseStartup<Startup>());
            _httpClient = server.CreateClient();
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
            
            //Act
            var response = await _httpClient.SendAsync(request);

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
            var counDays = 3;
            var startDateTime = DateTime.Now.Date;
            
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/weather/{cityName}/{counDays}");

            //Act
            var response = await _httpClient.SendAsync(request);

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
    }
}
