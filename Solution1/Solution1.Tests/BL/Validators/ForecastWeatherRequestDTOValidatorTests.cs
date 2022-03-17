using BusinessLayer.Configuration.Abstract;
using BusinessLayer.DTOs;
using BusinessLayer.Vlidators;
using ConsoleApp.Configuration;
using FluentValidation;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Weather.Tests.BL.Validators
{
    public class ForecastWeatherRequestDTOValidatorTests
    {
        private readonly IValidator<ForecastWeatherRequestDTO> _validator;
        private readonly Mock<IConfig> _config;

        public ForecastWeatherRequestDTOValidatorTests()
        {
            _config = new Mock<IConfig>();
            _config
               .Setup(config =>
                   config.MinCountDaysForecast)
               .Returns(1);
            _config
               .Setup(config =>
                   config.MaxCountDaysForecast)
               .Returns(5);

            _validator = new ForecastWeatherRequestDTOValidator(_config.Object);
        }

        [Theory]
        [InlineData("Minsk", 3, true, 0)]
        [InlineData("Minsk", -1, false, 1)]
        [InlineData(default, 3, false, 1)]
        [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", 3, false, 1)]
        [InlineData(default, 6, false, 2)]
        public async Task GetByCityNameAsync_CheckValidationAllRules_Success(string cityName, int countDay, bool isValid, int countErrorMessages)
        {
            // Arrange
            var forecastWeatherRequestDTO = new ForecastWeatherRequestDTO() { CityName = cityName, PeriodOfDays = countDay};
            
            // Act
            var result = await _validator.ValidateAsync(forecastWeatherRequestDTO, options => options.IncludeAllRuleSets());
            
            // Assert
            if (isValid)
            {
                Assert.True(result.IsValid);
            }
            else
            {
                Assert.False(result.IsValid);
                Assert.Equal(countErrorMessages, result.Errors.Count);
            }
        }

        [Theory]
        [InlineData("Minsk", -1, true)]
        [InlineData("Minsk", 0, true)]
        [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", -1, false)]
        [InlineData(default, 3, false)]
        public async Task GetByCityNameAsync_CheckValidationOnlySityNameRules_Success(string cityName, int countDay,  bool isValid)
        {
            // Arrange
            var forecastWeatherRequestDTO = new ForecastWeatherRequestDTO() { CityName = cityName, PeriodOfDays = countDay };

            // Act
            var result = await _validator.ValidateAsync(forecastWeatherRequestDTO, options => options.IncludeRuleSets("CityName"));

            // Assert
            if (isValid)
            {
                Assert.True(result.IsValid);
            }
            else
            {
                Assert.False(result.IsValid);
                Assert.Single(result.Errors);
            }
        }
    }
}
