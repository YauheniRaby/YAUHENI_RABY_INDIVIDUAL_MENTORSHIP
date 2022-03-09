using BusinessLayer.DTOs;
using BusinessLayer.Extensions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Weather.Tests.BL.Extensions
{
    public class ForecastWeatherDTOExtensionsTests
    {
        [Fact]
        public void GetMultiStringRepresentation_GetMultiStringRepresentationFromWeatherDTO_Success()
        {
            // Arrange
            var forecastWeatherDTO = GetForecastWeatherDTO().FillCommentByTemp();

            // Act
            var result = forecastWeatherDTO.GetMultiStringRepresentation();

            // Assert
            var expected = $"Minsk weather forecast: " +
                $"\nDay 0 (10 октября 2022 г.): 18,0 C. It's fresh. " +
                $"\nDay 1 (11 октября 2022 г.): 25,0 C. Good weather.";
            
            Assert.Equal(expected, result);
        }

        [Fact]
        public void FillCommentByTemp_FillsComment_Success()
        {
            // Arrange
            var forecastWeatherDTO = GetForecastWeatherDTO();

            // Act
            forecastWeatherDTO.FillCommentByTemp();

            // Assert            
            Assert.Equal("It's fresh.", forecastWeatherDTO.WeatherForPeriod[0].Comment);
            Assert.Equal("Good weather.", forecastWeatherDTO.WeatherForPeriod[1].Comment);
        }

        private ForecastWeatherDTO GetForecastWeatherDTO()
        {
            var weatherForPeriod = new List<WeatherForDateDTO>()
            {
                new WeatherForDateDTO(){ DateTime = new DateTime(2022, 10, 10), Temp = 18 },
                new WeatherForDateDTO(){ DateTime = new DateTime(2022, 10, 11), Temp = 25 }
            };
            return new ForecastWeatherDTO() { CityName = "Minsk", WeatherForPeriod = weatherForPeriod };
        }
    }
}
