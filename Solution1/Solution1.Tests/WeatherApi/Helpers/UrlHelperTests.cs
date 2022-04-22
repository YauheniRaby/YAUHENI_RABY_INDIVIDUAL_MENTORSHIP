using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherApi.Helpers;
using Xunit;

namespace Weather.Tests.WeatherApi.Helpers
{
    public  class UrlHelperTests
    {


        //first second Third
        [Theory]
        [InlineData("First", "First")]
        [InlineData("FirstSecond", "First", "Second")]
        [InlineData("FirstSecondThird", "First", "Second", "Third")]
        public void Configure_SetStartupSettingsForBackgroundJob_Success(string expected, params string[] partsUrl)
        {
            //Act
            var actual = UrlHelper.Combine(partsUrl);

            //Assert
            Assert.Equal(expected, actual);            
        }
    }
}
