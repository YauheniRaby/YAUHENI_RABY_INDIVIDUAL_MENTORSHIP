using ConsoleApp;
using ConsoleApp.Services.Abstract;
using Moq;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Weather.Tests.Infrastructure;
using Xunit;

namespace Weather.Tests.Integration
{
    public class ConsoleAppTests
    {
        private readonly Mock<ICloseApplicationService> _closeApplicationService;

        public ConsoleAppTests()
        {
            _closeApplicationService = new Mock<ICloseApplicationService>();
        }

        [Fact]
        public async Task Main_GetCurrentWeather_Seccess()
        {
            // Arrange
            var cityName = "Minsk";
            
            var temperaturePattern = @"-*\d{1,2}.\d{1,2}";
            var menu = Menu.GetMenuRepresentation();
            var commentRegex = GetCommentsPattern();
            var pattern = $"^{menu}\r\nIn {cityName} {temperaturePattern} C. {commentRegex}\r\n{menu}\r\n$";

            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            Console.SetIn(new StringReader(string.Format("1{0}{1}{0}0{0}", Environment.NewLine, cityName)));

            //Act
            await Program.Main();

            //Assert
            var result = consoleOutput.ToString().Replace("Please, enter city name:\r\n", string.Empty);
            Assert.Matches(pattern, result);
        }

        [Fact]
        public async Task Main_GetForecastWeather_Seccess()
        {
            // Arrange
            var cityName = "Minsk";
            var countDays = 2;
            var menu = Menu.GetMenuRepresentation();

            var temperaturePattern = @"-*\d{1,2}.\d{1,2}";
            var forecastPattern = $@"(\nDay [0-5]{{1}} \(\d{{1,2}} \w+ \d{{4}} г.\): {temperaturePattern} C. {GetCommentsPattern()}\s{{0,1}}){{{countDays},{countDays + 1}}}";
            var pattern = $"^{menu}\r\nPlease, enter city name:\r\nPlease, enter count day:\r\n{cityName} weather forecast: {forecastPattern}\r\n{menu}\r\n$";

            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            Console.SetIn(new StringReader(string.Format("2{0}{1}{0}{2}{0}0{0}", Environment.NewLine, cityName, countDays)));
            
            //Act
            await Program.Main();

            //Assert
            Assert.Matches(pattern, consoleOutput.ToString());
        }

        private string GetCommentsPattern()
        {
            var comment1 = "Dress warmly.";
            var comment2 = "It's fresh.";
            var comment3 = "Good weather.";
            var comment4 = "It's time to go to the beach.";
            return $"({comment1}|{comment2}|{comment3}|{comment4})";
        }
    }
}
