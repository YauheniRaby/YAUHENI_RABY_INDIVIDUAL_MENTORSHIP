using BusinessLayer;
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
            var cityName = "Minsk";
            var temp = 10;
            var comment = DataAccessLayer.Constants.WeatherComments.Fresh;
            var weatherDto = new WeatherDTO() { CityName = cityName, Temp = temp, Comment = comment };

            // Act
            var result = weatherDto.GetStringRepresentation();

            // Assert
            var expected = $"In {cityName} {temp} C. {comment}";
            Assert.Equal(expected, result);
        }
    }
}
