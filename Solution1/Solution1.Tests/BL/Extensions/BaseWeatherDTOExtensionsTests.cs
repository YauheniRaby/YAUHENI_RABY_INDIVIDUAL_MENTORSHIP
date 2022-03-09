using BusinessLayer.DTOs;
using BusinessLayer.Extensions;
using Xunit;

namespace Weather.Tests.BL.Extensions
{
    public class BaseWeatherDTOExtensionsTests
    {
        [Theory]
        [InlineData(-1, "Dress warmly.")]
        [InlineData(10, "It's fresh.")]
        [InlineData(25, "Good weather.")]
        [InlineData(40, "It's time to go to the beach.")]
        public void FillCommentByTemp_FillsCommentForWeatherDTO_Success(double temp, string comment)
        {
            // Arrange
            var weatherDto = new WeatherDTO() { Temp = temp };

            // Act
            weatherDto.FillCommentByTemp();

            // Assert
            Assert.Equal(comment, weatherDto.Comment);
        }

        [Theory]
        [InlineData(-5, "Dress warmly.")]
        [InlineData(15, "It's fresh.")]
        [InlineData(27, "Good weather.")]
        [InlineData(50, "It's time to go to the beach.")]
        public void FillCommentByTemp_FillsCommentForWeatherForDateDTO_Success(double temp, string comment)
        {
            // Arrange
            var weatherForDateDTO = new WeatherForDateDTO() { Temp = temp };

            // Act
            weatherForDateDTO.FillCommentByTemp();

            // Assert
            Assert.Equal(comment, weatherForDateDTO.Comment);
        }
    }
}
