using AutoMapper;
using BusinessLayer.DTOs;
using BusinessLayer.DTOs.WeatherAPI;
using BusinessLayer.Services;
using BusinessLayer.Services.Abstract;
using BusinessLayer.Vlidators;
using ConsoleApp.AutoMap;
using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;
using KellermanSoftware.CompareNetObjects;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
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

        public WeatherServiceTests()
        {
            _weatherApiServiceMock = new Mock<IWeatherApiService>();
            _validator = new Mock<IValidator<ForecastWeatherRequestDTO>>();
            _mapper = new Mapper(MapperConfig.GetConfiguration());
            _weatherService = new WeatherService(_mapper, _weatherApiServiceMock.Object, _validator.Object);
        }

        [Fact]
        public async Task GetByCityNameAsync_ReturnedWeatherDTO_Success()
        {
            // Arrange
            var cityName = "Minsk";
            var forecast = new ForecastWeatherRequestDTO() { CityName = cityName};
            var temp = 11;
            var weatherApiDto = new WeatherApiDTO() { CityName = cityName, TemperatureValues = new WeatherApiTempDTO() { Temp = temp } };
            var validationResult = new ValidationResult(new List<ValidationFailure>());

            _validator
                .Setup(validator => validator.ValidateAsync
                    (It.IsAny<ValidationContext<ForecastWeatherRequestDTO>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);
            
            _weatherApiServiceMock
                .Setup(weatherApiService => 
                    weatherApiService
                    .GetByCityNameAsync(cityName))
                .ReturnsAsync(weatherApiDto);           
            
            // Act
            var result = await _weatherService.GetByCityNameAsync(cityName);

            // Assert
            var expectedWeatherDto = new WeatherDTO() { CityName = cityName, Temp = temp, Comment = "It's fresh." };
            Assert.True(new CompareLogic().Compare(expectedWeatherDto, result).AreEqual);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        public async Task GetForecastByCityNameAsync_ReturnedForecastWeatherDTO_Success(int countDays)
        {
            // Arrange
            var cityName = "Minsk";
            var countWeatherPoint = 8*countDays + (DateTime.UtcNow.Date.AddDays(1) - DateTime.UtcNow).Hours / 3;
            var forecastWeatherApiDTO = new ForecastWeatherApiDTO()
            {
                City = new CityApiDTO() { Name = cityName },
                WeatherPoints = new List<WeatherInfoApiDTO>()
            };
            var validationResult = new ValidationResult(new List<ValidationFailure>());

            for (int currentDayNumber = 1; currentDayNumber <= countDays; currentDayNumber++)
            {
                for(int currentHour = 0; currentHour < 24; currentHour += 3)
                {
                    forecastWeatherApiDTO.WeatherPoints
                        .Add(
                            new WeatherInfoApiDTO()
                            {
                                DateTime = new DateTime(2022, 3, currentDayNumber, currentHour, 00, 00),
                                Temp = new TempApiDTO() { Value = 2 }
                            });
                }
            }

            _validator
                .Setup(validator => validator.ValidateAsync
                    (It.IsAny<ValidationContext<ForecastWeatherRequestDTO>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            _weatherApiServiceMock
                .Setup(weatherApiService =>
                    weatherApiService
                    .GetForecastByCityNameAsync(cityName, countWeatherPoint))
                .ReturnsAsync(forecastWeatherApiDTO);

            //Act
            var result = await _weatherService.GetForecastByCityNameAsync(cityName, countDays);

            //Assert
            var expectedWeatherDto = new ForecastWeatherDTO()
            {
                CityName = cityName,
                WeatherForPeriod = new List<WeatherForDateDTO>()};

            for (int currentDayNumber = 1; currentDayNumber <= countDays; currentDayNumber++)
            {
                expectedWeatherDto.WeatherForPeriod
                    .Add(new WeatherForDateDTO() { DateTime = new DateTime(2022, 3, currentDayNumber), Temp = 2, Comment = "It's fresh." });                
            }

            Assert.True(new CompareLogic().Compare(expectedWeatherDto, result).AreEqual);
        }
    }
}
