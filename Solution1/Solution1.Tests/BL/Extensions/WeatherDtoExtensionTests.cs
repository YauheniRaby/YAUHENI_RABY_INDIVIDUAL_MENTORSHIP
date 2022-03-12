using BusinessLayer.DTOs;
using BusinessLayer.Extensions;
using Xunit;

namespace Weather.Tests.BL.Extensions
{
    public class WeatherDtoExtensionTests
    {
        [Fact]
        public void GetStringRepresentation_GetStringRepresentationFromWeatherDTO_Success()
        {
            // Arrange
            var weatherDto = new WeatherDTO() { CityName = "Minsk", Temp = 10, Comment = "It's fresh."};

            // Act
            var result = weatherDto.GetStringRepresentation();

            // Assert
            var expected = "In Minsk 10 C. It's fresh.";
            Assert.Equal(expected, result);
        }
    }
}
