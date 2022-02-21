using AutoMapper;
using BusinessLayer.DTOs;
using BusinessLayer.Service;
using BusinessLayer.Service.Abstract;
using ConsoleApp.AutoMap;
using KellermanSoftware.CompareNetObjects;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Solution1.Tests.BL.Tests
{
    public class WeatherServiceTests
    {
        private readonly IMapper _mapper;
        private readonly WeatherService _weatherService;
        private readonly Mock<IWeatherApiService> _weatherApiServiceMock;

        public WeatherServiceTests()
        {
            _weatherApiServiceMock = new Mock<IWeatherApiService>();
            _mapper = new Mapper(MapperConfig.GetConfiguration());
            _weatherService = new WeatherService(_mapper, _weatherApiServiceMock.Object);
        }

        [Fact]
        public async Task GetByCityNameAsync_ReternedWeatherDTO()
        {
            // Arrange
            var cityName = "Minsk";
            var temp = 10;
            var WeatherApiDto = new WeatherApiDTO() { CityName = cityName, TemperatureValues = new WeatherApiTempDTO() { Temp = temp } };
            _weatherApiServiceMock
                .Setup(weatherApi => 
                    weatherApi
                    .GetByCityNameAsync(cityName))
                    .ReturnsAsync(WeatherApiDto);

            var expectedObject = new WeatherDTO() { CityName = cityName, Temp = temp, Comment = "It's fresh"};
            
            // Act
            var actualObject = await _weatherService.GetByCityNameAsync(cityName);

            // Assert
            Assert.True(new CompareLogic().Compare(expectedObject, actualObject).AreEqual);
        }               
    }
}
