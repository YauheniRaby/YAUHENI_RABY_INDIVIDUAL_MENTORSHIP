using BusinessLayer.DTOs;
using BusinessLayer.Vlidators;
using FluentValidation;
using System.Threading.Tasks;
using Xunit;
using FluentValidation.TestHelper;
using System.Linq;
using System.Collections.Generic;

namespace Weather.Tests.BL.Validators
{
    public class ForecastWeatherRequestDTOValidatorTests
    {
        private readonly IValidator<ForecastWeatherRequestDTO> _validator;

        public ForecastWeatherRequestDTOValidatorTests()
        {
            int minCountDays = 0;
            int maxCountDays = 5;

            _validator = new ForecastWeatherRequestDTOValidator(minCountDays, maxCountDays);
        }

        public static IEnumerable<object[]> DataForValidationTestAllRules =>
            new List<object[]>
            {
                new object[]{ "Minsk", 3, true, new Dictionary<string, string>()},
                new object[]
                {
                    "Minsk",
                    -1,
                    false,
                    new Dictionary<string, string>()
                    {
                        { nameof(ForecastWeatherRequestDTO.PeriodOfDays), "'Period Of Days' must be between 0 and 5. You entered -1." }
                    }
                },
                new object[]
                {
                    string.Empty,
                    3,
                    false,
                    new Dictionary<string, string>()
                    {
                        { nameof(ForecastWeatherRequestDTO.CityName), "'City Name' must not be empty." }
                    }
                },
                new object[]
                {
                    "aaaaaaaaaaaaaaaaaaaaa",
                    3,
                    false,
                    new Dictionary<string, string>()
                    {
                        { nameof(ForecastWeatherRequestDTO.CityName), "The length of 'City Name' must be 20 characters or fewer. You entered 21 characters." }
                    }
                },
                new object[]
                {
                    string.Empty,
                    6,
                    false,
                    new Dictionary<string, string>()
                    {
                        { nameof(ForecastWeatherRequestDTO.CityName), "'City Name' must not be empty." },
                        { nameof(ForecastWeatherRequestDTO.PeriodOfDays), "'Period Of Days' must be between 0 and 5. You entered 6." }
                    }
                }
            };

        [Theory]
        [MemberData(nameof(DataForValidationTestAllRules))]
        public async Task GetByCityNameAsync_CheckValidationAllRules_Success(string cityName, int countDay, bool isValid, Dictionary<string, string> errors)
        {
            // Arrange
            var countErrors = errors.Count();
            var forecastWeatherRequestDTO = new ForecastWeatherRequestDTO() { CityName = cityName, PeriodOfDays = countDay };

            // Act
            var result = await _validator.TestValidateAsync(forecastWeatherRequestDTO, options => options.IncludeAllRuleSets());

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Errors);
            Assert.Equal(countErrors, result.Errors.Count);
            if (isValid)
            {
                Assert.True(result.IsValid);
                result.ShouldNotHaveValidationErrorFor(x => x.CityName);
                result.ShouldNotHaveValidationErrorFor(x => x.PeriodOfDays);
            }
            else
            {
                Assert.False(result.IsValid);

                errors.ToList().ForEach(x =>
                {
                    result.ShouldHaveValidationErrorFor(x.Key);
                    Assert.Equal(x.Value, result.Errors.Single(errors => errors.PropertyName == x.Key).ErrorMessage);
                });
            }
        }

        public static IEnumerable<object[]> DataForValidationTestOnlyCityName =>
            new List<object[]>
            {
                new object[]{ "Minsk", -1, true},
                new object[]{ "Minsk", 0, true },
                new object[]{ string.Empty, 3, false, "'City Name' must not be empty." },
                new object[]
                {
                    "aaaaaaaaaaaaaaaaaaaaa",
                    -1,
                    false,
                    "The length of 'City Name' must be 20 characters or fewer. You entered 21 characters."
                }
            };

        [Theory]
        [MemberData(nameof(DataForValidationTestOnlyCityName))]
        public async Task GetByCityNameAsync_CheckValidationOnlySityNameRules_Success(string cityName, int countDay, bool isValid, string message = null)
        {
            // Arrange
            var forecastWeatherRequestDTO = new ForecastWeatherRequestDTO() { CityName = cityName, PeriodOfDays = countDay };
            // Act
            var result = await _validator
                .TestValidateAsync(
                    forecastWeatherRequestDTO,
                    options => options.IncludeRuleSets(BusinessLayer.Constants.Validators.OnlyCityName));

            // Assert
            Assert.NotNull(result);
            if (isValid)
            {
                Assert.True(result.IsValid);
                result.ShouldNotHaveValidationErrorFor(x => x.CityName);
            }
            else
            {
                Assert.False(result.IsValid);
                result.ShouldHaveValidationErrorFor(x => x.CityName);
                Assert.Single(result.Errors);
                Assert.Equal(message, result.Errors.Single(x => x.PropertyName == nameof(ForecastWeatherDTO.CityName)).ErrorMessage);
            }
        }
    }
}
