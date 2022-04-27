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
using BusinessLayer.Helpers;

namespace Weather.Tests.Integration
{
    public class WeatherApiTests
    {
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly string _cityName = "Minsk";
        private readonly int _countDays = 3;
        private readonly string _currentWeatherURL = "/api/weather/current?cityName=";
        private readonly string _forecastWeatherURL = "/api/weather/forecast?";
        private readonly string _historyWeatherURL = "/api/weather/history?";
        private readonly List<string> _comments = new() { "Dress warmly.", "It's fresh.", "Good weather.", "It's time to go to the beach." };
        private readonly DateTime _startPeriodHistory = new DateTime(2020, 02, 01, 10, 40, 0);
        private readonly DateTime _endPeriodHistory = new DateTime(2020, 02, 03, 14, 0, 0);

        public static IEnumerable<object[]> InvalidDataForForecastEndpoint =>
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

        public static IEnumerable<object[]> InvalidDataForHistoryEndpoint =>
            new List<object[]>
            {
                new object[]
                {
                    new HistoryWeatherRequestDTO() { CityName = string.Empty, StartPeriod = new DateTime(2022, 01, 01, 15, 30, 00), EndPeriod = new DateTime(2022, 01, 02, 14, 10, 00), },
                    new ValidationProblemDetails(
                        new Dictionary<string, string[]>()
                        {
                            { nameof(HistoryWeatherRequestDTO.CityName), new string[] { "'City Name' must not be empty." }}
                        })
                },
                new object[]
                {
                    new HistoryWeatherRequestDTO() { CityName = "aaaaaaaaaaaaaaaaaaaaa", StartPeriod = new DateTime(2022, 01, 01, 15, 30, 00), EndPeriod = new DateTime(2022, 01, 02, 14, 10, 00), },
                    new ValidationProblemDetails(
                        new Dictionary<string, string[]>()
                        {
                            { nameof(HistoryWeatherRequestDTO.CityName), new string[] { "The length of 'City Name' must be 20 characters or fewer. You entered 21 characters." }}
                        })
                },
                new object[]
                {
                    new HistoryWeatherRequestDTO() { CityName = "Minsk", StartPeriod = new DateTime(2022, 01, 02, 15, 30, 00), EndPeriod = new DateTime(2022, 01, 01, 14, 10, 00), },
                    new ValidationProblemDetails(
                        new Dictionary<string, string[]>()
                        {
                            { string.Empty, new string[] { "'End Period' must be more or equal than 'Start Period'." }}
                        })
                },
                new object[]
                {
                    new HistoryWeatherRequestDTO() { CityName = "Minsk", StartPeriod = (DateTime)default, EndPeriod = (DateTime)default },
                    new ValidationProblemDetails(
                        new Dictionary<string, string[]>()
                        {
                            { nameof(HistoryWeatherRequestDTO.StartPeriod), new string[] { "'Start Period' must not be empty." }},
                            { nameof(HistoryWeatherRequestDTO.EndPeriod), new string[] { "'End Period' must not be empty." }}
                        })
                },
                new object[]
                {
                    new HistoryWeatherRequestDTO() { CityName = string.Empty, StartPeriod = new DateTime(2022, 01, 01), EndPeriod = (DateTime)default },
                    new ValidationProblemDetails(
                        new Dictionary<string, string[]>()
                        {
                            { nameof(HistoryWeatherRequestDTO.CityName), new string[] { "'City Name' must not be empty." }},
                            { nameof(HistoryWeatherRequestDTO.EndPeriod), new string[] { "'End Period' must not be empty." }},
                            { string.Empty, new string[] { "'End Period' must be more or equal than 'Start Period'." }},
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

            var request = new HttpRequestMessage(HttpMethod.Get, UrlHelper.Combine(_currentWeatherURL, _cityName));
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
            Assert.Equal(weather.CityName, _cityName);
            Assert.NotNull(weather.Comment);
            Assert.Contains(weather.Comment, _comments);
        }

        [Fact]
        public async Task GetForecastByCityName_Success()
        {
            // Arrange            
            var startDateTime = DateTime.Now.Date;
            var httpClient = GetClient();
            var request = new HttpRequestMessage(HttpMethod.Get, GetForecastWeatherURL(_cityName, _countDays));

            //Act
            var response = await httpClient.SendAsync(request);

            //Assert
            Assert.NotNull(response);
            Assert.NotNull(response.Content);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var forecast = JsonSerializer.Deserialize<ForecastWeatherDTO>(await response.Content.ReadAsStringAsync(), _serializerOptions);

            Assert.NotNull(forecast);
            Assert.NotNull(forecast.CityName);
            Assert.Equal(forecast.CityName, _cityName);
            Assert.NotNull(forecast.WeatherForPeriod);
            Assert.Equal(forecast.WeatherForPeriod.Count, _countDays + 1);

            forecast.WeatherForPeriod.ForEach(x =>
            {
                Assert.NotNull(x);
                Assert.NotNull(x.Comment);
                Assert.Contains(x.Comment, _comments);
                Assert.Equal(startDateTime, x.DateTime);
                startDateTime = startDateTime.AddDays(1);
            });
        }

        [Fact]
        public async Task GetHistoryWeatherByCityNameAsync_Success()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, GetHistoryWeatherURL(_cityName, _startPeriodHistory, _endPeriodHistory));
            var httpClient = GetClient();

            //Act
            var response = await httpClient.SendAsync(request);

            //Assert
            Assert.NotNull(response);
            Assert.NotNull(response.Content);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var historyWeather = JsonSerializer.Deserialize<HistoryWeatherDTO>(await response.Content.ReadAsStringAsync(), _serializerOptions);

            var expected = new HistoryWeatherDTO()
            {
                CityName = _cityName,
                WeatherList = new List<WeatherWithDatetimeDTO>()
                {
                    new WeatherWithDatetimeDTO(){ DateTime = new DateTime(2020, 02, 01, 10, 45, 0), Comment = "It's frash" , Temp = 5},
                    new WeatherWithDatetimeDTO(){ DateTime = new DateTime(2020, 02, 01, 15, 30, 0), Comment = "Good weather." , Temp = 21},
                    new WeatherWithDatetimeDTO(){ DateTime = new DateTime(2020, 02, 02, 11, 0, 0), Comment = "Dress warmly." , Temp = -5},
                }
            };

            Assert.NotNull(historyWeather);
            Assert.NotNull(historyWeather.CityName);
            Assert.NotNull(historyWeather.WeatherList);
            Assert.True(new CompareLogic().Compare(expected, historyWeather).AreEqual);
        }

        [Theory]
        [InlineData("aaaaaaaaaaaaaaaaaaaaa", "The length of 'City Name' must be 20 characters or fewer. You entered 21 characters.")]
        [InlineData("", "'City Name' must not be empty.")]
        public async Task GetWeatherByCityName_EnterInvalidData_HandlingException(string cityName, string message)
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, UrlHelper.Combine(_currentWeatherURL, cityName));
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
        [MemberData(nameof(InvalidDataForForecastEndpoint))]
        public async Task GetForecastByCityName_EnterInvalidData_HandlingException(string cityName, int countDays, ValidationProblemDetails validationDetails)
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, GetForecastWeatherURL(cityName, countDays));
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

