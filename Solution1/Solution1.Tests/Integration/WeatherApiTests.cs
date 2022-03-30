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
using System;

namespace Weather.Tests.Integration
{
    public class WeatherApiTests
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly string cityName = "Minsk";

        public WeatherApiTests()
        {
            var server = new TestServer(new WebHostBuilder()
                .UseEnvironment("Development")
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
            var comments = new List<string>() { "Dress warmly.", "It's fresh.", "Good weather.", "It's time to go to the beach." };
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Weather/{cityName}");
            
            //Act
            var response = await _httpClient.SendAsync(request);

            //Assert
            var weather = JsonSerializer.Deserialize<WeatherDTO>(await response.Content.ReadAsStringAsync(), _serializerOptions);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Assert.Equal(weather?.CityName, cityName);
            Assert.Contains(weather?.Comment, comments);            
        }

        [Fact]
        public async Task GetForecastByCityName_Success()
        {
            // Arrange
            var counDays = 3;
            var startDateTime = DateTime.Now.Date;
            var comments = new List<string>() { "Dress warmly.", "It's fresh.", "Good weather.", "It's time to go to the beach." };

            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/Weather/{cityName}/{counDays}");

            //Act
            var response = await _httpClient.SendAsync(request);

            //Assert
            var forecast = JsonSerializer.Deserialize<ForecastWeatherDTO>(await response.Content.ReadAsStringAsync(), _serializerOptions);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(forecast?.CityName, cityName);
            Assert.Equal(forecast?.WeatherForPeriod.Count, counDays+1);
            forecast.WeatherForPeriod.ForEach(x =>
            {
                Assert.Contains(x?.Comment, comments);
                Assert.Equal(startDateTime, x?.DateTime);
                startDateTime = startDateTime.AddDays(1);
            });
        }
    }
}
