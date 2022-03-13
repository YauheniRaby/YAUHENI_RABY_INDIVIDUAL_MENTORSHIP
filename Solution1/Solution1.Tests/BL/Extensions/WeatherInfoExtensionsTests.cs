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
            var dateTime = new DateTime(2022, 10, 11);
            var temp = 15;
            var riseTempForNextDay = 2;
            var countdays = 10;
            var countPountInDay = 8;

            var weatherInfoApiDTO = new List<WeatherInfoApiDTO>();
            for (int currentCountDays = 0; currentCountDays < countdays; currentCountDays++)
            {
                var currentTemp = temp + currentCountDays * riseTempForNextDay;
                for (int currentCountPoints = 0; currentCountPoints < countPountInDay; currentCountPoints++)
                {
                    weatherInfoApiDTO
                        .Add(
                            new WeatherInfoApiDTO() 
                            { 
                                DateTime = dateTime.AddDays(currentCountDays).AddHours(currentCountPoints*3), 
                                Temp = new TempApiDTO() { Value = currentTemp++ } 
                            });
                };                 
            }

            // Act
            var result = weatherInfoApiDTO.GetMeanValueWeather().ToList();

            // Assert
            var expected = new List<WeatherForDateDTO>();
            for (int currentCountDays = 0; currentCountDays < countdays; currentCountDays++)
            {
                var currentTemp = currentCountDays * 2 + (countPountInDay - 1) / 2.0 + temp;
                expected.Add(new WeatherForDateDTO() { DateTime = dateTime.AddDays(currentCountDays), Temp = currentTemp });
            }           
                
            Assert.True(new CompareLogic().Compare(expected, result).AreEqual);
        }
    }
}
