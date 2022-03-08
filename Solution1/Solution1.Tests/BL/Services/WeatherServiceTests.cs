using AutoMapper;
using BusinessLayer.DTOs;
using BusinessLayer.DTOs.WeatherAPI;
using BusinessLayer.Services;
using BusinessLayer.Services.Abstract;
using BusinessLayer.Vlidators;
using ConsoleApp.AutoMap;
using ConsoleApp.Command.Abstract;
using FluentValidation;
using KellermanSoftware.CompareNetObjects;
using Moq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using Xunit;

namespace Weather.Tests.BL.Services
{
    public class WeatherServiceTests
    {
        private readonly IMapper _mapper;
        private readonly WeatherService _weatherService;
        private readonly IValidator<ForecastWeatherRequestDTO> _validator;
        private readonly Mock<IWeatherApiService> _weatherApiServiceMock;

        public WeatherServiceTests()
        {
            _weatherApiServiceMock = new Mock<IWeatherApiService>();
            _validator = new ForecastWeatherRequestDTOValidator();
            _mapper = new Mapper(MapperConfig.GetConfiguration());
            _weatherService = new WeatherService(_mapper, _weatherApiServiceMock.Object, _validator);
        }

        [Fact]
        public async Task GetByCityNameAsync_ReturnedWeatherDTO_Success()
        {
            // Arrange
            var cityName = "Minsk";
            var forecast = new ForecastWeatherRequestDTO() { CityName = cityName };


            var temp = 11;
            var weatherApiDto = new WeatherApiDTO() { CityName = cityName, TemperatureValues = new WeatherApiTempDTO() { Temp = temp } };

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

        [Fact]
        public async Task GetForecastByCityNameAsync_ReturnedForecastWeatherDTO_Success()
        {
            // Arrange
            var cityName = "Minsk";
            var countWeatherPoint = 8 + (DateTime.UtcNow.Date.AddDays(1) - DateTime.UtcNow).Hours / 3;

            var forecastWeatherApiDTO = new ForecastWeatherApiDTO()
            {
                City = new CityApiDTO() { Name = cityName },
                WeatherPoints = new List<WeatherInfoApiDTO>()
                {
                    new WeatherInfoApiDTO()
                    {
                        DateTime = new DateTime(2022, 3, 5, 18, 00, 00),
                        Temp = new TempApiDTO() { Value = 2 }
                    },
                    new WeatherInfoApiDTO()
                    {
                        DateTime = new DateTime(2022, 3, 5, 21, 00, 00),
                        Temp = new TempApiDTO() { Value = 4 }
                    },
                }
            };

            _weatherApiServiceMock
                .Setup(weatherApiService =>
                    weatherApiService
                    .GetForecastByCityNameAsync(cityName, countWeatherPoint))
                .ReturnsAsync(forecastWeatherApiDTO);

            //Act
            var result = await _weatherService.GetForecastByCityNameAsync(cityName, 1);

            //Assert
            var expectedWeatherDto = new ForecastWeatherDTO()
            {
                CityName = cityName,
                WeatherForPeriod = new List<WeatherForDateDTO>()
                {
                    new WeatherForDateDTO() { DateTime = new DateTime(2022, 3, 5), Temp = 3, Comment = "It's fresh."}
                }
            };
            Assert.True(new CompareLogic().Compare(expectedWeatherDto, result).AreEqual);
        }
    }
}
