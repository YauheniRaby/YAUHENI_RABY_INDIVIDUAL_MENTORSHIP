using BusinessLayer.DTOs;
using BusinessLayer.Extensions;
using System.Collections.Generic;
using Weather.Tests.Infrastructure.Enums;
using Weather.Tests.Infrastructure.Extensions;
using Xunit;

namespace Weather.Tests.BL.Extensions
{
    public class BaseWeatherDTOExtensionsTests
    {
        public static IEnumerable<object[]> ParamsForFillsCommentTest =>
            new List<object[]>
            {
                new object[] { new WeatherDTO() { Temp = -10 }, WeatherComments.DressWarmly.GetString() },
                new object[] { new WeatherForDateDTO() { Temp = -1 }, WeatherComments.DressWarmly.GetString() },
                new object[] { new WeatherDTO() { Temp = 0 }, WeatherComments.Fresh.GetString() },
                new object[] { new WeatherForDateDTO() { Temp = 19 }, WeatherComments.Fresh.GetString() },
                new object[] { new WeatherDTO() { Temp = 20 }, WeatherComments.GoodWeather.GetString() },
                new object[] { new WeatherForDateDTO() { Temp = 29 }, WeatherComments.GoodWeather.GetString() },
                new object[] { new WeatherDTO() { Temp = 30 }, WeatherComments.GoToBeach.GetString() },
                new object[] { new WeatherForDateDTO() { Temp = 50 }, WeatherComments.GoToBeach.GetString() }
            };

        [Theory]
        [MemberData(nameof(ParamsForFillsCommentTest))]
        public void FillCommentByTemp_FillsComment_Success(BaseWeatherDTO baseWeatherDTO, string comment)
        {
            // Arrange

            // Act
            baseWeatherDTO.FillCommentByTemp();

            // Assert
            Assert.Equal(comment, baseWeatherDTO.Comment);
        }        
    }
}
