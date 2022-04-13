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
            var startForecast = new DateTime(2022, 10, 11);
            
            var weatherInfoApiDTO = new List<WeatherInfoApiDTO>()
                {
                    new WeatherInfoApiDTO() { DateTime = startForecast, Temp = new TempApiDTO { Value = 11 }},
                    new WeatherInfoApiDTO() { DateTime = startForecast.AddHours(3), Temp = new TempApiDTO { Value = 13 }},
                    new WeatherInfoApiDTO() { DateTime = startForecast.AddDays(1), Temp = new TempApiDTO { Value = 14 }},
                    new WeatherInfoApiDTO() { DateTime = startForecast.AddDays(1).AddHours(3), Temp = new TempApiDTO { Value = 16 }},
                };

            // Act
            var result = weatherInfoApiDTO.GetMeanValueWeather().ToList();

            // Assert
            var expected = new List<WeatherForDateDTO>()
                {
                    new WeatherForDateDTO() { DateTime = startForecast, Temp = 12 },
                    new WeatherForDateDTO() { DateTime = startForecast.AddDays(1), Temp = 15 }
                };

            Assert.True(new CompareLogic().Compare(expected, result).AreEqual);
        }
    }
}
