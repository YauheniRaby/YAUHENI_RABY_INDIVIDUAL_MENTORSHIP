using BusinessLayer.DTOs;
using BusinessLayer.Extensions;
using KellermanSoftware.CompareNetObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public void FillCommentByTemp_FillsComment_Success(double temp, string comment)
        {
            // Arrange

            var weatherDto = new WeatherDTO() { Temp = temp };

            // Act
            var result = weatherDto.FillCommentByTemp();

            // Assert
            var expectedWeatherDto = new WeatherDTO() { Temp = temp, Comment = comment };
            Assert.True(new CompareLogic().Compare(expectedWeatherDto, result).AreEqual);
        }
    }
}
