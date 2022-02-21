using AutoMapper;
using BusinessLayer.DTOs;
using BusinessLayer.Service;
using BusinessLayer.Service.Abstract;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Solution1.Tests.BL.Tests
{
    public class WeatherServiceTests
    {
        [Fact]
        public async Task GetByCityNameAsync_ReternedWeatherToStringAsync()
        {
            // Arrange
            var cityName = "Minsk";
            var temp = 10;
            var mockApi = new Mock<IWeatherApiService>();
            var mockApiResult = new WeatherApiDTO() { CityName = cityName, TemperatureValues = new WeatherApiTempDTO() { Temp = temp } };
            mockApi.Setup(weatherApi =>
                        weatherApi
                         .GetByCityNameAsync(cityName))
                         .ReturnsAsync(mockApiResult);

            var mockAutoMapper = new Mock<IMapper>();
            var mockAutoMapperResult = new WeatherDTO() { CityName = cityName, Temp = temp};
            mockAutoMapper.Setup(weatherApiDTO =>
                                weatherApiDTO
                                    .Map<WeatherDTO>(mockApiResult))
                                    .Returns(mockAutoMapperResult);
            
            var weatherServise = new WeatherService(mockAutoMapper.Object, mockApi.Object);

            var expectedCount = new WeatherDTO() { CityName = cityName, Temp = temp, Comment = "It's fresh"};
            // Act
            var actualCount = await weatherServise.GetByCityNameAsync(cityName);

            // Assert
            Assert.Equal(expectedCount.Comment, actualCount.Comment);
        }               
    }
}
