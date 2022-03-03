using AutoMapper;
using BusinessLayer.DTOs;
using BusinessLayer.DTOs.WeatherAPI;
using BusinessLayer.Services;
using BusinessLayer.Services.Abstract;
using ConsoleApp.AutoMap;
using ConsoleApp.Command.Abstract;
using FluentValidation;
using KellermanSoftware.CompareNetObjects;
using Moq;
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
    }
}
