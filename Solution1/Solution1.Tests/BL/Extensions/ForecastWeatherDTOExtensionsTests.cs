using BusinessLayer.DTOs;
using BusinessLayer.Extensions;
using KellermanSoftware.CompareNetObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Weather.Tests.BL.Extensions
{
    public class ForecastWeatherDTOExtensionsTests
    {
        [Fact]
        public void GetMultiStringRepresentation_GetMultiStringRepresentationFromWeatherDTO_Success()
        {
            // Arrange
            
            // Act
            var result = GetForecastWeatherDTO().FillCommentByTemp().GetMultiStringRepresentation();

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
            
            // Act
            var result = GetForecastWeatherDTO().FillCommentByTemp();

            // Assert
            var expected = GetForecastWeatherDTO();
            expected.WeatherForPeriod[0].Comment = "It's fresh.";
            expected.WeatherForPeriod[1].Comment = "Good weather.";

            Assert.True(new CompareLogic().Compare(expected, result).AreEqual);
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
