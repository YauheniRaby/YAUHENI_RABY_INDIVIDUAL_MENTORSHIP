using BusinessLayer.DTOs;
using BusinessLayer.Extensions;
using KellermanSoftware.CompareNetObjects;
using Xunit;

namespace Solution1.Tests.BL.Tests.Extension
{
    public class WeatherDtoExtensionTests
    {
        [Theory]
        [InlineData(-1, "Dress warmly.")]
        [InlineData(10, "It's fresh")]
        [InlineData(25, "Good weather.")]
        [InlineData(40, "It's time to go to the beach.")]
        public void FillCommentByTemp_ReternedWeatherDTO_with_Comment(double temp, string comment)
        {
            // Arrange
            var weatherDtoWithoutCooment = new WeatherDTO() { Temp = temp };
            var expectedObject = new WeatherDTO() { Temp = temp, Comment = comment };

            // Act
            var actualObject = weatherDtoWithoutCooment.FillCommentByTemp();

            // Assert
            Assert.True(new CompareLogic().Compare(actualObject, expectedObject).AreEqual);
        }
    }
}
