using ConsoleApp;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Weather.Tests.Integration
{
    public class ConsoleAppTests
    {
        [Fact]
        public async Task Communicate_EnterCityName_HandlingExceptionAndShowNoticeAsync()
        {
            // Arrange
            var cityName = "Minsk";
            
            var doublePettern = @"\d{1,2}.\d{1,2}";
            var comment1 = "Dress warmly.";
            var comment2 = "It's fresh.";
            var comment3 = "Good weather.";
            var comment4 = "It's time to go to the beach.";
            var pattern = $"^In {cityName} {doublePettern} C. ({comment1}|{comment2}|{comment3}|{comment4})\r\n$";

            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            Console.SetIn(new StringReader(string.Format("{0}{1}exit", cityName, Environment.NewLine)));

            //Act
            await Program.Main();

            //Assert
            var result = consoleOutput.ToString().Replace("Please, enter city name:\r\n", string.Empty);
            Assert.Matches(pattern, result);
        }
    }
}
