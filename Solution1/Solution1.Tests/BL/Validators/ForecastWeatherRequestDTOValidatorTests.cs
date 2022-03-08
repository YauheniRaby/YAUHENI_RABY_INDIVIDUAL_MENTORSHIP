using BusinessLayer.DTOs;
using BusinessLayer.Vlidators;
using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Weather.Tests.BL.Validators
{
    public class ForecastWeatherRequestDTOValidatorTests
    {
        private readonly IValidator<ForecastWeatherRequestDTO> _validator;

        public ForecastWeatherRequestDTOValidatorTests()
        {
            _validator = new ForecastWeatherRequestDTOValidator();
        }

        [Theory]
        [InlineData("Minsk", 3, false, true)]
        [InlineData(default, 6, false, false)]
        [InlineData("Minsk", -1, true, true)]
        public async Task GetByCityNameAsync_CheckValidationScript_Success(string cityName, int countDay, bool isOnlyCityNameRule, bool isValid)
        {
            // Arrange
            var forecastWeatherRequestDTO = new ForecastWeatherRequestDTO() { CityName = cityName, PeriodOfDays = countDay};

            // Act
            ValidationResult result;
            if (isOnlyCityNameRule)
            {
                result = await _validator.ValidateAsync(forecastWeatherRequestDTO, options => options.IncludeRuleSets("CityName"));
            }
            else
            {
                result = await _validator.ValidateAsync(forecastWeatherRequestDTO);
            }

            // Assert
            if (isValid)
            {
                Assert.True(result.IsValid);
            }
            else
            {
                Assert.True(!result.IsValid);
            }
        }
    }
}
