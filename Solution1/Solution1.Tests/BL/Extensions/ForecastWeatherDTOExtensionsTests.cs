using BusinessLayer;
using BusinessLayer.DTOs;
using BusinessLayer.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace Weather.Tests.BL.Extensions
{
    public class ForecastWeatherDTOExtensionsTests
    {
        private readonly string cityName = "Minsk";
        private readonly DateTime date = new(2022, 10, 10);
        private readonly double temperature1 = 18;
        private readonly string comment1 = Constants.WeatherComments.Fresh;        
        private readonly double temperature2 = 25;
        private readonly string comment2 = Constants.WeatherComments.GoodWeather;

        [Fact]
        public void GetMultiStringRepresentation_GetMultiStringRepresentationFromWeatherDTO_Success()
        {
            // Arrange
            var forecastWeatherDTO = GetForecastWeatherDTO(comment1, comment2);
            var culture = new CultureInfo("en-US");

            // Act
            var result = forecastWeatherDTO.GetMultiStringRepresentation();
            
            // Assert
            var expected = $"{cityName} weather forecast:" +
                $"{Environment.NewLine}Day 0 ({date.ToString(Constants.Patterns.Date, culture)}): {temperature1:f1} C. {comment1}" +
                $"{Environment.NewLine}Day 1 ({date.AddDays(1).ToString(Constants.Patterns.Date, culture)}): {temperature2:f1} C. {comment2}";
            
            Assert.Equal(expected, result);
        }

        [Fact]
        public void FillCommentByTemp_FillsComment_Success()
        {
            // Arrange
            var forecastWeatherDTO = GetForecastWeatherDTO(null, null);

            // Act
            forecastWeatherDTO.FillCommentByTemp();

            // Assert            
            Assert.Equal(comment1, forecastWeatherDTO.WeatherForPeriod[0].Comment);
            Assert.Equal(comment2, forecastWeatherDTO.WeatherForPeriod[1].Comment);
        }
        private ForecastWeatherDTO GetForecastWeatherDTO(string comment1, string comment2)
        {
            var weatherForPeriod = new List<WeatherForDateDTO>()
            {
                new WeatherForDateDTO(){ DateTime = date, Temp = temperature1, Comment = comment1 },
                new WeatherForDateDTO(){ DateTime = date.AddDays(1), Temp = temperature2, Comment = comment2 }
            };
            return new ForecastWeatherDTO() { CityName = cityName, WeatherForPeriod = weatherForPeriod };
        }
    }
}