        [Theory]
        [MemberData(nameof(InvalidDataForHistoryEndpoint))]
        public async Task GetHistoryByCityName_EnterInvalidData_HandlingException(HistoryWeatherRequestDTO historyWeatherRequestDTO, ValidationProblemDetails validationDetails)
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, GetHistoryWeatherURL(historyWeatherRequestDTO.CityName, historyWeatherRequestDTO.StartPeriod, historyWeatherRequestDTO.EndPeriod));
            var httpClient = GetClient();

            //Act
            var response = await httpClient.SendAsync(request);

            //Assert
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(response.Content);

            var validationInfo = JsonSerializer.Deserialize<ValidationProblemDetails>(await response.Content.ReadAsStringAsync());
            Assert.NotNull(validationInfo);
            Assert.True(new CompareLogic().Compare(validationDetails.Errors, validationInfo.Errors).AreEqual);
        }

        [Fact]
        public async Task GetWeatherByCityName_CanceledOperation_Success()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, UrlHelper.Combine(_currentWeatherURL, _cityName));
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
            var request = new HttpRequestMessage(HttpMethod.Get, GetForecastWeatherURL(_cityName, _countDays));
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
        public async Task GetHistoryByCityName_CanceledOperation_Success()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, GetHistoryWeatherURL(_cityName, _startPeriodHistory, _endPeriodHistory));
            var httpClient = GetClient(0);

            //Act
            var response = await httpClient.SendAsync(request);

            //Assert
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.RequestTimeout, response.StatusCode);
            Assert.NotNull(response.Content);
            Assert.Empty(await response.Content.ReadAsStringAsync());
        }

        private HttpClient GetClient(int requestTimeout = 1000000, int maxCountDaysForecast = 5, int minCountDaysForecast = 0, bool isDebugMode = true)
        {
            var configuration = new Dictionary<string, string>
            {
                {"AppConfiguration:MaxCountDaysForecast", $"{maxCountDaysForecast}"},
                {"AppConfiguration:MinCountDaysForecast", $"{minCountDaysForecast}"},
                {"AppConfiguration:IsDebugMode", $"{isDebugMode}"},
                {"AppConfiguration:RequestTimeout", $"{requestTimeout}"}
            };

            var server = new TestServer(new WebHostBuilder()
                .ConfigureAppConfiguration(configurationBuilder =>
                {
                    configurationBuilder
                    .AddUserSecrets<Program>()
                    .AddEnvironmentVariables()
                    .AddInMemoryCollection(configuration);
                })
                .UseStartup<Startup>());

            return server.CreateClient();
        }

        private string GetForecastWeatherURL(string cityName, int countDays)
        {
            return $"{_forecastWeatherURL}cityName={cityName}&countDays={countDays}";
        }

        private string GetHistoryWeatherURL(string cityName, DateTime startPeriod, DateTime endPeriod)
        {
            var dateFormat = "MM/dd/yyyy HH:mm:ss";
            return $"{_historyWeatherURL}cityName={cityName}&startPeriod={startPeriod.ToString(dateFormat)}&endPeriod={endPeriod.ToString(dateFormat)}";
        }
    }
}
