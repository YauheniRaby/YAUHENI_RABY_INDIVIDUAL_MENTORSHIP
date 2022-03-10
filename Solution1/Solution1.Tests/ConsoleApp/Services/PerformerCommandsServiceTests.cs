using BusinessLayer.DTOs;
using BusinessLayer.Services.Abstract;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Weather.Tests.ConsoleApp.Services
{
    public class PerformerCommandsServiceTests
    {
        private readonly Mock<IWeatherServiсe> _weatherServiceMock;
        private readonly Mock<ILogger> _loggerMock;

        public PerformerCommandsServiceTests()
        {
            _loggerMock = new Mock<ILogger>();
            _weatherServiceMock = new Mock<IWeatherServiсe>();            
        }

        [Fact]
        public async Task GetCurrentWeatherAsync_EnterCityName_ShowStringRepresentationAsync()
        {
            // Arrange
            var cityName = "Minsk";
            var weatherDto = new WeatherDTO() { CityName = cityName, Temp = 10, Comment = "It's fresh." };
            _weatherServiceMock
                .Setup(weatherApiService =>
                    weatherApiService
                    .GetByCityNameAsync(cityName))
                .ReturnsAsync(weatherDto);

            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            Console.SetIn(new StringReader(string.Format("Minsk{0}", Environment.NewLine)));

            //Act
            await _performerCommandsService.GetCurrentWeatherAsync();

            //Assert
            var expected = string.Format("Please, enter city name:{0}In Minsk 10 C. It's fresh.{0}", Environment.NewLine);
            Assert.Equal(expected, consoleOutput.ToString());
        }

        [Fact]
        public async Task GetForecastByCityNameAsync_EnterCityNameAndCountDay_ShowMultiStringRepresentationAsync()
        {
            // Arrange
            var cityName = "Minsk";
            var countDay = 3;

            var weatherForPeriod = new List<WeatherForDateDTO>()
            {
                new WeatherForDateDTO() { DateTime = new DateTime(2022, 12, 10), Temp = -1, Comment = "Dress warmly." },
                new WeatherForDateDTO() { DateTime = new DateTime(2022, 12, 11), Temp = 10, Comment = "It's fresh."  },
                new WeatherForDateDTO() { DateTime = new DateTime(2022, 12, 12), Temp = 25, Comment = "Good weather."  },
                new WeatherForDateDTO() { DateTime = new DateTime(2022, 12, 13), Temp = 35, Comment = "It's time to go to the beach."  }
            };

            var forecastWeatherDTO = new ForecastWeatherDTO() { CityName = cityName, WeatherForPeriod = weatherForPeriod };

            _weatherServiceMock
                .Setup(weatherApiService =>
                    weatherApiService.GetForecastByCityNameAsync(cityName, countDay)
                    )
                .ReturnsAsync(forecastWeatherDTO);

            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);
            Console.SetIn(new StringReader(string.Format("{0}{1}{2}{1}", cityName, Environment.NewLine, countDay)));

            //Act
            await _performerCommandsService.GetForecastByCityNameAsync();

            //Assert
            var expected = "Please, enter city name:\r\n" +
                "Please, enter count day:\r\n" +
                "Minsk weather forecast: \n" +
                "Day 0 (10 декабря 2022 г.): -1,0 C. Dress warmly. \n" +
                "Day 1 (11 декабря 2022 г.): 10,0 C. It's fresh. \n" +
                "Day 2 (12 декабря 2022 г.): 25,0 C. Good weather. \n" +
                "Day 3 (13 декабря 2022 г.): 35,0 C. It's time to go to the beach.\r\n";

            Assert.Equal(expected, consoleOutput.ToString());
        }
        
        [Fact]
        public async Task CloseApplication_Success()
        {
            // Arrange
            var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);
            
            //Act
            await _performerCommandsService.CloseApplication();

            //Assert
            var expected = "Сlose the application\r\n";

            Assert.Equal(expected, consoleOutput.ToString());
        }
    }
}
