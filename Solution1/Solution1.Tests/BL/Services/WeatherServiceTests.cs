using AutoMapper;
using BusinessLayer;
using BusinessLayer.Configuration.Abstract;
using BusinessLayer.DTOs;
using BusinessLayer.DTOs.Enum;
using BusinessLayer.DTOs.WeatherAPI;
using BusinessLayer.Services;
using BusinessLayer.Services.Abstract;
using ConsoleApp.AutoMap;
using FluentValidation;
using FluentValidation.Results;
using KellermanSoftware.CompareNetObjects;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Weather.Tests.BL.Services
{
    public class WeatherServiceTests
    {
        private readonly IMapper _mapper;
        private readonly WeatherService _weatherService;
        private readonly Mock<IValidator<ForecastWeatherRequestDTO>> _validator;
        private readonly Mock<IWeatherApiService> _weatherApiServiceMock;
        private readonly Mock<IConfig> _config;
        private readonly string cityName = "Minsk";
        private readonly double temp = 11;
        private readonly string comment = Constants.WeatherComments.Fresh;

        public WeatherServiceTests()
        {
            _weatherApiServiceMock = new Mock<IWeatherApiService>();
            _validator = new Mock<IValidator<ForecastWeatherRequestDTO>>();
            _mapper = new Mapper(MapperConfig.GetConfiguration());
            _config = new Mock<IConfig>();
            _weatherService = new WeatherService(_mapper, _weatherApiServiceMock.Object, _validator.Object, _config.Object);
        }

        [Fact]
        public async Task GetByCityNameAsync_ReturnedWeatherDTO_Success()
        {
            // Arrange            
            var forecast = new ForecastWeatherRequestDTO() { CityName = cityName };
            var weatherApiDto = new WeatherApiDTO() { CityName = cityName, TemperatureValues = new WeatherApiTempDTO() { Temp = temp } };
            var validationResult = new ValidationResult(new List<ValidationFailure>());

            _validator
                 .Setup(validator => validator.ValidateAsync
                     (It.Is<ValidationContext<ForecastWeatherRequestDTO>>(context => context.InstanceToValidate.CityName == cityName),
                     It.IsAny<CancellationToken>()))
                 .ReturnsAsync(validationResult);

            _weatherApiServiceMock
                .Setup(weatherApiService =>
                    weatherApiService.GetByCityNameAsync
                    (It.Is<string>(x => x == cityName),
                     It.IsAny<CancellationToken>()))
                .ReturnsAsync(weatherApiDto);

            // Act
            var result = await _weatherService.GetByCityNameAsync(cityName);

            // Assert
            var expectedWeatherDto = new WeatherDTO() { CityName = cityName, Temp = temp, Comment = comment };
            Assert.True(new CompareLogic().Compare(expectedWeatherDto, result).AreEqual);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        public async Task GetForecastByCityNameAsync_ReturnedForecastWeatherDTO_Success(int countDays)
        {
            // Arrange
            var startForecast = new DateTime(2022, 10, 12, 00, 00, 00);
            var countPointForCurrentDay =
                (DateTime.UtcNow.Date.AddDays(1) - DateTime.UtcNow).Hours /
                (24 / Constants.WeatherAPI.WeatherPointsInDay);
            var countWeatherPoints = countDays * Constants.WeatherAPI.WeatherPointsInDay + countPointForCurrentDay;

            var forecastWeatherApiDTO = new ForecastWeatherApiDTO()
            {
                City = new CityApiDTO() { Name = cityName },
                WeatherPoints = new List<WeatherInfoApiDTO>()
                {
                    new WeatherInfoApiDTO() { DateTime = startForecast, Temp = new TempApiDTO { Value = 11 }},
                    new WeatherInfoApiDTO() { DateTime = startForecast.AddHours(3), Temp = new TempApiDTO { Value = 13 }},
                    new WeatherInfoApiDTO() { DateTime = startForecast.AddDays(1), Temp = new TempApiDTO { Value = 14 }},
                    new WeatherInfoApiDTO() { DateTime = startForecast.AddDays(1).AddHours(3), Temp = new TempApiDTO { Value = 16 }},
                }
            };
            var validationResult = new ValidationResult(new List<ValidationFailure>());

            _validator
                .Setup(validator => validator.ValidateAsync
                    (It.Is<ValidationContext<ForecastWeatherRequestDTO>>
                        (context =>
                            context.InstanceToValidate.CityName == cityName
                            && context.InstanceToValidate.PeriodOfDays == countDays),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            _weatherApiServiceMock
                .Setup(weatherApiService =>
                    weatherApiService.GetForecastByCityNameAsync
                    (It.Is<string>(x => x == cityName),
                     It.Is<int>(x => x == countWeatherPoints),
                     It.IsAny<CancellationToken>()))
                .ReturnsAsync(forecastWeatherApiDTO);

            //Act
            var result = await _weatherService.GetForecastByCityNameAsync(cityName, countDays);

            //Assert
            var expectedWeatherDto = new ForecastWeatherDTO()
            {
                CityName = cityName,
                WeatherForPeriod = new List<WeatherForDateDTO>()
                {
                    new WeatherForDateDTO() { DateTime = startForecast, Temp = 12, Comment = comment },
                    new WeatherForDateDTO() { DateTime = startForecast.AddDays(1), Temp = 15, Comment = comment }
                }
            };

            Assert.True(new CompareLogic().Compare(expectedWeatherDto, result).AreEqual);
        }

        [Fact]
        public async Task GetWeatherByArrayCityNameAsync_ReturnedDictionaryForWeatherResponseDTO_Success()
        {
            var cityName2 = "Paris";
            var cityName3 = "AАА";
            var cityName4 = string.Empty;
            var cityName5 = "return null";

            var errorMessage3 = "404 (Not Found)";
            var errorMessage4 = "Test validation error";
            var errorMessage5 = "Unknown error";
            
            var weatherApiDto1 = new WeatherApiDTO() { CityName = cityName, TemperatureValues = new WeatherApiTempDTO() };
            var weatherApiDto2 = new WeatherApiDTO() { CityName = cityName2, TemperatureValues = new WeatherApiTempDTO() };
            
            var listCityName = new List<string>() { cityName, cityName2, cityName3, cityName4, cityName5 };
            
            var validationFailure = new List<ValidationFailure>() { new ValidationFailure("CityName", "Test validation error")};
            var notValidResult = new ValidationResult(validationFailure);
            var validResult = new ValidationResult(new List<ValidationFailure>());

            SetWeatherApiServiceSettings(weatherApiDto1);
            SetWeatherApiServiceSettings(weatherApiDto2);
            SetWeatherApiServiceExceptionSettings(cityName3, new Exception(errorMessage3));
            SetWeatherApiServiceExceptionSettings(cityName4, new  ValidationException(validationFailure));

            _validator
                 .Setup(validator => validator.ValidateAsync
                     (It.Is<ValidationContext<ForecastWeatherRequestDTO>>(context => context.InstanceToValidate.CityName == cityName4),
                     It.IsAny<CancellationToken>()))
                 .ReturnsAsync(notValidResult);
            
            _validator
                 .Setup(validator => validator.ValidateAsync(
                     It.Is<ValidationContext<ForecastWeatherRequestDTO>>(
                         context => 
                            context.InstanceToValidate.CityName == cityName
                            || context.InstanceToValidate.CityName == cityName2
                            || context.InstanceToValidate.CityName == cityName3
                            || context.InstanceToValidate.CityName == cityName5),
                     It.IsAny<CancellationToken>()))
                 .ReturnsAsync(validResult);

            // Act
            var result = await _weatherService.GetWeatherByArrayCityNameAsync(listCityName);

            // Assert
            Assert.Contains(result.Keys, k => k == ResponseStatus.Successful);
            Assert.Contains(result.Keys, k => k == ResponseStatus.Fail);

            Assert.Equal(2, result[ResponseStatus.Successful].Count());
            Assert.Equal(3, result[ResponseStatus.Fail].Count());

            Assert.Contains(result[ResponseStatus.Successful], weatherResponse => weatherResponse.CityName == cityName);
            Assert.Contains(result[ResponseStatus.Successful], weatherResponse => weatherResponse.CityName == cityName2);
            Assert.Contains(result[ResponseStatus.Fail], weatherResponse => weatherResponse.CityName == cityName3 
                                                              && weatherResponse.ErrorMessage == errorMessage3); 
            Assert.Contains(result[ResponseStatus.Fail], weatherResponse => weatherResponse.CityName == cityName4 
                                                              && weatherResponse.ErrorMessage == errorMessage4);
            Assert.Contains(result[ResponseStatus.Fail], weatherResponse => weatherResponse.CityName == cityName5 
                                                              && weatherResponse.ErrorMessage == errorMessage5);
        }

        private void SetWeatherApiServiceSettings(WeatherApiDTO weatherApiDTO)
        {
            _weatherApiServiceMock
                .Setup(weatherApiService =>
                    weatherApiService.GetByCityNameAsync
                    (It.Is<string>(x => x == weatherApiDTO.CityName),
                     It.IsAny<CancellationToken>()))
                .ReturnsAsync(weatherApiDTO);
        }

        private void SetWeatherApiServiceExceptionSettings(string cityName, Exception exception)
        {
            _weatherApiServiceMock
                .Setup(weatherApiService =>
                    weatherApiService.GetByCityNameAsync
                    (It.Is<string>(x => x == cityName),
                     It.IsAny<CancellationToken>()))
                .ThrowsAsync(exception);
        }
    }
}
