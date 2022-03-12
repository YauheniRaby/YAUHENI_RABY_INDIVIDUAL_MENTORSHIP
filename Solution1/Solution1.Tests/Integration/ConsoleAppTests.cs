using ConsoleApp;
using System;
using System.IO;
using System.Threading.Tasks;
using Weather.Tests.Infrastructure;
using Xunit;

namespace Weather.Tests.Integration
{
    public class ConsoleAppTests
    {
        [Fact]
        public async Task Main_GetCurrentWeather_Seccess()
        {
            // Arrange
            var cityName = "Minsk";
            
            var temperaturePattern = @"-*\d{1,2}.\d{1,2}";
            var menu = Menu.GetMenuRepresentation();
            var commentRegex = GetCommentsPattern();
            var pattern = $"^{menu}{Environment.NewLine}" +
                $"In {cityName} {temperaturePattern} C. {commentRegex}{Environment.NewLine}" +
                $"{menu}{Environment.NewLine}" +
                $"Сlose the application{Environment.NewLine}$";

            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            Console.SetIn(new StringReader(string.Format("1{0}{1}{0}0{0}", Environment.NewLine, cityName)));

            //Act
            await Program.Main();

            //Assert
            var result = consoleOutput.ToString().Replace($"Please, enter city name:{Environment.NewLine}", string.Empty);
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
            var forecastPattern = $@"({Environment.NewLine}Day [0-5]{{1}} \(\w+ \d{{1,2}}, \d{{4}}\): {temperaturePattern} C. {GetCommentsPattern()}\s{{0,1}}){{{countDays},{countDays + 1}}}";
            var pattern = $"^{menu}{Environment.NewLine}" +
                $"Please, enter city name:{Environment.NewLine}" +
                $"Please, enter count day:{Environment.NewLine}" +
                $"{cityName} weather forecast:{forecastPattern}{Environment.NewLine}" +
                $"{menu}{Environment.NewLine}" +
                $"Сlose the application{Environment.NewLine}$";

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
