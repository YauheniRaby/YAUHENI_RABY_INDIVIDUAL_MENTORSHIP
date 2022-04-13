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
        private readonly string _cityName = "Minsk";
        private readonly DateTime _date = new(2022, 10, 10);
        private readonly double _temperature1 = 18;
        private readonly string _comment1 = Constants.WeatherComments.Fresh;        
        private readonly double _temperature2 = 25;
        private readonly string _comment2 = Constants.WeatherComments.GoodWeather;

        [Fact]
        public void GetMultiStringRepresentation_GetMultiStringRepresentationFromWeatherDTO_Success()
        {
            // Arrange
            var forecastWeatherDTO = GetForecastWeatherDTO(_comment1, _comment2);
            var culture = new CultureInfo("en-US");

            // Act
            var result = forecastWeatherDTO.GetMultiStringRepresentation();
            
            // Assert
            var expected = $"{_cityName} weather forecast:" +
                $"{Environment.NewLine}Day 0 ({_date.ToString(Constants.DateTimeFormats.Date, culture)}): {_temperature1:f1} C. {_comment1}" +
                $"{Environment.NewLine}Day 1 ({_date.AddDays(1).ToString(Constants.DateTimeFormats.Date, culture)}): {_temperature2:f1} C. {_comment2}";
            
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
            Assert.Equal(_comment1, forecastWeatherDTO.WeatherForPeriod[0].Comment);
            Assert.Equal(_comment2, forecastWeatherDTO.WeatherForPeriod[1].Comment);
        }

        private ForecastWeatherDTO GetForecastWeatherDTO(string comment1, string comment2)
        {
            var weatherForPeriod = new List<WeatherForDateDTO>()
            {
                new WeatherForDateDTO(){ DateTime = _date, Temp = _temperature1, Comment = comment1 },
                new WeatherForDateDTO(){ DateTime = _date.AddDays(1), Temp = _temperature2, Comment = comment2 }
            };
            return new ForecastWeatherDTO() { CityName = _cityName, WeatherForPeriod = weatherForPeriod };
        }
    }
}
