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

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        public async Task GetForecastByCityNameAsync_ReturnedForecastWeatherDTO_Success(int countDay)
        {
            // Arrange
            var cityName = "Minsk";
            var countWeatherPoint = 8*countDay + (DateTime.UtcNow.Date.AddDays(1) - DateTime.UtcNow).Hours / 3;
            var forecastWeatherApiDTO = new ForecastWeatherApiDTO()
            {
                City = new CityApiDTO() { Name = cityName },
                WeatherPoints = new List<WeatherInfoApiDTO>()
            };

            for(int i = 1; i <= countDay; i ++)
            {
                for(int j = 0; j < 24; j += 3)
                {
                    forecastWeatherApiDTO.WeatherPoints
                        .Add(
                            new WeatherInfoApiDTO()
                            {
                                DateTime = new DateTime(2022, 3, i, j, 00, 00),
                                Temp = new TempApiDTO() { Value = 2 }
                            });
                }
            }

            _weatherApiServiceMock
                .Setup(weatherApiService =>
                    weatherApiService
                    .GetForecastByCityNameAsync(cityName, countWeatherPoint))
                .ReturnsAsync(forecastWeatherApiDTO);

            //Act
            var result = await _weatherService.GetForecastByCityNameAsync(cityName, countDay);

            //Assert
            var expectedWeatherDto = new ForecastWeatherDTO()
            {
                CityName = cityName,
                WeatherForPeriod = new List<WeatherForDateDTO>()};

            for (int i = 1; i <= countDay; i++)
            {
                expectedWeatherDto.WeatherForPeriod
                    .Add(new WeatherForDateDTO() { DateTime = new DateTime(2022, 3, i), Temp = 2, Comment = "It's fresh." });                
            }

            Assert.True(new CompareLogic().Compare(expectedWeatherDto, result).AreEqual);
        }
    }
}
