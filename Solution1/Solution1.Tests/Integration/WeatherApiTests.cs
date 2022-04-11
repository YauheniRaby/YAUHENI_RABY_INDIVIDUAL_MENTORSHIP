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
using Microsoft.AspNetCore.Mvc;
using KellermanSoftware.CompareNetObjects;

namespace Weather.Tests.Integration
{
    public class WeatherApiTests
    {
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly string cityName = "Minsk";
        private readonly int countDays = 3;
        private readonly string currentWeatherURL = "/api/weather/current?";
        private readonly string forecastWeatherURL = "/api/weather/forecast?";
        private readonly string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=weatherdb;Trusted_Connection=True;";
        private readonly List<string> comments = new List<string>() { "Dress warmly.", "It's fresh.", "Good weather.", "It's time to go to the beach." };

        public static IEnumerable<object[]> DataForValidationTest =>
            new List<object[]>
            {
                new object[]
                {
                    "Minsk",
                    -1,
                    new ValidationProblemDetails(
                        new Dictionary<string, string[]>()
                        {
                            { nameof(ForecastWeatherRequestDTO.PeriodOfDays), new string[] { "'Period Of Days' must be between 0 and 5. You entered -1." }}
                        })
                },
                new object[]
                {
                    "Minsk",
                    10,
                    new ValidationProblemDetails(
                        new Dictionary<string, string[]>()
                        {
                            { nameof(ForecastWeatherRequestDTO.PeriodOfDays), new string[] { "'Period Of Days' must be between 0 and 5. You entered 10." }}
                        })
                },
                new object[]
                {
                    "aaaaaaaaaaaaaaaaaaaaa",
                    3,
                    new ValidationProblemDetails(
                        new Dictionary<string, string[]>()
                        {
                            { nameof(ForecastWeatherRequestDTO.CityName), new string[] { "The length of 'City Name' must be 20 characters or fewer. You entered 21 characters." }}
                        })
                },
                new object[]
                {
                    "aaaaaaaaaaaaaaaaaaaaa",
                    7,
                    new ValidationProblemDetails(
                        new Dictionary<string, string[]>()
                        {
                            { nameof(ForecastWeatherRequestDTO.CityName), new string[] { "The length of 'City Name' must be 20 characters or fewer. You entered 21 characters." }},
                            { nameof(ForecastWeatherRequestDTO.PeriodOfDays), new string[] { "'Period Of Days' must be between 0 and 5. You entered 7." }}
                        })
                },
                new object[]
                {
                    string.Empty,
                    10,                    
                    new ValidationProblemDetails(
                        new Dictionary<string, string[]>()
                        {
                            { nameof(ForecastWeatherRequestDTO.CityName), new string[] { "'City Name' must not be empty." }},
                            { nameof(ForecastWeatherRequestDTO.PeriodOfDays), new string[] { "'Period Of Days' must be between 0 and 5. You entered 10." }}
                        })
                },
                new object[]
                {
                    string.Empty,
                    3,
                    new ValidationProblemDetails(
                        new Dictionary<string, string[]>()
                        {
                            { nameof(ForecastWeatherRequestDTO.CityName), new string[] { "'City Name' must not be empty." }}
                        })
                }
            };

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
            var request = new HttpRequestMessage(HttpMethod.Get, $"{currentWeatherURL}{nameof(cityName)}={cityName}");
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
            var request = new HttpRequestMessage(HttpMethod.Get, $"{forecastWeatherURL}{nameof(cityName)}={cityName}&{nameof(countDays)}={countDays}");

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
            Assert.Equal(forecast.WeatherForPeriod.Count, countDays+1);
            
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
        [InlineData("aaaaaaaaaaaaaaaaaaaaa", "The length of 'City Name' must be 20 characters or fewer. You entered 21 characters.")]
        [InlineData("", "'City Name' must not be empty.")]
        public async Task GetWeatherByCityName_EnterInvalidData_HandlingException(string cityName, string message)
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, $"{currentWeatherURL}{nameof(cityName)}={cityName}");
            var httpClient = GetClient();

            var validationDetails = new ValidationProblemDetails(
            new Dictionary<string, string[]>()
            {
                { nameof(ForecastWeatherRequestDTO.CityName), new string[] { message }}
            });                

            //Act
            var response = await httpClient.SendAsync(request);

            //Assert
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(response.Content);

            var validationInfo = JsonSerializer.Deserialize<ValidationProblemDetails>(await response.Content.ReadAsStringAsync());
            Assert.NotNull(validationInfo);
            Assert.True(new CompareLogic().Compare(validationDetails, validationInfo).AreEqual);            
        }

        [Theory]
        [MemberData(nameof(DataForValidationTest))]
        public async Task GetForecastByCityName_EnterInvalidData_HandlingException(string cityName, int countDays, ValidationProblemDetails validationDetails)
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, $"{forecastWeatherURL}{nameof(cityName)}={cityName}&{nameof(countDays)}={countDays}");
            var httpClient = GetClient();
            
            //Act
            var response = await httpClient.SendAsync(request);
            //Assert
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(response.Content);

            var validationInfo = JsonSerializer.Deserialize<ValidationProblemDetails>(await response.Content.ReadAsStringAsync());
            Assert.NotNull(validationInfo);
            Assert.True(new CompareLogic().Compare(validationDetails, validationInfo).AreEqual);
        }

        [Fact]
        public async Task GetWeatherByCityName_CanceledOperation_Success()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, $"{currentWeatherURL}{nameof(cityName)}={cityName}");
            var httpClient = GetClient(0);

            //Act
            var response = await httpClient.SendAsync(request);
            
            //Assert
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.RequestTimeout, response.StatusCode);
            Assert.NotNull(response.Content);
            Assert.Empty(await response.Content.ReadAsStringAsync());
        }

        [Fact]
        public async Task GetForecastByCityName_CanceledOperation_Success()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, $"{forecastWeatherURL}{nameof(cityName)}={cityName}&{nameof(countDays)}={countDays}");
            var httpClient = GetClient(0);

            //Act
            var response = await httpClient.SendAsync(request);

            //Assert
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.RequestTimeout, response.StatusCode);
            Assert.NotNull(response.Content);
            Assert.Empty(await response.Content.ReadAsStringAsync());
        }

        private HttpClient GetClient(int requestTimeout = 5000, int maxCountDaysForecast = 5, int minCountDaysForecast = 0, bool isDebugMode = true)
        {
            var configuration = new Dictionary<string, string>
            {
                {"AppConfiguration:MaxCountDaysForecast", $"{maxCountDaysForecast}"},
                {"AppConfiguration:MinCountDaysForecast", $"{minCountDaysForecast}"},
                {"AppConfiguration:IsDebugMode", $"{isDebugMode}"},
                {"AppConfiguration:RequestTimeout", $"{requestTimeout}"},
                {"ConnectionStrings:DefaultConnection", $"{connectionString}"}
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
