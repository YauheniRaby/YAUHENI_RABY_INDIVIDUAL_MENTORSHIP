using BusinessLayer;
using BusinessLayer.DTOs;
using BusinessLayer.Extensions;
using System.Collections.Generic;
using Xunit;

namespace Weather.Tests.BL.Extensions
{
    public class BaseWeatherDTOExtensionsTests
    {
        public static IEnumerable<object[]> ParamsForFillsCommentTest =>
            new List<object[]>
            {
                new object[] { new WeatherDTO() { Temp = -10 }, Constants.WeatherComments.DressWarmly },
                new object[] { new WeatherForDateDTO() { Temp = -1 }, Constants.WeatherComments.DressWarmly },
                new object[] { new WeatherDTO() { Temp = 0 }, Constants.WeatherComments.Fresh },
                new object[] { new WeatherForDateDTO() { Temp = 19 }, Constants.WeatherComments.Fresh },
                new object[] { new WeatherDTO() { Temp = 20 }, Constants.WeatherComments.GoodWeather },
                new object[] { new WeatherForDateDTO() { Temp = 29 }, Constants.WeatherComments.GoodWeather },
                new object[] { new WeatherDTO() { Temp = 30 }, Constants.WeatherComments.GoToBeach },
                new object[] { new WeatherForDateDTO() { Temp = 50 }, Constants.WeatherComments.GoToBeach }
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
