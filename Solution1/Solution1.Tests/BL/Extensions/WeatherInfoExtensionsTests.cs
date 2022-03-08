using BusinessLayer.DTOs;
using BusinessLayer.DTOs.WeatherAPI;
using BusinessLayer.Extensions;
using KellermanSoftware.CompareNetObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Weather.Tests.BL.Extensions
{
    public class WeatherInfoExtensionsTests
    {
        [Fact]
        public void GetMeanValueWeather_GetMeanValueWeatherFromWeatherInfoApiDTO_Success()
        {
            // Arrange
            var weatherInfoApiDTO = new List<WeatherInfoApiDTO>()
            {
                new WeatherInfoApiDTO() { DateTime = new DateTime(2022, 10, 10, 09,00,00), Temp = new TempApiDTO() { Value = 10 } },
                new WeatherInfoApiDTO() { DateTime = new DateTime(2022, 10, 10, 12,00,00), Temp = new TempApiDTO() { Value = 16 } },
                new WeatherInfoApiDTO() { DateTime = new DateTime(2022, 10, 11, 15,00,00), Temp = new TempApiDTO() { Value = 12 } },
                new WeatherInfoApiDTO() { DateTime = new DateTime(2022, 10, 11, 18,00,00), Temp = new TempApiDTO() { Value = 16 } }
            };

            // Act
            var result = weatherInfoApiDTO.GetMeanValueWeather().ToList();

            // Assert
            var expected = new List<WeatherForDateDTO>()
            {
                new WeatherForDateDTO() { DateTime = new DateTime(2022, 10, 10), Temp = 13 },
                new WeatherForDateDTO() { DateTime = new DateTime(2022, 10, 11), Temp = 14 }
            }; 
            
            Assert.True(new CompareLogic().Compare(expected, result).AreEqual);
        }
    }
}
