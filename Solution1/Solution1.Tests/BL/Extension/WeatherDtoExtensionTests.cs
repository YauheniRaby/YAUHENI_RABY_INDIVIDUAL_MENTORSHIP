using BusinessLayer.DTOs;
using BusinessLayer.Extensions;
using KellermanSoftware.CompareNetObjects;
using Xunit;

namespace Weather.Tests.BL.Extension
{
    public class WeatherDtoExtensionTests
    {
        [Theory]
        [InlineData(-1, "Dress warmly.")]
        [InlineData(10, "It's fresh.")]
        [InlineData(25, "Good weather.")]
        [InlineData(40, "It's time to go to the beach.")]
        public void FillCommentByTemp_FillsComment_Success(double temp, string comment)
        {
            // Arrange
            var weatherDto = new WeatherDTO() { Temp = temp };

            // Act
            var result = weatherDto.FillCommentByTemp();

            // Assert
            var expectedWeatherDTO = new WeatherDTO() { Temp = temp, Comment = comment };
            Assert.True(new CompareLogic().Compare(expectedWeatherDTO, result).AreEqual);
        }

        [Fact]
        public void GetStringRepresentation_GetStringRepresentationFromWeatherDTO_Success()
        {
            // Arrange
            var weatherDto = new WeatherDTO() { CityName = "Minsk", Temp = 10, Comment = "It's fresh."};

            // Act
            var result = weatherDto.GetStringRepresentation();

            // Assert
            var expected = $"In Minsk 10 C. It's fresh.";
            Assert.True(new CompareLogic().Compare(result, expected).AreEqual);
        }
    }
}
