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
        private readonly int counDays = 3;
        private readonly List<string> comments = new List<string>() { "Dress warmly.", "It's fresh.", "Good weather.", "It's time to go to the beach." };

        public static IEnumerable<object[]> DataForValidationTest =>
            new List<object[]>
            {
                new object[]
                {
                    "Minsk",
                    -1,
                    HttpStatusCode.BadRequest,
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
                    HttpStatusCode.BadRequest,
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
                    HttpStatusCode.BadRequest,
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
                    HttpStatusCode.BadRequest,
                    new ValidationProblemDetails(
                        new Dictionary<string, string[]>()
                        {
                            { nameof(ForecastWeatherRequestDTO.CityName), new string[] { "The length of 'City Name' must be 20 characters or fewer. You entered 21 characters." }},
                            { nameof(ForecastWeatherRequestDTO.PeriodOfDays), new string[] { "'Period Of Days' must be between 0 and 5. You entered 7." }}
                        })
                },
                new object[] { "", 10, HttpStatusCode.NotFound, null },
                new object[] { "", 3, HttpStatusCode.NotFound, null },
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

            var validationDetails = new ValidationProblemDetails(
            new Dictionary<string, string[]>()
            {
                { nameof(ForecastWeatherRequestDTO.CityName), new string[] { "The length of 'City Name' must be 20 characters or fewer. You entered 21 characters." }}
            });                

            //Act
            var response = await httpClient.SendAsync(request);

            //Assert
            Assert.NotNull(response);
            Assert.Equal(httpStatusCode, response.StatusCode);
            Assert.NotNull(response.Content);

            if (httpStatusCode == HttpStatusCode.BadRequest)
            {
                var validationInfo = JsonSerializer.Deserialize<ValidationProblemDetails>(await response.Content.ReadAsStringAsync());
                Assert.True(new CompareLogic().Compare(validationDetails, validationInfo).AreEqual);
            }
            else
            {
                Assert.Empty(await response.Content.ReadAsStringAsync());
            }
        }

        [Theory]
        [MemberData(nameof(DataForValidationTest))]
        public async Task GetForecastByCityName_EnterInvalidData_HandlingException(string cityName, int countDays, HttpStatusCode httpStatusCode, ValidationProblemDetails validationDetails)
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/weather/{cityName}/{countDays}");
            var httpClient = GetClient();
            
            //Act
            var response = await httpClient.SendAsync(request);
            //Assert
            Assert.NotNull(response);
            Assert.Equal(httpStatusCode, response.StatusCode);
            Assert.NotNull(response.Content);

            if (validationDetails != null)
            {
                var validationInfo = JsonSerializer.Deserialize<ValidationProblemDetails>(await response.Content.ReadAsStringAsync());
                Assert.True(new CompareLogic().Compare(validationDetails, validationInfo).AreEqual);
            }
            else
            {
                Assert.Empty(await response.Content.ReadAsStringAsync());
            }
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
            Assert.NotNull(response.Content);
            Assert.Empty(await response.Content.ReadAsStringAsync());
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
            Assert.NotNull(response.Content);
            Assert.Empty(await response.Content.ReadAsStringAsync());
        }

        private HttpClient GetClient(int RequestTimeout = 5000, int maxCountDaysForecast = 5, int minCountDaysForecast = 0, bool isDebugMode = true)
        {
            var configuration = new Dictionary<string, string>
            {
                {"AppConfiguration:MaxCountDaysForecast", $"{maxCountDaysForecast}"},
                {"AppConfiguration:MinCountDaysForecast", $"{minCountDaysForecast}"},
                {"AppConfiguration:IsDebugMode", $"{isDebugMode}"},
                {"AppConfiguration:RequestTimeout", $"{RequestTimeout}"}
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
