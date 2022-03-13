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
                new object[] { new WeatherDTO() { Temp = -10 }, "Dress warmly." },
                new object[] { new WeatherForDateDTO() { Temp = -1 }, "Dress warmly." },
                new object[] { new WeatherDTO() { Temp = 0 }, "It's fresh." },
                new object[] { new WeatherForDateDTO() { Temp = 19 }, "It's fresh." },
                new object[] { new WeatherDTO() { Temp = 20 }, "Good weather." },
                new object[] { new WeatherForDateDTO() { Temp = 29 }, "Good weather." },
                new object[] { new WeatherDTO() { Temp = 30 }, "It's time to go to the beach." },
                new object[] { new WeatherForDateDTO() { Temp = 50 }, "It's time to go to the beach." }
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
