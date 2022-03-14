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
        private readonly string cityName = "Minsk";
        private readonly string menu = Menu.GetMenuRepresentation();
        private readonly string temperaturePattern = @"-*\d{1,2}.\d{1,2}";
        private readonly string commentPattern =
            $"({BusinessLayer.Constants.WeatherComments.DressWarmly}" +
            $"|{BusinessLayer.Constants.WeatherComments.Fresh}" +
            $"|{BusinessLayer.Constants.WeatherComments.GoodWeather}" +
            $"|{BusinessLayer.Constants.WeatherComments.GoToBeach})";

        [Fact]
        public async Task Main_GetCurrentWeather_Seccess()
        {
            // Arrange
            var pattern = $"^{menu}{Environment.NewLine}" +
                $"In {cityName} {temperaturePattern} C. {commentPattern}{Environment.NewLine}" +
                $"{menu}{Environment.NewLine}" +
                $"Сlose the application{Environment.NewLine}$";

            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            Console.SetIn(new StringReader($"1{Environment.NewLine}{cityName}{Environment.NewLine}0{Environment.NewLine}"));

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
            var countDays = 2;
            var forecastPattern = $@"({Environment.NewLine}Day [0-5]{{1}} \(\w+ \d{{1,2}}, \d{{4}}\): {temperaturePattern} C. {commentPattern}\s{{0,1}}){{{countDays},{countDays + 1}}}";
            var pattern = $"^{menu}{Environment.NewLine}" +
                $"Please, enter city name:{Environment.NewLine}" +
                $"Please, enter count day:{Environment.NewLine}" +
                $"{cityName} weather forecast:{forecastPattern}{Environment.NewLine}" +
                $"{menu}{Environment.NewLine}" +
                $"Сlose the application{Environment.NewLine}$";

            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);
            Console.SetIn(new StringReader($"2{Environment.NewLine}{cityName}{Environment.NewLine}{countDays}{Environment.NewLine}0{Environment.NewLine}"));
            
            //Act
            await Program.Main();

            //Assert
            Assert.Matches(pattern, consoleOutput.ToString());
        }        
    }
}
