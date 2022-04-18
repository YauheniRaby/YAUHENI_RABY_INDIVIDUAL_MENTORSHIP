using ConsoleApp;
using ConsoleApp.Configuration;
using ConsoleApp.Configuration.Abstract;
using Moq;
using System;
using System.IO;
using System.Threading.Tasks;
using Weather.Tests.Infrastructure;
using Xunit;

namespace Weather.Tests.Integration
{
    public class ConsoleAppTests
    {
        private readonly Mock<IConfig> _config;

        private readonly string _cityName = "Minsk";
        private readonly string _menu = Menu.GetMenuRepresentation();
        private readonly string _temperaturePattern = @"-*\d{1,2}.\d{1,2}";
        private readonly string _commentPattern =
            $"({BusinessLayer.Constants.WeatherComments.DressWarmly}" +
            $"|{BusinessLayer.Constants.WeatherComments.Fresh}" +
            $"|{BusinessLayer.Constants.WeatherComments.GoodWeather}" +
            $"|{BusinessLayer.Constants.WeatherComments.GoToBeach})";

        public ConsoleAppTests()
        {
            _config = new Mock<IConfig>();
        }

        [Fact]
        public async Task Main_GetCurrentWeather_Success()
        {
            // Arrange
            var pattern = $"^{_menu}{Environment.NewLine}" +
                $"In {_cityName} {_temperaturePattern} C. {_commentPattern}{Environment.NewLine}" +
                $"{_menu}{Environment.NewLine}" +
                $"Сlose the application{Environment.NewLine}$";

            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            Console.SetIn(new StringReader($"1{Environment.NewLine}{_cityName}{Environment.NewLine}0{Environment.NewLine}"));

            //Act
            await Program.Main();

            //Assert
            var result = consoleOutput.ToString().Replace($"Please, enter city name:{Environment.NewLine}", string.Empty);
            Assert.Matches(pattern, result);
        }

        [Fact]
        public async Task Main_GetForecastWeather_Success()
        {
            // Arrange
            var countDays = 2;
            var forecastPattern = $@"({Environment.NewLine}Day [0-5]{{1}} \(\w+ \d{{1,2}}, \d{{4}}\): {_temperaturePattern} C. {_commentPattern}\s{{0,1}}){{{countDays},{countDays + 1}}}";
            var pattern = $"^{_menu}{Environment.NewLine}" +
                $"Please, enter city name:{Environment.NewLine}" +
                $"Please, enter count day:{Environment.NewLine}" +
                $"{_cityName} weather forecast:{forecastPattern}{Environment.NewLine}" +
                $"{_menu}{Environment.NewLine}" +
                $"Сlose the application{Environment.NewLine}$";

            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);
            Console.SetIn(new StringReader($"2{Environment.NewLine}{_cityName}{Environment.NewLine}{countDays}{Environment.NewLine}0{Environment.NewLine}"));

            //Act
            await Program.Main();

            //Assert
            Assert.Matches(pattern, consoleOutput.ToString());
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task Main_GetBestWeatherForArrayCities_Success(bool isDebugMode)
        {
            _config
                .Setup(config => config.AppConfig)
                .Returns(new AppConfiguration());
            _config
                .Setup(config => config.ApiConfig)
                .Returns(new WeatherApiConfiguration());
            _config
                .Setup(config => config.DbConfig)
                .Returns(new DbConfiguration());

            var ninjectKernel = Program.GetRegistrarDependencies(_config.Object);

            var arrayCityNames = $"{_cityName}, ААА, {string.Empty}, Paris";
            string cityPattern = arrayCityNames.Replace(" ", "").Replace(',', '|');

            var resultRequestPattern = $@"\(City with the highest temperature {_temperaturePattern} C: {cityPattern}. " +
                $@"Successful request count: \d, failed: \d.|" +
                $@"Error, no successful requests. Failed requests count: \d\)";

            var successResponsePattern = $"Success case:" +
                $@"\({Environment.NewLine}City: '{cityPattern}', Temp: {_temperaturePattern}, Timer: \d{{1,}} ms.\)+";
            var failResponsePattern = $"On fail:" +
                $@"\({Environment.NewLine}City: '{cityPattern}', ErrorMessage: \w+, Timer: \d{{1,}} ms.\)+";

            var debugInfoPattern = isDebugMode
                ? $@"\({successResponsePattern}{Environment.NewLine}|" +
                $@"{successResponsePattern}{Environment.NewLine}{failResponsePattern}{Environment.NewLine}|" +
                $@"{failResponsePattern}{Environment.NewLine}\)"
                : string.Empty;

            var pattern = $@"^{_menu}{Environment.NewLine}" +
                $@"Please, enter array city name \(separator symbal - ','\) :" +
                $"{resultRequestPattern}{Environment.NewLine}{debugInfoPattern}" +
                $"{_menu}{Environment.NewLine}" +
                $"Сlose the application{Environment.NewLine}$";

            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);
            Console.SetIn(new StringReader($"3{Environment.NewLine}{arrayCityNames}{Environment.NewLine}0{Environment.NewLine}"));

            //Act
            await Program.StartUserCommunication(ninjectKernel);

            //Assert
            Assert.Matches(pattern, consoleOutput.ToString());
        }
    }
}
