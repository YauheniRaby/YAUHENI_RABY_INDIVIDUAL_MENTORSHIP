using BusinessLayer.DTOs;
using BusinessLayer.Vlidators;
using FluentValidation;
using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Weather.Tests.BL.Validators
{
    public class HistoryWeatherRequestDTOValidatorTests
    {
        private readonly IValidator<HistoryWeatherRequestDTO> _validator;

        public HistoryWeatherRequestDTOValidatorTests()
        {
            _validator = new HistoryWeatherRequestDTOValidator();
        }

        public static IEnumerable<object[]> DataForValidationTest =>
            new List<object[]>
            {
                new object[]
                {
                    new HistoryWeatherRequestDTO() { CityName = "Minsk", StartPeriod = new DateTime(2022, 01, 01, 15, 30, 00), EndPeriod = new DateTime(2022, 01, 02, 14, 10, 00), },
                    true,
                    new Dictionary<string, string>()
                },
                new object[]
                {
                    new HistoryWeatherRequestDTO() { CityName = string.Empty, StartPeriod = new DateTime(2022, 01, 01, 15, 30, 00), EndPeriod = new DateTime(2022, 01, 02, 14, 10, 00), },
                    false,
                    new Dictionary<string, string>()
                    {
                        { nameof(HistoryWeatherRequestDTO.CityName), "'City Name' must not be empty." }
                    }
                },
                new object[]
                {
                    new HistoryWeatherRequestDTO() { CityName = "aaaaaaaaaaaaaaaaaaaaa", StartPeriod = new DateTime(2022, 01, 01, 15, 30, 00), EndPeriod = new DateTime(2022, 01, 02, 14, 10, 00), },
                    false,
                    new Dictionary<string, string>()
                    {
                        { nameof(HistoryWeatherRequestDTO.CityName), "The length of 'City Name' must be 20 characters or fewer. You entered 21 characters." }
                    }
                },
                new object[]
                {
                    new HistoryWeatherRequestDTO() { CityName = "Minsk", StartPeriod = new DateTime(2022, 01, 02, 15, 30, 00), EndPeriod = new DateTime(2022, 01, 01, 14, 10, 00), },
                    false,
                    new Dictionary<string, string>()
                    {
                        { nameof(HistoryWeatherRequestDTO.StartPeriod) , "'Start Period' must be less than or equal to 'End Period'." }
                    }
                },
                new object[]
                {
                    new HistoryWeatherRequestDTO() { CityName = "Minsk", StartPeriod = (DateTime)default, EndPeriod = (DateTime)default },
                    false,
                    new Dictionary<string, string>()
                    {
                        { nameof(HistoryWeatherRequestDTO.StartPeriod) , "'Start Period' must not be empty." },
                        { nameof(HistoryWeatherRequestDTO.EndPeriod) , "'End Period' must not be empty." }
                    }
                },
                new object[]
                {
                    new HistoryWeatherRequestDTO() { CityName = string.Empty, StartPeriod = new DateTime(2022, 01, 01), EndPeriod = (DateTime)default },
                    false,
                    new Dictionary<string, string>()
                    {
                        { nameof(HistoryWeatherRequestDTO.CityName), "'City Name' must not be empty." },
                        { nameof(HistoryWeatherRequestDTO.EndPeriod) , "'End Period' must not be empty." },
                        { nameof(HistoryWeatherRequestDTO.StartPeriod) , "'Start Period' must be less than or equal to 'End Period'." }
                    }
                }
            };

        [Theory]
        [MemberData(nameof(DataForValidationTest))]
        public async Task GetHistoryWeatherByCityNameAsync_CheckValidation_Success(HistoryWeatherRequestDTO requestHistoryWeatherDto, bool isValid, Dictionary<string, string> errors)
        {
            // Arrange
            var countErrors = errors.Count;
            
            // Act
            var result = await _validator.TestValidateAsync(requestHistoryWeatherDto);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Errors);
            Assert.Equal(countErrors, result.Errors.Count);
            if (isValid)
            {
                Assert.True(result.IsValid);
                result.ShouldNotHaveValidationErrorFor(x => x.CityName);
                result.ShouldNotHaveValidationErrorFor(x => x.StartPeriod);
                result.ShouldNotHaveValidationErrorFor(x => x.EndPeriod);
                result.ShouldNotHaveValidationErrorFor(x => x);
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

    }
}
